using System;
using System.Net;
using System.Net.Sockets;
using UnityEngine;


namespace ZCFrame
{
    public class NetSocket
    {

        private const int SocketOverTime = 3000;

        /// <summary>
        /// 连接状态 true成功 false失败
        /// </summary>
        public Action<bool> ConnectionState = null;

        /// <summary>
        /// 连续发送消息失败次数
        /// </summary>
        public byte FailuresSendMessagesNum = 0;
        /// <summary>
        /// 是否链接中
        /// </summary>
        public bool IsConnecting = false;

        private readonly IPEndPoint ipEndPoint;
        private readonly AddressFamily ipType;

        private readonly ReceiveCallBack m_receiveCallBack;

        private Socket m_GameSocket = null;

        /// <summary>
        /// 是否正在发送消息 用来保证正在发送消息是不会被关闭
        /// </summary>
        private bool m_IsSendingMessage = false;
        /// <summary>
        /// 标记为需要关闭的 用来在发送完最后一条消息后自动关闭连接
        /// </summary>
        private bool m_MarkedClosed = false;
        /// <summary>
        /// 是否连接成功
        /// </summary>
        private bool IsConnected
        {
            get
            {
                return m_GameSocket != null && m_GameSocket.Connected;
            }
        }


        public NetSocket(string url, int port, ReceiveCallBack callBack)
        {
            m_receiveCallBack = callBack;

            ipType = AddressFamily.InterNetwork;

#if UNITY_IPHONE
             IPAddress ipAdress;
			string serverIp = NativeConnect.GetIpType(url, out ipType);
			ipAdress = IPAddress.Parse(serverIp);
#else
            //测试是否是一个有效的ip地址
            if (IPAddress.TryParse(url, out IPAddress ipAdress))
            {
                ipAdress = IPAddress.Parse(url);
            }
            else
            {
                //根据地址获取ip
                IPHostEntry ipInfo = Dns.GetHostEntry(url);

                ipAdress = ipInfo.AddressList[0];
            }
#endif

            ipEndPoint = new IPEndPoint(ipAdress, port);
        }


        public void ConnectNet()
        {
            try
            {
                if (IsConnecting) return;
                IsConnecting = true;

                if (m_GameSocket != null)
                {
                    m_GameSocket.Close();
                    m_GameSocket = null;
                }

                m_GameSocket = new Socket(ipType, SocketType.Stream, ProtocolType.Tcp);

                IAsyncResult result = m_GameSocket.BeginConnect(ipEndPoint, new AsyncCallback(Callback_ConnectSuccess), null);
                bool success = result.AsyncWaitHandle.WaitOne(SocketOverTime, true);

                if (!success)
                {
                    ConnectionState?.Invoke(false);
                    Closed();

                    IsConnecting = false;
                }
            }
            catch (Exception e)
            {
                ConnectionState?.Invoke(false);
                Closed();

                IsConnecting = false;
                Debug.Log(e);
            }
        }


        /// <summary>
        /// 连接成功
        /// </summary>
        private void Callback_ConnectSuccess(IAsyncResult ar)
        {
            IsConnecting = false;

            try
            {
                if (IsConnected)
                {
                    if (ar.IsCompleted) m_GameSocket.EndConnect(ar);

                    FailuresSendMessagesNum = 0;
                    m_IsSendingMessage = false;
                    m_MarkedClosed = false;

                    ConnectionState?.Invoke(true);

                    BeginRead();
                }
            }
            catch (Exception e)
            {
                ConnectionState?.Invoke(false);
                Debug.Log(e);
            }
        }


        private void BeginRead()
        {
            if (IsConnected)
            {
                byte[] readBuf = new byte[4];
                //开始接收
                m_GameSocket.BeginReceive(readBuf, 0, readBuf.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), readBuf);
            }
        }


        private void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                if (!IsConnected) return;

                int rl = m_GameSocket.EndReceive(ar);
                byte[] lengthBuf = (byte[])ar.AsyncState;

                //string msg = System.Text.Encoding.UTF8.GetString(lengthBuf, 0, lengthBuf.Length);
                //UnityEngine.Debug.Log("消息:" + msg);

                if (lengthBuf.Length == rl)
                {
                    NetPacket packet = ProNetTool.ReadPacket(lengthBuf, null, m_GameSocket);
                    m_receiveCallBack?.Invoke(packet);

                    BeginRead();
                }
            }
            catch (Exception e)
            {
                Closed();
                Debug.Log("读取消息异常  " + e);
            }
        }


        public void SendMessage(byte[] message)
        {
            try
            {
                if (IsConnected)
                {
                    m_IsSendingMessage = true;

                    IAsyncResult asyncSend = m_GameSocket.BeginSend(message, 0, message.Length, SocketFlags.None, new AsyncCallback(Callback_SendSuccess), null);
                    bool success = asyncSend.AsyncWaitHandle.WaitOne(SocketOverTime, true);
                    if (!success)
                    {
                        FailuresSendMessagesNum++;
                        m_IsSendingMessage = false;
                    }
                }
            }
            catch (Exception e)
            {
                FailuresSendMessagesNum++;
                Closed();
                Debug.Log(e);
            }
        }


        private void Callback_SendSuccess(IAsyncResult ar)
        {
            byte fsmn = FailuresSendMessagesNum;

            try
            {
                if (IsConnected) m_GameSocket.EndSend(ar);

                m_IsSendingMessage = false;
                FailuresSendMessagesNum = 0;

                if (m_MarkedClosed) Closed();
            }
            catch (Exception e)
            {
                FailuresSendMessagesNum = fsmn;
                Debug.Log("发送消息过程中异常   " + m_IsSendingMessage + "        " + e);
            }
        }


        public void Closed()
        {

            m_MarkedClosed = true;

            if (m_GameSocket != null && !m_IsSendingMessage)
            {
                m_GameSocket.Close();
                m_GameSocket = null;
            }
        }
    }
}



