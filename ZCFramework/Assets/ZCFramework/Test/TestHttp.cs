using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
