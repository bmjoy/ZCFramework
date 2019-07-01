using System;



namespace ZCFrame
{
    public class EventManager : ManagerBase, IDisposable
    {

        
        public SocketEvent SocketEvent { private set; get; }
        public CommonEvent CommonEvent { private set; get; }


        internal EventManager()
        {
            SocketEvent = new SocketEvent();
            CommonEvent = new CommonEvent();
        }

        public void Dispose()
        {
            SocketEvent.Dispose();
            CommonEvent.Dispose();
        }

    }

}


