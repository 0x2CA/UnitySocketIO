using System.Collections.Generic;
using System.Threading.Tasks;
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
        } else {
            Destroy (gameObject);
        }
    }

    private void Start () {
        initialize ();
        SocketIOHelper Test1 = getSocketHelper ("Test1");
        if (Test1 != null) {
            Debug.Log ("存在:" + Test1.info.url + "  状态:" + Test1.getStatus ());
            Test1.connectAsync ().Wait ();

            Debug.Log ("已经连接" + "  状态:" + Test1.getStatus ());
            Test1.On ("test", delegate (SocketIOClient.Arguments.ResponseArgs args) {
                Debug.Log (args.Text);
            });
            Test1.EmitAsync ("test", Test1);
        }
    }

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