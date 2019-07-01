using System;
using System.Collections.Generic;

//using XLua;

namespace ZCFrame
{
    
    
    /// <summary>
    /// 通用事件
    /// </summary>
    public class CommonEvent : IDisposable
    {
        
        
        //[CSharpCallLua]
        public delegate void OnActionHandler(Object param);
        public Dictionary<ushort, List<OnActionHandler>> dic = new Dictionary<ushort, List<OnActionHandler>>();
        
        
         #region AddEventListener 添加监听
        /// <summary>
        /// 添加监听
        /// </summary>
        /// <param name="key"></param>
        /// <param name="handler"></param>
        public void AddEventListener(ushort key, OnActionHandler handler)
        {
            List<OnActionHandler> lstHandler = null;
            if (!dic.TryGetValue(key, out lstHandler))
            {
                lstHandler = new List<OnActionHandler>();
                dic[key] = lstHandler;
            }

            lstHandler.Add(handler);
        }
        
        #endregion

        #region RemoveEventListener 移除监听
        /// <summary>
        /// 移除监听
        /// </summary>
        /// <param name="key"></param>
        /// <param name="handler"></param>
        public void RemoveEventListener(ushort key, OnActionHandler handler)
        {
            List<OnActionHandler> lstHandler = null;
            
            if (dic.TryGetValue(key, out lstHandler))
            {
                lstHandler.Remove(handler);
                if (lstHandler.Count == 0) dic.Remove(key);
            }
        }
        #endregion

        #region Dispatch 派发
        /// <summary>
        /// 派发
        /// </summary>
        /// <param name="key"></param>
        /// <param name="p"></param>
        public void Dispatch(ushort key, Object param)
        {
            List<OnActionHandler> lstHandler = null;
            
            if (dic.TryGetValue(key, out lstHandler))
            {
                int listCount = lstHandler.Count;
                
                for (int i = 0; i < listCount; i++)
                {
                    OnActionHandler handler = lstHandler[i];
                  
                    if (handler != null) handler(param);
                }
            }
        }

        public void Dispatch(ushort key)
        {
            Dispatch(key, null);
        }

        public void Dispose()
        {
            dic.Clear();
        }
        #endregion
    }

}



