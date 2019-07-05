using System;
using UnityEngine;


namespace ZCFrame
{
    
    /// <summary>
    /// 变量基类
    /// </summary>
    public abstract class VariableBase 
    {

        /// <summary>
        /// 获取变量类型
        /// </summary>
        public abstract Type Type { get; }

        /// <summary>
        /// 引用计数
        /// </summary>
        public byte ReferenceCount
        {
            get;
            private set;
        }

        /// <summary>
        /// 保留对象
        /// </summary>
        public void Retain()
        {
            ReferenceCount++;
        }

        /// <summary>
        /// 释放对象
        /// </summary>
        public void Release()
        {

            if (ReferenceCount.Equals(0))
            {
                Debug.LogError(string.Format("{0:s} 重复释放, 请检查", this.ToString()));
                return; 
            }
            
            ReferenceCount--;  
            
            if (ReferenceCount < 1)
            {
                //回池
               GameEntry.Pool.EnqueueClassObject(this);
            }
        }
    }
}


