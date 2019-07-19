using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;



namespace ZCFrame
{
    
    
    /// <summary>
    /// Http发送数据的回调委托
    /// </summary>
    /// <param name="args"></param>
    public delegate void HttpSendDataCallBack(HttpCallBackArgs args);
    
    /// <summary>
    /// Http访问器
    /// </summary>
    public class HttpRoutine
    {

        #region 属性
        /// <summary>
        /// Http请求回调数据
        /// </summary>
        private HttpCallBackArgs m_CallBackArgs;

        /// <summary>
        /// 是否繁忙
        /// </summary>
        public bool IsBusy { get; private set;}
        #endregion

        public HttpRoutine()
        {
            m_CallBackArgs = new HttpCallBackArgs();
        }

        #region GetUrl Get请求
        /// <summary>
        /// Get请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="callBack"></param>
        public void GetUrl(string url, HttpSendDataCallBack callBack)
        {
            
            if (IsBusy)return;
            IsBusy = true;
            
            UnityWebRequest data = UnityWebRequest.Get(url);
            GameEntry.Http.StartCoroutine(Request(data, callBack));
        }
        #endregion

        
        #region PostUrl Post请求
        /// <summary>
        /// Post请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="callBack"></param>
        /// <param name="dic"></param>
        public void PostUrl(string url, HttpSendDataCallBack callBack, Dictionary<string, string> dic)
        {
            
            if (IsBusy)return;
            IsBusy = true;

            //web加密
            if (dic != null)
            {
                //客户端标识符
                dic["deviceIdentifier"] = DeviceUtil.DeviceIdentifier;
                //设备型号
                dic["deviceModel"] = DeviceUtil.DeviceModel;
            }

            string json = string.Empty;

            if (dic != null)
            {
                //json = JsonMapper.ToJson(dic);
                
                #if DEBUG_LOG_PROTO
                Debug.Log("Http打印通讯协议");
                #endif
                
                GameEntry.Pool.EnqueueClassObject(dic);
            }
            
            //定义一个表单
            WWWForm form = new WWWForm();
            //给表单添加值
           form.AddField("", json);

            UnityWebRequest data = UnityWebRequest.Post(url, form);
            GameEntry.Http.StartCoroutine(Request(data, callBack));
        }
        #endregion

        
        #region Request 请求服务器
        /// <summary>
        /// 请求服务器
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private IEnumerator Request(UnityWebRequest data, HttpSendDataCallBack callBack)
        {
            yield return data.SendWebRequest();

            IsBusy = false;
            if (data.isHttpError || data.isNetworkError)
            {
                m_CallBackArgs.HasError = true;
                m_CallBackArgs.Value = data.error;
            }
            else
            {
                m_CallBackArgs.HasError = false;
                m_CallBackArgs.Value = data.downloadHandler.text;
            }
            
            if (callBack != null)
            {
                callBack(m_CallBackArgs);
            }
            
            data.Dispose();
            data = null;

            //支持多个HttpRoutine，结束之后回池
            GameEntry.Pool.EnqueueClassObject(this);
        }
        
        #endregion
    }

}


