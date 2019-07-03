using UnityEngine;



namespace ZCFrame
{
    /// <summary>
    /// 对象池基础信息
    /// </summary>
    [System.Serializable]
    public class ObjectPoolBasicInfo
    {
        
        [SerializeField] private ObjectTag _ObjectTag;
        [SerializeField] private GameObject _Prefab;
        

        /// <summary>
        /// 对象标签
        /// </summary>
        public ObjectTag ObjectTag
        {
            get { return _ObjectTag; }
            private set { _ObjectTag = value; }
        }

        
        /// <summary>
        /// 对象预制体
        /// </summary>
        public GameObject Prefab
        {
            get { return _Prefab; }
            private set { _Prefab = value; }
        }
    }

}
 

