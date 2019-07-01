using System;
using System.Collections.Generic;

namespace ZCFrame
{
    
    /// <summary>
    /// 定时器管理器
    /// </summary>
    public class TimeManager : ManagerBase,IDisposable
    {
        /// <summary>
        /// 定时器链表
        /// </summary>
        private LinkedList<TimeAction> m_TimeActionList;
        
        internal TimeManager()
        {
            m_TimeActionList = new LinkedList<TimeAction>();
        }
        
        
        #region 定时器管理
        /// <summary>
        /// 注册定时器
        /// </summary>
        /// <param name="action"></param>
        internal void RegisterTimeAction(TimeAction action)
        {

            if (!m_TimeActionList.Contains(action)) 
                m_TimeActionList.AddLast(action);
        }
        
        /// <summary>
        /// 移除定时器
        /// </summary>
        /// <param name="action"></param>
        internal void RemoveTimeAction(TimeAction action)
        {
            m_TimeActionList.Remove(action);
           // GameEntry.Pool.EnqueueClassObject(action);
        }
        
        /// <summary>
        /// 创建定时器
        /// </summary>
        /// <returns></returns>
        internal TimeAction CreatTimeAction()
        {
            return new TimeAction();
//            return GameEntry.Pool.DequeueClassObject<TimeAction>();
        }
        #endregion
        
        internal void OnUpdate()
        {
            for (LinkedListNode<TimeAction> curr = m_TimeActionList.First; curr != null; curr = curr.Next)
            {
                curr.Value.OnUpdate();
            }
        }
        
        public void Dispose()
        {
            m_TimeActionList.Clear();
        }
    }

    /// <summary>
    /// 定时器状态
    /// </summary>
    public enum TimeActionState
    {
        
        /// <summary>
        /// 闲置
        /// </summary>
        Idle,
        /// <summary>
        /// 运行
        /// </summary>
        Runing,
        /// <summary>
        /// 暂停
        /// </summary>
        Pause,
        /// <summary>
        /// 结束
        /// </summary>
        Stop,
    }
}


