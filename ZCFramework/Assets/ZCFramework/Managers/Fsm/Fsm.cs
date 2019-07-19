using System.Collections.Generic;



namespace ZCFrame
{
    
    public class Fsm<T> : FsmBase where T : class
    {

        /// <summary>
        /// 当前状态
        /// </summary>
        private FsmState<T> m_CurrState;

        /// <summary>
        /// 状态字典
        /// </summary>
        private readonly Dictionary<byte, FsmState<T>> m_StateDic;

        /// <summary>
        /// 参数字典
        /// </summary>
        private readonly Dictionary<string, VariableBase> m_ParamDic;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="fsmId">状态机编号</param>
        /// <param name="owner">拥有者</param>
        /// <param name="states">状态数组</param>
        public Fsm(int fsmId, T owner,params FsmState<T>[] states) : base(fsmId, owner.GetType())
        {
            m_StateDic = new Dictionary<byte, FsmState<T>>();
            m_ParamDic = new Dictionary<string, VariableBase>();
            
            //把状态加入字典
            int len = states.Length;
            for (int i = 0; i < len; i++)
            {
                FsmState<T> state = states[i];
                state.CurrFsm = this;
                m_StateDic[(byte)i] = state;
            }
        }

        /// <summary>
        /// 设置默认状态
        /// </summary>
        public void SetDefaultState(byte defaultState = 0)
        {
            //设置默认状态
            CurrStateType = defaultState;
            m_CurrState = m_StateDic[CurrStateType];
            m_CurrState.OnEnable();
        }


        /// <summary>
        /// 设置参数字典
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <typeparam name="TData"></typeparam>
        public void SetData<TData>(string key, TData value) where TData : VariableBase
        {

            Variable<TData> item;

            if (!m_ParamDic.ContainsKey(key))
            {
                item = new Variable<TData>();
                m_ParamDic.Add(key, item);
            }

            item = m_ParamDic[key] as Variable<TData>;
            item.Value = value;
            m_ParamDic[key] = item;
        }


        /// <summary>
        /// 获取参数字典里面的值
        /// </summary>
        /// <param name="key"></param>
        /// <typeparam name="TData"></typeparam>
        /// <returns></returns>
        public TData GetData<TData>(string key)  where TData : VariableBase
        {
           
            if (m_ParamDic.TryGetValue(key, out VariableBase itemBase))
            {
                Variable<TData> item = itemBase as Variable<TData>;
                return item.Value;
            }
            
            return default;
        }


        /// <summary>
        /// 获取某个状态
        /// </summary>
        /// <param name="stateType"></param>
        /// <returns></returns>
        public FsmState<T> GetState(byte stateType)
        {
            m_StateDic.TryGetValue(stateType, out FsmState<T> state);
            return state;
        }

        /// <summary>
        /// 执行当前状态的Update
        /// </summary>
        public void OnUpdate()
        {
            if (m_CurrState != null)
            {
                m_CurrState.OnUpdate();
            }
        }

        /// <summary>
        /// 切换状态
        /// </summary>
        /// <param name="newState"></param>
        public void ChangeState(byte newState)
        {
            //如果新状态就是当前状态，不切换
            if (CurrStateType.Equals(newState)) return;

            if (m_CurrState != null)
            m_CurrState.OnLeave();

            CurrStateType = newState;
            m_CurrState = m_StateDic[CurrStateType];

            if (m_CurrState != null)
            m_CurrState.OnEnable();
            
        }
        
        /// <summary>
        /// 关闭状态机
        /// </summary>
        public override void ShutDown()
        {
            if (m_CurrState != null)
            {
                m_CurrState.OnLeave();
            }
            
            foreach (var state in m_StateDic)
            {
                state.Value.OnDestroy();
            }
            
            m_StateDic.Clear();
            m_ParamDic.Clear();
        }
    } 

}


