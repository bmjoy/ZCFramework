using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZCFrame;

public class TestResource : MonoBehaviour
{
    
    void Start()
    {

      
        //预加载资源包
        Queue<string> queue = new Queue<string>();
        queue.Enqueue("ghostwill");
        GameEntry.Resource.LoadQueue(queue, (index) =>
        {

        });


    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            //同步加载  需要预加载资源
            GameObject go = GameEntry.Resource.LoadAsset<GameObject>("GhostWill/", "ghostwill", "GhostWill");
            go = Instantiate(go);
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            //异步
            GameEntry.Resource.LoadAsset<GameObject>("GhostWill/", "ghostwill", "GhostWill", (go) =>
            {
                go = Instantiate(go);
            });
        }
    }


}
