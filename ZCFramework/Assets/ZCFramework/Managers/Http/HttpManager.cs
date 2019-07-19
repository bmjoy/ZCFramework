using System.Collections.Generic;

namespace ZCFrame
{
    
    /// <summary>
    /// Http管理器
    /// </summary>
    public class HttpManager : ManagerBase
    {
        
        /// <summary>
        /// Get请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="callBack"></param>
        public void GetUrl(string url, HttpSendDataCallBack callBack)
        {
            //支持多个HttpRoutine，从对象池中取出HttpRoutine
            HttpRoutine http = GameEntry.Pool.DequeueClassObject<HttpRoutine>();
            http.GetUrl(url, callBack);
        }
        
        /// <summary>
        /// Post请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="callBack"></param>
        /// <param name="dic"></param>
        public void PostUrl(string url, HttpSendDataCallBack callBack, Dictionary<string, string> dic)
        {
            //支持多个HttpRoutine，从对象池中取出HttpRoutine
            HttpRoutine http = GameEntry.Pool.DequeueClassObject<HttpRoutine>();
            http.PostUrl(url, callBack, dic);
        }
    }

}


