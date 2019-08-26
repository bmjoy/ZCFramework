namespace ZCFrame
{
    /// <summary>
    /// 协议接口
    /// </summary>
    public interface IProto
    {
        //协议编号
        ushort ProtoCode { get; }

        //协议名称
        string ProtoEnName { get; }

        //协议转化为数组
        byte[] ToArray();
    }
}