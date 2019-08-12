
namespace ZCFrame
{
    
    /// <summary>
    /// Socket组件
    /// </summary>
    public class SocketComponent : ZCBaseComponent, IUpdateComponent
    {

        private SocketManager m_SocketManager = null;


        protected override void OnAwake()
        {
            base.OnAwake();
            m_SocketManager = new SocketManager();
            GameEntry.RegisterUpdateComponent(this);
        }


        public void OnUpdate()
        {
            m_SocketManager.OnUpdate();
        }


        public void CreateSocket(SocketConnectTypes connectType, string serverAddr)
        {
            m_SocketManager.CreateSocket(connectType, serverAddr);
        }


        public void CreateSocket(SocketConnectTypes connectType, string url, int port)
        {
            m_SocketManager.CreateSocket(connectType, url, port);
        }


        /************************************************消息监听**************************************************/
        /// <summary>
        /// 添加一个返回成功的消息的监听
        /// </summary>
        public void AddMessageListener(short opcode, MessageListener ml)
        {
            m_SocketManager.AddMessageListener(opcode, ml);
        }

        /// <summary>
        /// 在已有的返回成功的监听上追加一个监听
        /// </summary>
        public void AdditionalMessageListener(short opcode, MessageListener ml)
        {
            m_SocketManager.AdditionalMessageListener(opcode, ml);
        }

        /// <summary>
        /// 删除一个返回成功的消息的监听
        /// </summary>
        public void RemoveMessageListener(short opcode)
        {
            m_SocketManager.RemoveMessageListener(opcode);
        }

        /// <summary>
        /// 清空返回成功的消息的监听
        /// </summary>
        public void ClearMessageListener()
        {
            m_SocketManager.ClearMessageListener();
        }


        /// <summary>
        /// 添加一个返回错误的消息的监听
        /// </summary>
        public void AddErrorListener(short opcode, MessageListener ml)
        {
            m_SocketManager.AddErrorListener(opcode, ml);
        }

        /// <summary>
        /// 删除一个返回错误的消息的监听
        /// </summary>
        public void RemoveErrorListener(short opcode)
        {
            m_SocketManager.RemoveErrorListener(opcode);
        }

        /// <summary>
        /// 清空返回错误消息的的监听
        /// </summary>
        public void ClearErrorListener()
        {
            m_SocketManager.ClearErrorListener();
        }


        /// <summary>
        /// 添加一个永久的返回成功的消息的监听
        /// </summary>
        public void AddMessageListenerForever(short opcode, MessageListener ml)
        {
            m_SocketManager.AddMessageListenerForever(opcode, ml);
        }


        void OnApplicationPause(bool isPause)
        {
            m_SocketManager.OnApplicationPause(isPause);
        }


        public override void Shutdown()
        {
            m_SocketManager.Dispose();
        }

       
    }
}


