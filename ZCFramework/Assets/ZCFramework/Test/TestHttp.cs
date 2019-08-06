using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ZCFrame;

public class TestHttp : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
//        string url = GameEntry.Http.RealWebUrl;
//        GameEntry.Http.GetUrl(url, WebGetCallBack);

//        Dictionary<string, object> dic = GameEntry.Pool.DequeueClassObject<Dictionary<string,object>>();
//        dic["ChannelId"] = 0;
//        dic["InnerVersion"] = 1001;
//        GameEntry.Http.SendData(url, WebPostCallBack, true, dic);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        { 
            Dictionary<string, string> dic = GameEntry.Pool.DequeueClassObject<Dictionary<string,string>>();
//            dic["ChannelId"] = 0;
//            dic["InnerVersion"] = 1001;
            GameEntry.Http.PostUrl("http://www.my-server.com/myform", WebPostCallBack, dic);
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            string url = "https://image.baidu.com/search/detail?z=0&word=%E6%91%84%E5%BD%B1%E5%B8%88%E7%89%9F%E6%B5%B7%E6%B3%A2&hs=0&pn=3&spn=0&di=0&pi=63849592746&tn=baiduimagedetail&is=0%2C0&ie=utf-8&oe=utf-8&cs=1377074305%2C2777267173&os=&simid=&adpicid=0&lpn=0&fm=&sme=&cg=&bdtype=-1&oriquery=&objurl=http%3A%2F%2Fc.hiphotos.baidu.com%2Fimage%2Fpic%2Fitem%2Fd1a20cf431adcbefdc7ef0eba2af2edda2cc9f91.jpg&fromurl=&gsm=0&catename=pcindexhot&islist=&querylist=";
                  GameEntry.Http.GetUrl(url, WebGetCallBack);
        }

    }
    
    
    private void WebGetCallBack(HttpCallBackArgs args)
    {
        Debug.Log("haserror = " + args.HasError);
     
        Debug.Log("value = " + args.Value);

    }

    private void WebPostCallBack(HttpCallBackArgs args)
    {
        Debug.Log("Post   "+args.Value);
        GameEntry.Procedure.ChangeState(ProcedureState.CheckVersion);
    }
}
