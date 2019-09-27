using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using SocketIOClient;
using UnityEngine;

enum Protocol {
    Http,
    Https,
    WS,
    WSS,
}

enum Status {
    NONE,
    CONNECTED,
    CLOSED,
    ERROR,
}

public class SocketIOHelper : MonoBehaviour {
    private static SocketIOHelper _instance = null;
    public static SocketIOHelper Instance { get => _instance; }

    [SerializeField]
    private Protocol protocol = Protocol.WS;

    [SerializeField]
    private String domainName = "127.0.0.1";

    [SerializeField]
    private int port = 80;

    public String url { get => getProtocol () + domainName + ":" + port; }

    private static SocketIO socket = null;

    private static Status status = Status.NONE;

    public static event Action<ServerCloseReason> onClosed = delegate (ServerCloseReason reason) { status = Status.CLOSED; print ("Socket Closed:" + (ServerCloseReason) reason); };
    public static event Action onConnected = delegate () { status = Status.CONNECTED; print ("Socket Connected"); };

    private void Awake () {
        if (_instance == null) {
            DontDestroyOnLoad (gameObject);
            _instance = this;
        } else {
            Destroy (gameObject);
        }
    }

    // Start is called before the first frame update

    void Start () {
        try {
            connectAsync ().Wait ();
        } catch (System.Exception error) {
            print ("Socket Connect Error:" + error);
        }
    }

    // Update is called once per frame
    void Update () {

    }

    public string getProtocol () {
        switch (protocol) {
            case Protocol.Http:
                return "http://";
            case Protocol.Https:
                return "https://";
            case Protocol.WSS:
                return "wss://";
            default:
                return "ws://";
        }
    }

    public async Task connectAsync () {
        if (socket == null) {
            print ("Socket Url:" + url);
            socket = new SocketIO (url);
            socket.OnClosed += onClosed;
            socket.OnConnected += onConnected;
            status = Status.NONE;
        }
        if (status != Status.CONNECTED) {
            try {
                await socket.ConnectAsync ();
            } catch (System.Exception error) {
                status = Status.ERROR;
                throw error;
            }
        }
    }
    public async Task closeAsync () {
        if (socket != null) {
            if (status == Status.CONNECTED) {
                await socket.CloseAsync ();
            }
            socket = null;
        }
    }

}