using System;
using System.Collections.Generic;
using UnityEngine;


namespace ZCFrame
{
    /// <summary>
    /// 消息监听委托
    /// </summary>
    /// <param name="packet"></param>
    public delegate void MessageListener(NetPacket packet);

    public enum SocketConnectTypes : short
    {
        NoConnect = 0,
        Hall,
        Game
    }


    /// <summary>
    /// Socket管理器
    /// </summary>
    public class SocketManager : ManagerBase, IDisposable
    {

        #region 连接数据
        /// <summary>
        /// 心跳位置
        /// </summary>
        public SocketConnectTypes heartbeatLocation = 0;
        /// <summary>
        /// 心跳间隔
        /// </summary>
        private float heartbeatIntervalTime = 20;
        /// <summary>
        /// 上一次心跳时间
        /// </summary>
        private float lastHeartbeatTime = 0;
        /// <summary>
        /// 连接列表
        /// </summary>
        private readonly Dictionary<SocketConnectTypes, NetSocket> connectDictionary;
        /// <summary>
		/// 消息监听委托列表
		/// </summary>
		private readonly Dictionary<short, MessageListener> mlDictionary;
        /// <summary>
        /// 消息监听委托列表_永久有效
        /// </summary>
        private readonly Dictionary<short, MessageListener> mlDictionaryForever;
        /// <summary>
        /// 消息监听委托列表_错误消息
        /// </summary>
        private readonly Dictionary<short, MessageListener> mlDictionaryError;
        /// <summary>
        /// 上一次发送消息的时间列表
        /// </summary>
        private readonly Dictionary<short, float> lastSendMessageTime;
        /// <summary>
        /// 返回消息队列 主动分发的
        /// </summary>
        private readonly Queue<NetPacket> receiveMessageQueueActive;
        /// <summary>
        /// 返回消息队列 临时的
        /// </summary>
        private readonly Queue<NetPacket> receiveMessageQueueTemporary;

        public readonly object _syncObject = new object();
        #endregion


        /// <summary>
        /// 成功返回标志
        /// </summary>
        public const short RESULT_SUCCESS = 0;
        /// <summary>
        /// 是否暂停中
        /// </summary>
        private bool m_IsPause = false;
        /// <summary>
        /// 上一次重连时间
        /// </summary>
        private float LastReconnectTime = 0;
        /// <summary>
        /// 重连次数
        /// </summary>
        private int reconnectNum = 0;
        /// <summary>
        /// 最大重连次数
        /// </summary>
        private const int maxReconnectNum = 3;



        public SocketManager()
        {
            connectDictionary = new Dictionary<SocketConnectTypes, NetSocket>();
            mlDictionary = new Dictionary<short, MessageListener>();
            mlDictionaryForever = new Dictionary<short, MessageListener>();
            mlDictionaryError = new Dictionary<short, MessageListener>();
            lastSendMessageTime = new Dictionary<short, float>();
            receiveMessageQueueActive = new Queue<NetPacket>();
            receiveMessageQueueTemporary = new Queue<NetPacket>();
        }


        public void OnUpdate()
        {
            if (heartbeatLocation > SocketConnectTypes.NoConnect)
            {
                //发送心跳
                if ((Time.realtimeSinceStartup - lastHeartbeatTime) >= heartbeatIntervalTime)
                {
                    NetPacket packet = new NetPacket
                    {
                        Opcode = MesOpcode.HEARTBEAT
                    };

                    NetSocket ns = connectDictionary[heartbeatLocation];
                    ns.SendMessage(packet.Encoder());

                    //更新心跳时间
                    lastHeartbeatTime = Time.realtimeSinceStartup;
                }
            }

            //分发服务器返回消息
            if (receiveMessageQueueActive.Count > 0)
            {
                lock (_syncObject)
                {
                    DistributeMessage(receiveMessageQueueActive);
                }
            }

            if (heartbeatLocation > SocketConnectTypes.NoConnect)
            {
                if (connectDictionary[heartbeatLocation].FailuresSendMessagesNum > 3)
                {
                    if ((Time.realtimeSinceStartup - LastReconnectTime) > 3 && reconnectNum <= maxReconnectNum)
                    {
                        LastReconnectTime = Time.realtimeSinceStartup;
                        Reconnect();
                    }
                }
            }
        }


