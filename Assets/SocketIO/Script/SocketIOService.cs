using System.Collections.Generic;
using System.Threading.Tasks;
using SocketIO;
using UnityEngine;

public class SocketIOService : MonoBehaviour {

    private static SocketIOService _instance = null;

    public static SocketIOService Instance { get => _instance; }

    [SerializeField]
    private List<SocketIOList> list = new List<SocketIOList> ();

    private Dictionary<string, SocketIOHelper> _list = new Dictionary<string, SocketIOHelper> ();

    private void Awake () {
        if (_instance == null) {
            _instance = this;
            DontDestroyOnLoad (gameObject);
            initialize ();
        } else {
            Destroy (gameObject);
        }
    }

    private void Start () { }

    public void initialize () {
        foreach (SocketIOList item in list) {
            if (item.name != "" && item.socket.info.url != "") {
                _list.Add (item.name, item.socket);
                item.socket.initialize ();
            }
        }
    }

    public static SocketIOHelper getSocketHelper (string name) {
        if (_instance != null) {
            SocketIOHelper result = null;
            _instance._list.TryGetValue (name, out result);
            return result;
        } else {
            return null;
        }
    }

    // Update is called once per frame
    void Update () {

    }
}