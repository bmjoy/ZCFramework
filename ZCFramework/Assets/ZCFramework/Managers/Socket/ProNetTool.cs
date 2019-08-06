
namespace ZCFrame
{
    /// <summary>
    /// 收到的消息处理委托
    /// </summary>
    /// <param name="packet"></param>
    public delegate void ReceiveCallBack(NetPacket packet);				

    public class ProNetTool
    {
        public static byte[] PutIntToBuffet(int value, byte[] buf, int index)
        {
            buf[index++] = (byte)((value >> 24) & byte.MaxValue);
            buf[index++] = (byte)((value >> 16) & byte.MaxValue);
            buf[index++] = (byte)((value >> 8) & byte.MaxValue);
            buf[index++] = (byte)(value & byte.MaxValue);

            return buf;
        }

        public static int GetIntBytBuf(byte[] buf, int index)
        {
            int value = 0;
            value += (buf[index++] & byte.MaxValue) << 24;
            value += (buf[index++] & byte.MaxValue) << 16;
            value += (buf[index++] & byte.MaxValue) << 8;
            value += (buf[index++] & byte.MaxValue);
            return value;
        }

        public static byte[] PutShortToBuffet(short value, byte[] buf, int index)
        {
            buf[index++] = (byte)((value >> 8) & byte.MaxValue);
            buf[index++] = (byte)(value & byte.MaxValue);

            return buf;
        }

        public static short GetShortBytBuf(byte[] buf, int index)
        {
            short value = 0;
            value += (short)((buf[index++] & byte.MaxValue) << 8);
            value += (short)(buf[index++] & byte.MaxValue);
            return value;
        }

        public static string PrintByteArray(byte[] buf)
        {
            string str = "";
            for (int i = 0; i < buf.Length; i++)
            {
                str += (buf[i] + ",");
            }

            return str;
        }

        public static NetPacket ReadPacket(byte[] lengthBuf, System.IO.Stream reqStream, System.Net.Sockets.Socket socket)
        {
            if (lengthBuf == null || (reqStream == null && socket == null)) return null;

            int packgeLength = GetIntBytBuf(lengthBuf, 0);
            packgeLength -= 4;//减去已经读取的长度


            byte[] packgeBuff = new byte[packgeLength];
            byte[] readbuff = new byte[packgeLength];

            int readLength = 0;

            while (readLength < packgeLength)
            {
                int size;
                if (reqStream != null)
                {
                    size = reqStream.Read(readbuff, 0, readbuff.Length);
                }
                else
                {
                    size = socket.Receive(readbuff);
                }


                for (int i = 0; i < size; i++)
                {
                    packgeBuff[readLength + i] = readbuff[i];
                }

                readLength += size;


                if (readLength < packgeLength)
                {
                    readbuff = new byte[packgeLength - readLength];
                }
            }

            NetPacket packet = new NetPacket();
            packet.Decode(packgeBuff);

            return packet;
        }
    }
}


