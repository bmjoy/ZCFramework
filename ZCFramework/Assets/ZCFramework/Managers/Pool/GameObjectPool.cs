using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace ZCFrame
{
    
    /// <summary>
    /// 游戏对象池
    /// </summary>
    public class GameObjectPool :IDisposable
    {
        
        /// <summary>
        /// 游戏对象源数据
        /// </summary>
        private ObjectPoolSource m_ObjectPoolSource;

        /// <summary>
        /// 游戏对象池字典
        /// </summary>
        public Dictionary<ObjectTag, Queue<GameObject>> ObjectPoolDic
        {
            get;
            private set;
        }

        #if UNITY_EDITOR
        /// <summary>
        /// 池子外部数量
        /// </summary>
        public Dictionary<ObjectTag, int> ObjectPoolExternalNumDic = new Dictionary<ObjectTag, int>(); 
        #endif
       
        
        internal GameObjectPool(ObjectPoolSource ObjectPoolSource)
        {
            m_ObjectPoolSource = ObjectPoolSource;
            ObjectPoolDic = new Dictionary<ObjectTag, Queue<GameObject>>();
        }

        
        #region  游戏对象池的出池和回池
        /// <summary>
        /// 游戏对象：出池
        /// </summary>
        /// <param name="objTag">对象标签</param>
        /// <typeparam name="T">对象类型</typeparam>
        /// <returns></returns>
        internal T Spawn<T>(ObjectTag objTag) where T : ObjectBase
        {
            
            lock (ObjectPoolDic)
            {
                Queue<GameObject> queue;

                if (!ObjectPoolDic.TryGetValue(objTag, out queue))
                {
                    queue = new Queue<GameObject>();
                    ObjectPoolDic.Add(objTag, queue);
                }

                T t;
                
                if (queue.Count > 0)
                {
                    GameObject go = queue.Dequeue();
                    t = go.GetComponent<T>();
                }
                else
                {
                    t = OnNeedCreateNewObject<T>(objTag);
                }

                t.gameObject.SetActive(true);
                //当出池
                t.OnSpawn();
                
                #if UNITY_EDITOR
                DequeueAndEnqueueCount(objTag, 1);              
                #endif
                
                return t;
            }
            
        }
        
        /// <summary>
        /// 需要生成新的对象
        /// </summary>
        /// <param name="objTag">对象标签</param>
        /// <typeparam name="T">对象类型</typeparam>
        /// <returns></returns>
        private T OnNeedCreateNewObject<T>(ObjectTag objTag) where T : ObjectBase
        {
            GameObject go = m_ObjectPoolSource.FindInfoWithObjectTag(objTag).Prefab;
            go = GameObject.Instantiate(go);
            T t = go.AddComponent<T>();
            
            //需要初始化
            t.Init(objTag);
            
            return t;
        }

        
        /// <summary>
        /// 游戏对象：回池
        /// </summary>
        /// <param name="对象"></param>
        /// <typeparam name="对象类型"></typeparam>
        internal void Despawn<T>(T t) where T : ObjectBase
        {
            lock (ObjectPoolDic)
            {

                Queue<GameObject> queue;

                if (ObjectPoolDic.TryGetValue(t.ObjTag, out queue))
                {
                    if (queue.Contains(t.gameObject))
                    {
                        Debug.LogError(string.Format("回池失败，{0:s} 重复入池 ", t.name));
                        return;
                    }
                    
                    t.gameObject.SetActive(false);
                    //当回池
                    queue.Enqueue(t.gameObject);
                    t.OnDespawn();
                    
                    #if UNITY_EDITOR
                    DequeueAndEnqueueCount(t.ObjTag, -1);              
                    #endif
                }
                else
                {
                    Debug.LogError(string.Format("回池失败，未存在 {0:s} 对象标签 ", t.ObjTag));
                }
            }
        }
        #endregion
        
        
        #region 出池与回池计数
        #if UNITY_EDITOR
        /// <summary>
        /// 出池与回池计数
        /// </summary>
        /// <param name="objTag">对象标签</param>
        /// <param name="variable"></param>
        private void DequeueAndEnqueueCount(ObjectTag objTag, int variable)
        {
           
            if (ObjectPoolExternalNumDic.ContainsKey(objTag))
            {
                ObjectPoolExternalNumDic[objTag] += variable;
            }
            else
            {
                ObjectPoolExternalNumDic[objTag] = 1;
            }
        }
        #endif
        #endregion
        
        
        
        public void Dispose()
        {
            ObjectPoolDic.Clear();
        }
    }

}



