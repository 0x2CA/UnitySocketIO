using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using SocketIO;
using UnityEngine;

public class Test : MonoBehaviour {
    // Start is called before the first frame update
    SocketIOHelper Test1;
    void Start () {
        Test1 = SocketIOService.getSocketHelper ("Test1");
        if (Test1 != null) {
            Debug.Log ("存在:" + Test1.info.url + "  状态:" + Test1.getStatus ());
            Test1.connectAsync ();

            Debug.Log ("已经连接" + "  状态:" + Test1.getStatus ());
            Test1.On ("test", (result, callback) => {
                Debug.Log ("收到:" +
                    result);
                callback ("你好");
            });
        }
    }

    private void OnGUI () {
        if (GUI.Button (new Rect (0, 0, 200, 100), "Socket send 测试!")) {
            if (Test1 != null) {
                Test1.EmitAsync ("test", "12345", (result, callback) => {
                    Debug.Log ("运程调用:" + result);
                });
            }
        }
    }

    // private void Start () {
    // IO a = new IO ("https://xcx.test.tongchuanggame.com:2020");
    // a.connect ();

    // a.on ("test", (result, callback) => {
    //     callback("你好");
    //     Debug.Log (result);
    // });

    // a.on (IOEvent.CONNECT, () => {
    //     a.emit ("test", "123456", (result, callback) => {
    //         Debug.Log ("callback" + result);
    //     });

    // });

    // }

    // Update is called once per frame
    void Update () {

    }
}

[Serializable]
class SocketResult {
    public int code = 200;

    public string msg = "";
    public string[] data = new string[] { };
}