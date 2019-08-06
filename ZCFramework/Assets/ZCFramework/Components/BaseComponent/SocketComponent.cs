
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