        private void DistributeMessage(Queue<NetPacket> queue)
        {
            while (queue != null && queue.Count > 0)
            {
                NetPacket packet = queue.Dequeue();
                packet.LogPacket("分发");

                if (lastSendMessageTime.ContainsKey(packet.Opcode))
                {
                    lastSendMessageTime.Remove(packet.Opcode);
                }
                else
                {
                    //服务器弹窗, 更新玩家信息，获得道具
                    //if (packet.Opcode == MesOpcode.SERVER_POPUP_WINDOW || packet.Opcode == MesOpcode.UPDATA_PLAYER_DATA || packet.Opcode == MesOpcode.GET_AWARD)
                    //{
                    //收到返回清除上一次访问时间
                    if (lastSendMessageTime.ContainsKey(packet.Param))
                    {
                        lastSendMessageTime.Remove(packet.Param);
                    }
                    //}
                }

                //心跳
                if (packet.Opcode == MesOpcode.HEARTBEAT)
                {
                    //判断是否要对表
                    //if (GameClock.TIME_DIFFERENCE > 20000)
                    //{
                    //    if (Time.realtimeSinceStartup - netData.lastHeartbeatTime < 0.01f)
                    //    {
                    //        GameClock.Clock.setClock();
                    //    }
                    //}
                }
                //弹窗消息
                //else if (packet.Opcode == MesOpcode.SERVER_POPUP_WINDOW)
                //{
                //    Notify notify = packet.getBody<Notify>();

                //    NotifyManage.Manage.addNotify(notify);
                //}
                ////如果是错误消息
                //else if (packet.Result != MesOpcode.MESSAGE_RIGHT_CODE)
                //{
                //    if (netData.mlDictionaryError.ContainsKey(packet.Opcode))
                //    {
                //        netData.mlDictionaryError[packet.Opcode](packet);
                //    }
                //}
                //如果有监听就分发
                else if (mlDictionary.ContainsKey(packet.Opcode))
                {
                    mlDictionary[packet.Opcode](packet);
                }
                //长效监听
                else if (mlDictionaryForever.ContainsKey(packet.Opcode))
                {
                    mlDictionaryForever[packet.Opcode](packet);
                }
                //没有监听的消息
                else
                {
                    packet.LogPacket("忽略消息");
                }
            }
        }

        public void OnApplicationPause(bool isPause)
        {
            this.m_IsPause = isPause;

            if (m_IsPause)
            {
               receiveMessageQueueTemporary.Clear();
            }
            else
            {
                lock (_syncObject)
                {
                    //处理临时消息队列
                    DistributeMessage(receiveMessageQueueTemporary);
                }
            }
        }


        /// <summary>
        /// 重连
        /// </summary>
        private void Reconnect()
        {
            if (!connectDictionary[heartbeatLocation].IsConnecting)
            {
                connectDictionary[heartbeatLocation].ConnectionState = delegate (bool obj)
                {
                    if (obj)
                    {
                        connectDictionary[heartbeatLocation].ConnectionState = null;
                        LastReconnectTime = 0;
                        reconnectNum = 0;

                        NetPacket packet = new NetPacket
                        {
                            Opcode = MesOpcode.NET_RECONNECT
                        };
                        // packet.Key = PlayerDataManager.Manage.Main.id;

                        SendPacket(heartbeatLocation, packet, true);
                    }
                    else
                    {
                        reconnectNum++;
                    }
                };

                if (reconnectNum >= maxReconnectNum)
                {
                    //NotifyManage.Manage.notifyTips(66, null, null);
                }
                else if (reconnectNum >= maxReconnectNum - 2)
                {
                    //NotifyManage.Manage.notifyTips(146, null, null);
                }

                connectDictionary[heartbeatLocation].ConnectNet();
            }
        }



        /************************************************管理工具**************************************************/
        /// <summary>
        /// 重置
        /// </summary>
        public void Reset()
        {
            ClosedAllSocket();
            ClearMessageListener();
            ClearErrorListener();
            ClearLastSendMessageTime();
            ClearReceiveMessageList();

            reconnectNum = 0;

            heartbeatLocation = SocketConnectTypes.NoConnect;
        }

        /// <summary>
        /// 清理上一次发送消息时间
        /// </summary>
        public void ClearLastSendMessageTime()
        {
            lastSendMessageTime.Clear();
        }


        /************************************************心跳管理**************************************************/
        /// <summary>
        /// 心跳位置
        /// </summary>
        public SocketConnectTypes HeartbeatLocation
        {
            get { return heartbeatLocation; }
            set { heartbeatLocation = value; }
        }

        /// <summary>
        /// 心跳间隔时间
        /// </summary>
        public float HeartbeatIntervalTime
        {
            get { return heartbeatIntervalTime; }
            set { heartbeatIntervalTime = value; }
        }



        /************************************************Socket连接**************************************************/
        public void CreateSocket(SocketConnectTypes connectType, string serverAddr)
        {
            string[] addr = serverAddr.Split(':');
            CreateSocket(connectType, addr[0], int.Parse(addr[1]));
        }

        public void CreateSocket(SocketConnectTypes connectType, string url, int port)
        {
            if (connectDictionary.ContainsKey(connectType))
             ClosedSocket(connectType);

            NetSocket ns = new NetSocket(url, port, ReceiveMessage);
            ns.ConnectNet();

            connectDictionary.Add(connectType, ns);
        }


        public void ClosedSocket(SocketConnectTypes connectType)
        {
            if (connectDictionary.TryGetValue(connectType, out NetSocket nc))
            {
                nc.Closed();
                connectDictionary.Remove(connectType);
            }

            if (connectType.Equals(heartbeatLocation))
            {
                heartbeatLocation = (int)SocketConnectTypes.NoConnect;
            }
        }

