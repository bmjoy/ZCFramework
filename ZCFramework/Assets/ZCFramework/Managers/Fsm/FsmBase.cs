using System;


namespace ZCFrame
{

    /// <summary>
    /// 状态机基类
    /// </summary>
    public abstract class FsmBase 
    {

        /// <summary>
        /// 状态机编号
        /// </summary>
        public int FsmId { get; private set; }

        /// <summary>
        /// 状态机拥有者
        /// </summary>
        public Type Owner { get; private set; }

        /// <summary>
        /// 当前状态
        /// </summary>
        public byte CurrStateType;

        public FsmBase(int fsmId, Type owner)
        {
            FsmId = fsmId;
            Owner = owner;
        }

        public abstract void ShutDown();
    }
}


