using System;
using UnityEngine;

[Serializable]
public class SocketIOInfo {
    public enum Protocol {
        Http,
        Https,
        WS,
        WSS,
    }

    [SerializeField]
    private Protocol protocol = Protocol.WS;

    [SerializeField]
    private string domainName = "127.0.0.1";

    [SerializeField]
    private int port = 80;

    public string url { get => getProtocol () + domainName + ":" + port; }

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
}