        public void ClosedAllSocket()
        {

            var enumerator = connectDictionary.GetEnumerator();

            while (enumerator.MoveNext())
            {
                enumerator.Current.Value.Closed();
            }

            connectDictionary.Clear();
            heartbeatLocation = (int)SocketConnectTypes.NoConnect;
        }



        public void Send<T>(SocketConnectTypes connectType, short opcode, T obj, bool allowFrequent = false, string logHead = null) where T : Google.Protobuf.IMessage<T>
        {
            NetPacket packet = new NetPacket
            {
                Opcode = opcode
            };
            packet.PutBody<T>(obj);

            SendPacket(connectType, packet, allowFrequent, logHead);
        }


        public void Send(SocketConnectTypes connectType, short opcode, bool allowFrequent = false)
        {
            NetPacket packet = new NetPacket
            {
                Opcode = opcode
            };

            SendPacket(connectType, packet, allowFrequent);
        }

        public void SendPacket(SocketConnectTypes connectType, NetPacket packet, bool allowFrequent = false, string logHead = null)
        {
            if (connectDictionary.ContainsKey(connectType))
            {
                //是否有锁
                bool haveLastTime = lastSendMessageTime.ContainsKey(packet.Opcode);

                if (haveLastTime)
                {
                    //有锁时 如果两次发送间隔大于n秒也可以发送
                    haveLastTime = Time.time - lastSendMessageTime[packet.Opcode] < 3;
                }

                if (allowFrequent ? true : !haveLastTime)
                {
                    packet.LogPacket("发送");
                    connectDictionary[connectType].SendMessage(packet.Encoder(logHead));
                }

                if (!allowFrequent && !haveLastTime)
                {
                    lastSendMessageTime.Add(packet.Opcode, Time.time);
                }
            }
            else
            {
                Debug.Log("----------未找到 " + connectType + "\t" + packet.Opcode + "连接消息发送失败----------");
            }
        }


        /************************************************消息接收**************************************************/
        private void ReceiveMessage(NetPacket packet)
        {
            lock (_syncObject)
            {
                //if (isPause && (packet.Opcode == MesOpcode.GET_AWARD || packet.Opcode == MesOpcode.OPEN_REBATEGIFT || packet.Opcode == MesOpcode.UPDATA_PLAYER_DATA || packet.Opcode == MesOpcode.SERVER_POPUP_WINDOW))
                //{
                //   receiveMessageQueueTemporary.Enqueue(packet);
                //}
                //else
                //{
                receiveMessageQueueActive.Enqueue(packet);
                //}
            }
        }

        /// <summary>
        /// 清理接收消息列表
        /// </summary>
        public void ClearReceiveMessageList()
        {
            receiveMessageQueueActive.Clear();
        }


        /************************************************消息监听**************************************************/
        /// <summary>
        /// 添加一个返回成功的消息的监听
        /// </summary>
        public void AddMessageListener(short opcode, MessageListener ml)
        {
            mlDictionary[opcode] = ml;
        }

        /// <summary>
        /// 在已有的返回成功的监听上追加一个监听
        /// </summary>
        public void AdditionalMessageListener(short opcode, MessageListener ml)
        {
            if (mlDictionary.ContainsKey(opcode))
            {
                mlDictionary[opcode] += ml;
            }
            else
            {
                mlDictionary.Add(opcode, ml);
            }
        }

        /// <summary>
        /// 删除一个返回成功的消息的监听
        /// </summary>
        public void RemoveMessageListener(short opcode)
        {
            mlDictionary.Remove(opcode);
        }

        /// <summary>
        /// 清空返回成功的消息的监听
        /// </summary>
        public void ClearMessageListener()
        {
            mlDictionary.Clear();
        }


        /// <summary>
        /// 添加一个返回错误的消息的监听
        /// </summary>
        public void AddErrorListener(short opcode, MessageListener ml)
        {
            mlDictionaryError[opcode] = ml;
        }

        /// <summary>
        /// 删除一个返回错误的消息的监听
        /// </summary>
        public void RemoveErrorListener(short opcode)
        {
            mlDictionaryError.Remove(opcode);
        }

        /// <summary>
        /// 清空返回错误消息的的监听
        /// </summary>
        public void ClearErrorListener()
        {
            mlDictionaryError.Clear();
        }


        /// <summary>
        /// 添加一个永久的返回成功的消息的监听
        /// </summary>
        public void AddMessageListenerForever(short opcode, MessageListener ml)
        {
            mlDictionaryForever[opcode] = ml;
        }

        public void Dispose()
        {
            ClosedAllSocket();

            mlDictionary.Clear();
            mlDictionaryError.Clear();
            mlDictionaryForever.Clear();
            receiveMessageQueueActive.Clear();
            lastSendMessageTime.Clear();
        }
    }
}






