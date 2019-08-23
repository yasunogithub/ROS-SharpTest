using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;
using WebSocketSharp.Net;

public class TurtleBot_Script : MonoBehaviour {

    [System.Serializable]
    public class RosData1
    {
        public string op;
        public string topic;
        public string type;
    }

    [System.Serializable]
    public class RosData2
    {
        public string op;
        public string topic;
        public Msg msg;
    }

    [System.Serializable]
    public class Msg
    {
        public Vector3 linear;
        public Vector3 angular;
    }


    WebSocket ws;
    public float linear_val = 1.0f;
    public float angular_val = 1.0f;
    public string ROS_bridge_server_url = "ws://172.19.4.225:9090"; //自分のIPアドレス

    void Start()
    {
      
        ws = new WebSocket(ROS_bridge_server_url);

        ws.OnOpen += (sender, e) =>
        {
            Debug.Log("WebSocket Open");
            RosData1 data = new RosData1();
            data.op = "advertise";
            data.topic = "/mobile_base/commands/velocity";
            data.type = "geometry_msgs/Twist";
            string json = JsonUtility.ToJson(data);
            ws.Send(json);
        };
        bool ConnectedFlag = false;

        ws.OnMessage += (sender, e) =>
        {
            if (!ConnectedFlag)
            {
                // おそらくOnMessageイベントでやる処理ではないが、メッセージがきちゃうので弾きついでに。
                ConnectedFlag = true;
                if (e.Data != "{\"type\": \"hello\"}")
                {
                }
                Debug.Log("WebSocket Success Message:");
                return;
            }
        };
            ws.OnError += (sender, e) =>
        {
            Debug.Log("WebSocket Error Message: " + e.Message);
        };

        ws.OnClose += (sender, e) =>
        {
            Debug.Log("WebSocket Close");
        };

        ws.Connect();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 lin = new Vector3(2.0f, 0, 0);
        Vector3 ang = new Vector3(0, 0, 1.8f);

        Msg msg = new Msg();
        msg.linear = lin;
        msg.angular = ang;

        RosData2 data = new RosData2();
        data.op = "publish";

       // rostopic pub -r 1 / turtle1 / cmd_vel geometry_msgs / Twist-- "[2.0, 0.0, 0.0]" "[0.0, 0.0, 1.8]"

        data.topic = "/turtle1/cmd_vel";
        data.msg = msg;
        string json = JsonUtility.ToJson(data);
        ws.Send(json);
    }
}
