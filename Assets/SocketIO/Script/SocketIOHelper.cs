using System;
using System.Threading;
using System.Threading.Tasks;
using SocketIO.SocketEvent;
using UnityEngine;

namespace SocketIO {

    [Serializable]
    public class SocketIOHelper {

        public enum SocketIOState {
            NONE,
            CONNECTED,
            CONNECTING,
            CLOSED,
            CLOSEING,
            ERROR,
        }

        public SocketIOInfo info;

        private IO socket = null;

        private SocketIOState status = SocketIOState.NONE;

        // public event Action<ServerCloseReason> onClosed;
        // public event Action onConnected;

        // Start is called before the first frame update

        public Task initialize () {
            return connectAsync ();
        }

        public SocketIOState getStatus () {
            return status;
        }

        CancellationTokenSource _tokenSource;

        public Task connectAsync () {
            if (socket == null) {
                Debug.Log ("Socket Url:" + info.url);
                socket = new IO (info.url);
                _tokenSource = new CancellationTokenSource ();

                /**
                 * 监控 
                 **/
                socket.on (SocketStockEvent.Close, () => {
                    status = SocketIOState.CLOSED;
                    Debug.Log ("Socket Closed");
                });
                socket.on (SocketStockEvent.Connect, () => {
                    status = SocketIOState.CONNECTED;
                    Debug.Log ("Socket Connected");
                });

                status = SocketIOState.NONE;
            }

            return Task.Run (() => {
                if (status != SocketIOState.CONNECTED && status != SocketIOState.CONNECTING) {
                    try {
                        status = SocketIOState.CONNECTING;
                        socket.connect ().Wait ();
                    } catch (System.Exception error) {
                        status = SocketIOState.ERROR;
                        throw error;
                    }
                } else if (status == SocketIOState.CONNECTING) {
                    while (true) {
                        if (status != SocketIOState.CONNECTING) {
                            break;
                        }
                        Task.Delay (60 / 1000).Wait ();
                    }
                }
            }, _tokenSource.Token);

        }
        public async Task CloseAsync () {
            if (socket != null) {
                if (status == SocketIOState.CONNECTED && status != SocketIOState.CLOSEING) {
                    try {
                        _tokenSource.Cancel ();
                        _tokenSource.Dispose ();
                        status = SocketIOState.CLOSEING;
                        await socket.close ();
                    } catch (System.Exception error) {
                        status = SocketIOState.ERROR;
                        throw error;
                    }

                }
            }
        }

        public async Task ReConnectAsync () {
            if (status != SocketIOState.CONNECTED) {
                for (int i = 0; i < 3; i++) {
                    try {
                        Debug.Log ("Socket ReConnect :" + (i + 1));
                        await connectAsync ();
                        break;
                    } catch (System.Exception error) {
                        Debug.LogError (error);
                        await Task.Delay (1000);
                    }
                }
            }
        }

        public void On (string eventName, GeneralEventHandler callback) {
            if (socket != null) {
                socket.on (eventName, callback);
            }
        }
        public void On (SocketStockEvent eventName, StockEventHandler callback) {
            if (socket != null) {
                socket.on (eventName, callback);
            }
        }

        public Task EmitAsync (string eventName, object obj) {
            return Task.Run (() => {
                if (status == SocketIOState.CONNECTED) {
                    try {
                        Debug.Log ("Socket EmitAsync:" + eventName + " " + obj);
                        socket.emit (eventName, obj).Wait ();
                    } catch (System.Exception error) {
                        Debug.LogError (error);
                        throw error;
                    }
                }
            });
        }

        public Task EmitAsync (string eventName, object obj, ClientCallbackEventHandler callback) {
            return Task.Run (() => {
                if (status == SocketIOState.CONNECTED) {
                    Debug.Log ("Socket EmitAsync:" + eventName + " " + obj);
                    socket.emit (eventName, obj, callback).Wait ();
                }
            });
        }
    }

}