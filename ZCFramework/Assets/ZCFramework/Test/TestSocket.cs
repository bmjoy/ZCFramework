using UnityEngine;
using ZCFrame;

public class TestSocket : MonoBehaviour
{

    

    void Start()
    {
        //    //通信返回监听
        //    GameEntry.Socket.AddMessageListener(MesOpcode.GET_SERVER_TIME, NetCallBack_GET_SERVER_TIMET);
        //    GameEntry.Socket.CreateSocket(SocketConnectTypes.Hall, "127.0.0.1", 88);

    }

    //private void NetCallBack_GET_SERVER_TIMET(NetPacket packet)
    //{
    //    Protobuf.Person person = packet.GetBody<Protobuf.Person>();
    //    Debug.Log(person.Age + "  " + person.Name + "  " + person.NameList[0]);
    //}


    void Update()
    {

        if (Input.GetKeyDown(KeyCode.S))
        {
            GameEntry.Socket.ConnectToMainSocket("192.168.0.106", 1039);
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            //Task_SearchTaskProto proto = new Task_SearchTaskProto();
            //GameEntry.Socket.SendMsg(proto);
        }

    }
}
