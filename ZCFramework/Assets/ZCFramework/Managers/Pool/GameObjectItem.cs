using UnityEngine;


namespace ZCFrame
{
    /// <summary>
    /// 对象池游戏物体基类
    /// </summary>
    internal abstract class GameObjectItem : MonoBehaviour
    {
        /// <summary>
        /// 对象编号
        /// </summary>
        public ushort Id
        {
            get;
            private set;
        }

        /// <summary>
        /// 预制体编号
        /// </summary>
        public short PrefabId
        {
            get;
            private set;
        }
    
    
        public void Init(ushort id, short prefabId)
        {
            Id = id;
            PrefabId = prefabId;
        }

        public virtual void Init<T>(T t) { }
    }

}


