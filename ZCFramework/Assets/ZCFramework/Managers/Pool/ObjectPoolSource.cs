using System.Collections.Generic;
using UnityEngine;


namespace ZCFrame
{
    
    /// <summary>
    /// 游戏对象池源数据
    /// </summary>
    public class ObjectPoolSource : MonoBehaviour
    {

        /// <summary>
        /// 对象池信息集合
        /// </summary>
        [SerializeField]
        private List<ObjectPoolBasicInfo> _ObjectInfoList = new List<ObjectPoolBasicInfo>();

        /// <summary>
        /// 对象池信息字典
        /// </summary>
        private Dictionary<ObjectTag, ObjectPoolBasicInfo> ObjectInfoDic = null;

        /// <summary>
        /// 初始化
        /// </summary>
        public void Init()
        {
            
            ObjectInfoDic = new Dictionary<ObjectTag, ObjectPoolBasicInfo>();
            int count = _ObjectInfoList.Count;
            
            for (int i = 0; i < count; i++)
            {
                if (ObjectInfoDic.ContainsKey(_ObjectInfoList[i].ObjectTag))
                {
                    Debug.LogError(string.Format("对象池初始化失败,存在重复标签: {0:s} , 请检查源数据", _ObjectInfoList[i].ObjectTag.ToString()));
                    return;
                }
                
                ObjectInfoDic.Add(_ObjectInfoList[i].ObjectTag, _ObjectInfoList[i]);
            }
        }
        
        /// <summary>
        /// 通过对象标签查找对象基础信息
        /// </summary>
        /// <param name="tag">要搜索的标记</param>
        /// <returns>返回一个 ObjectPoolBasicInfo 对象。如果没有找到ObjectPoolBasicInfo，返回null</returns>
        public ObjectPoolBasicInfo FindInfoWithObjectTag(ObjectTag tag)
        {
            ObjectPoolBasicInfo info;

             if (!ObjectInfoDic.TryGetValue(tag,out info))
            {
                Debug.LogError(string.Format("没有找到 {0:s} 对应的的数据, 请检查源数据", tag.ToString()));
                return null;
            }
             return info;
        }
    }
    
}



