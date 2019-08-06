using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZCFrame;

public class TestSocket : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GameEntry.Socket.CreateSocket(SocketConnectTypes.Hall, "127.0.0.1", 88);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
