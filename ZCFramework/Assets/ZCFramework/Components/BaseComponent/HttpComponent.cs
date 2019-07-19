

using System.Collections.Generic;
using UnityEngine;

namespace ZCFrame
{
    /// <summary>
    /// Http组件
    /// </summary>
    public class HttpComponent : ZCBaseComponent
    {

        [SerializeField, Header("正式账号服务器Url")]
        private string m_WebAccountUrl = "";
       
        [SerializeField,Header("测试账号服务器Url")]
        private string m_TestWebAccountUrl = "";
      
        [SerializeField,Header("是否测试环境")]
        private bool m_IsTest = false;
        
        /// <summary>
        /// 当前真实的账号服务器Url
        /// </summary>
        public string RealWebUrl
        {
            get { return m_IsTest ? m_TestWebAccountUrl : m_WebAccountUrl; }
        }
        
        private HttpManager m_HttpManager;

        
        protected override void OnAwake()
        {
            base.OnAwake();
            m_HttpManager = new HttpManager();
        }

        
        /// <summary>
        /// Get请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="callBack"></param>
        public void GetUrl(string url, HttpSendDataCallBack callBack)
        {
            m_HttpManager.GetUrl(url, callBack);
        }
        
        /// <summary>
        /// Post请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="callBack"></param>
        /// <param name="dic"></param>
        public void PostUrl(string url, HttpSendDataCallBack callBack, Dictionary<string, string> dic)
        {
            m_HttpManager.PostUrl(url, callBack, dic);
        }
        
        
        public override void Shutdown()
        {
            
        }
       
    }

}


