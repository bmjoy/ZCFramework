using UnityEngine;
using System;


namespace ZCFrame
{
    
    /// <summary>
    /// 定时器
    /// </summary>
    public class TimeAction : ManagerBase
    {

        
        /// <summary>
        /// 定时器状态
        /// </summary>
        public TimeActionState TimeState
        {
            get;
            private set;
        }
        
        /// <summary>
        /// 当前运行了多长时间
        /// </summary>
        private float m_CurrRunTime;
        
        /// <summary>
        /// 距离下次执行间隔时间
        /// </summary>
        private float m_executionInterval = 0;

        /// <summary>
        /// 当前运行了多少次
        /// </summary>
        private int m_CurrLoop;

        /// <summary>
        /// 是否在等待延迟
        /// </summary>
        private bool IsDelaying = false;
        
        /// <summary>
        /// 每次间隔秒数
        /// </summary>
        private float m_Interval;
        
        /// <summary>
        /// 循环次数(-1表示无限循环，0表示循环一次)
        /// </summary>
        private int m_Loop;
        
        /// <summary>
        /// 开始运行委托
        /// </summary>
        private Action m_OnStartAction;

        /// <summary>
        /// 运行中的委托
        /// </summary>
        private Action<int> m_OnUpdateAction;

        /// <summary>
        /// 结束运行委托
        /// </summary>
        private Action m_OnCompleteAction;


        
        /// <summary>
        /// 初始化定时器
        /// </summary>
        /// <param name="delayTime">延迟时间</param>
        /// <param name="interval">间隔秒数</param>
        /// <param name="loop">循环次数</param>
        /// <param name="onStartAction">开始回调</param>
        /// <param name="onUpdateAction">运行中回调</param>
        /// <param name="onCompleteAction">结束回调</param>
        /// <returns></returns>
        public TimeAction Init(float delayTime, float interval, int loop, Action onStartAction, Action<int> onUpdateAction, Action onCompleteAction)
        {
            //将距离下次执行时间间隔设置为延迟时间
            m_executionInterval = delayTime;
            m_Interval = interval;
            m_Loop = loop;
            m_OnStartAction = onStartAction;
            m_OnUpdateAction = onUpdateAction;
            m_OnCompleteAction = onCompleteAction;
            m_CurrLoop = 0;
            
            IsDelaying = true;
            TimeState = TimeActionState.Idle;
            return this;
        }
        
        /// <summary>
        /// 定时器启动
        /// </summary>
        public void Run()
        {

            if (TimeState.Equals(TimeActionState.Stop))
            {
                Debug.Log("定时器生命周期已结束，请重新初始化");
                return;
            }
            
            //1.把自己加入时间管理器链表中
            GameEntry.Time.RegisterTimeAction(this);
            //2.设置当前运行的时间
            m_CurrRunTime = Time.time + m_executionInterval;
            //4.将计时器状态设置为运行
            TimeState = TimeActionState.Runing;
        }

        /// <summary>
        /// 定时器暂停
        /// </summary>
        public void Pause()
        {
            
            if (TimeState.Equals(TimeActionState.Stop))
            {
                Debug.Log("定时器生命周期已结束，请重新初始化");
                return;
            }
            
            //获取下次执行时间间隔
            m_executionInterval = m_CurrRunTime - Time.time;
            TimeState = TimeActionState.Pause;
        }

        /// <summary>
        /// 定时器结束
        /// </summary>
        public void Stop()
        {
            
            if (TimeState.Equals(TimeActionState.Stop))
            {
                Debug.Log("定时器生命周期已结束，请重新初始化");
                return;
            }

            m_OnCompleteAction?.Invoke();

            IsDelaying = false;
            TimeState = TimeActionState.Stop;

            m_OnStartAction = null;
            m_OnUpdateAction = null;
            m_OnCompleteAction = null;
            GameEntry.Time.RemoveTimeAction(this);
        }
        
        
        /// <summary>
        /// 定时器每帧更新
        /// </summary>
        public void OnUpdate()
        {
            
            //当定时器不处于运行状态时
            if (!TimeState.Equals(TimeActionState.Runing))return;
            
             if (Time.time > m_CurrRunTime)
             {
                 
                 //先处理延迟，过了延迟时间，第一次开始执行
                 if (IsDelaying)
                 {
                      IsDelaying = false;
                      m_CurrRunTime = Time.time;

                    m_OnStartAction?.Invoke();
                }

                 if (IsDelaying) return;
                     
                 m_CurrRunTime = Time.time + m_Interval;

                m_OnUpdateAction?.Invoke(m_Loop - m_CurrLoop);

                if (m_Loop > -1)
                 {
                     m_CurrLoop++;
                     if (m_CurrLoop >= m_Loop)
                     {
                         Stop();
                     }
                 }
                 
             }
        }
    }
}





