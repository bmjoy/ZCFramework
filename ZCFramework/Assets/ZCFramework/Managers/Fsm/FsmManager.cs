using System;
using System.Collections.Generic;


namespace ZCFrame
{
    
    /// <summary>
    /// 状态机管理器
    /// </summary>
    public class FsmManager : ManagerBase, IDisposable
    {
        private Dictionary<int, FsmBase> m_FsmDic;

        public FsmManager()
        {
            m_FsmDic = new Dictionary<int, FsmBase>();
        }


        /// <summary>
        /// 创建状态机
        /// </summary>
        /// <param name="fsmId">状态机编号</param>
        /// <param name="owner">拥有者</param>
        /// <param name="states">状态机数组</param>
        /// <typeparam name="T">拥有者类型</typeparam>
        /// <returns></returns>
        public Fsm<T> Create<T>(int fsmId, T owner,params FsmState<T>[] states) where T : class
        {
            Fsm<T> fsm = new Fsm<T>(fsmId, owner, states);
            m_FsmDic[fsmId] = fsm;
            return fsm;
        }

        /// <summary>
        /// 摧毁状态机
        /// </summary>
        /// <param name="fsmId"></param>
        public void DestroyFsm(int fsmId)
        {
            FsmBase fsm;
            if (m_FsmDic.TryGetValue(fsmId, out fsm))
            {
                fsm.ShutDown();
                m_FsmDic.Remove(fsmId);
            }
        }
        
        public void Dispose()
        {
            var enumerator = m_FsmDic.GetEnumerator();
            while (enumerator.MoveNext())
            {
                enumerator.Current.Value.ShutDown();
            }
            m_FsmDic.Clear();
        }

    }

}


