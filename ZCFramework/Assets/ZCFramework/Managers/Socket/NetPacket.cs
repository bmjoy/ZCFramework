using System;
using UnityEngine;
using Google.Protobuf;
using System.IO;

namespace ZCFrame
{
    public class NetPacket
    {

        /// <summary>
        /// 包头长度
        /// </summary>
        private const int PACKET_HEAD_SIZE = 14;

        // 键：通常为玩家编号
        private int key = 0;
        public int Key
        {
            get { return key; }
            set { key = value; }
        }

        // 操作码
        private short opcode = 0;
        public short Opcode
        {
            get { return opcode; }
            set { opcode = value; }
        }

        // 选项参数
        private short param = 0;
        public short Param
        {
            get { return param; }
            set { param = value; }
        }

        // 操作结果
        private short result = 0;
        public short Result
        {
            get { return result; }
            set { result = value; }
        }

        private byte[] body = null;


        public int GetBodyLength()
        {
            return body == null ? 0 : body.Length;
        }


        public void PutBody<T>(T obj) where T : IMessage<T>
        {
            try
            {
                using (MemoryStream stream = new MemoryStream())
                {
                    //将类文件信息转化为字节流
                    obj.WriteTo(stream);
                    body = stream.ToArray();
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
        }

        public T GetBody<T>(string logHead = null) where T :  IMessage, new()
        {
            try
            {
                if (!string.IsNullOrEmpty(logHead))
                {
                    Debug.Log(logHead + "  " + ProNetTool.PrintByteArray(body));
                }

                if (body == null) return default;

                //将字节数据还原到对象中
                IMessage IMperson = new T();
                T t = new T();
                t = (T)IMperson.Descriptor.Parser.ParseFrom(body);
                return t;

            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }

            return default;
        }


        public byte[] Encoder(string logHead = null)
        {
            int pLength = 4 + PACKET_HEAD_SIZE;//4是总流长度本身的长度
            if (body != null) pLength += (4 + body.Length);//4是包体长度本身的长度

            byte[] buf = new byte[pLength];

            buf = ProNetTool.PutIntToBuffet(pLength, buf, 0);
            buf = ProNetTool.PutIntToBuffet(0, buf, 4);
            buf = ProNetTool.PutIntToBuffet(Key, buf, 8);
            buf = ProNetTool.PutShortToBuffet(Opcode, buf, 12);
            buf = ProNetTool.PutShortToBuffet(Param, buf, 14);
            buf = ProNetTool.PutShortToBuffet(Result, buf, 16);

            if (body != null)
            {
                buf = ProNetTool.PutIntToBuffet(body.Length, buf, 18);
                Array.Copy(body, 0, buf, 22, body.Length);
            }

            if (!string.IsNullOrEmpty(logHead))
            {
                Debug.Log(logHead + "  " + ProNetTool.PrintByteArray(buf));
            }

            return buf;
        }


        public void Decode(byte[] buf, string logHead = null)
        {
            if (buf.Length < PACKET_HEAD_SIZE)
            {
                Debug.Log("－－－－－－－－－ 错误包 －－－－－－－－－－－－");
                return;
            }

            ProNetTool.GetIntBytBuf(buf, 0);
            Key = ProNetTool.GetIntBytBuf(buf, 4);
            Opcode = ProNetTool.GetShortBytBuf(buf, 8);
            Param = ProNetTool.GetShortBytBuf(buf, 10);
            Result = ProNetTool.GetShortBytBuf(buf, 12);

            try
            {
                if (buf.Length >= 14)
                {
                    int bodyLength = ProNetTool.GetIntBytBuf(buf, 14);

                    body = new byte[bodyLength];
                    Array.Copy(buf, 18, body, 0, body.Length);
                }
                else
                {
                    body = null;
                }
            }
            catch
            {
                body = null;
            }

            if (!string.IsNullOrEmpty(logHead))
            {
                Debug.Log(logHead + "  key=" + Key + " opcode=" + Opcode + " param=" + Param + " result=" + Result);
            }

        }


        public void logPacket(string logHead)
        {
#if ON_EDITOR
		if (Opcode == MesOpcode.FISH_CREATE_MONSTER || Opcode == MesOpcode.FISH_SYN_SHELL || Opcode == MesOpcode.HEARTBEAT || Opcode == MesOpcode.GET_SERVER_TIME
			|| Opcode == MesOpcode.FISH_SHELL_HIT || Opcode == MesOpcode.UPDATA_PLAYER_DATA
		)
			return;
		
		string mes = string.Format ("<color=#FF1F77>{0:s}</color> {1:D} Opcode=<color=#1DFF00>{2:D}</color> Param=<color=#1879FF>{3:D}</color> Result={4:D} BLength={5:D}",
			logHead, System.DateTime.Now.Millisecond, Opcode, Param, Result, getBodyLength ());

		Debug.Log (mes);
#endif
        }
    }
}


