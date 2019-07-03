using UnityEngine;


namespace ZCFrame
{
    /// <summary>
    /// 对象池游戏物体基类
    /// </summary>
    public abstract class ObjectBase : MonoBehaviour
    {
        
        /// <summary>
        /// 对象标签
        /// </summary>
        internal ObjectTag ObjTag
        {
            get;
            private set;
        }
        
        public void Init(ObjectTag objectTag)
        {
            ObjTag = objectTag;
            OnInit();
        }

        
        /// <summary>
        /// 当初始化
        /// </summary>
        public virtual void OnInit()
        {
            
        }

        /// <summary>
        /// 当出池子
        /// </summary>
        public virtual void OnSpawn()
        {
            
        }

        /// <summary>
        /// 当回到池子
        /// </summary>
        public virtual void OnDespawn()
        {
            
        }
        
    }

}


