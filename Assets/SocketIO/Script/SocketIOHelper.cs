using System;
using System.Threading.Tasks;
using SocketIOClient;
using UnityEngine;

[Serializable]
public class SocketIOHelper {

    public enum SocketStatus {
        NONE,
        CONNECTED,
        CONNECTING,
        CLOSED,
        CLOSEING,
        ERROR,
    }

    public SocketIOInfo info;

    private SocketIO socket = null;

    private SocketStatus status = SocketStatus.NONE;

    // public event Action<ServerCloseReason> onClosed;
    // public event Action onConnected;

    // Start is called before the first frame update

    public async Task initialize () {
        await connectAsync ();
    }

    public SocketStatus getStatus () {
        return status;
    }
    public async Task connectAsync () {
        if (socket == null) {
            Debug.Log ("Socket Url:" + info.url);
            socket = new SocketIO (info.url);

            /**
             * 监控 
             **/
            socket.OnClosed += delegate (ServerCloseReason reason) { status = SocketStatus.CLOSED; Debug.Log ("Socket Closed:" + (ServerCloseReason) reason); };
            socket.OnConnected += delegate () { status = SocketStatus.CONNECTED; Debug.Log ("Socket Connected"); };

            /**
             * 外部监控
             **/
            // socket.OnClosed += onClosed;
            // socket.OnConnected += onConnected;

            status = SocketStatus.NONE;
        }
        if (status != SocketStatus.CONNECTED && status != SocketStatus.CONNECTING) {
            try {
                status = SocketStatus.CONNECTING;
                await socket.ConnectAsync ();
            } catch (System.Exception error) {
                status = SocketStatus.ERROR;
                throw error;
            }
        } else if (status == SocketStatus.CONNECTING) {
            while (true) {
                if (status != SocketStatus.CONNECTING) {
                    break;
                }
                await Task.Delay (60 / 1000);
            }
        }

    }
    public async Task closeAsync () {
        if (socket != null) {
            if (status == SocketStatus.CONNECTED && status != SocketStatus.CLOSEING) {
                try {
                    status = SocketStatus.CLOSEING;
                    await socket.CloseAsync ();
                } catch (System.Exception error) {
                    status = SocketStatus.ERROR;
                    throw error;
                }

            }
        }
    }

    public void On (string eventName, SocketIOClient.EventHandler handler) {
        if (socket != null) {
            socket.On (eventName, handler);
        }
    }

    public async Task EmitAsync (string eventName, object obj) {
        if (status == SocketStatus.CONNECTED) {
            try {
                Debug.Log ("Socket EmitAsync:" + eventName + " " + obj);
                await socket.EmitAsync (eventName, obj);
            } catch (System.Exception error) {
                Debug.LogError (error);
                throw error;
            }
        }
    }

    public async Task EmitAsync (string eventName, object obj, SocketIOClient.EventHandler callback) {
        if (status == SocketStatus.CONNECTED) {
            await socket.EmitAsync (eventName, obj, callback);
        }
    }
}