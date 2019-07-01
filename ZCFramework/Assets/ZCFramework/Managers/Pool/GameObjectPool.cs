using System.Collections.Generic;
using UnityEngine;


namespace ZCFrame
{
    
    /// <summary>
    /// 游戏对象池
    /// </summary>
    internal class GameObjectPool
    {
        
        private Dictionary<ushort, GameObjctSpawnPool> m_GameObjectPoolDic = null;
        
        public GameObjectPool()
        {
            m_GameObjectPoolDic = new Dictionary<ushort, GameObjctSpawnPool>();
        }

        
        #region  游戏对象池的出池和回池
        /// <summary>
        ///  游戏对象：出池
        /// </summary>
        /// <param name="对象池编号"></param>
        /// <typeparam name="对象类型"></typeparam>
        /// <returns></returns>
        public T Dequeue<T>(ushort poolId) where T : GameObjectItem
        {
            return Dequeue<T>(poolId, 0);
        }
        
        /// <summary>
        /// 游戏对象：出池
        /// </summary>
        /// <param name="对象池编号"></param>
        /// <param name="对象预制体编号"></param>
        /// <typeparam name="对象类型"></typeparam>
        /// <returns></returns>
        public T Dequeue<T>(ushort poolId, short prefabId) where T : GameObjectItem
        {
            lock (m_GameObjectPoolDic)
            {

                GameObjctSpawnPool pool;

                if (!m_GameObjectPoolDic.TryGetValue(poolId, out pool))
                {
                    pool = new GameObjctSpawnPool(poolId);
                    m_GameObjectPoolDic.Add(poolId, pool);
                }

                return pool.Dequeue<T>(prefabId);
            }
        }

        /// <summary>
        /// 游戏对象：回池
        /// </summary>
        /// <param name="对象"></param>
        /// <typeparam name="对象类型"></typeparam>
        public void Enqueue<T>(T t) where T : GameObjectItem
        {
            lock (m_GameObjectPoolDic)
            {

                GameObjctSpawnPool pool;

                if (m_GameObjectPoolDic.TryGetValue(t.Id, out pool))
                {
                    pool.Enqueue<T>(t);
                }
                else
                {
                    Debug.LogError(string.Format("对象回池错误，未存在 {0:s} 对象池  ", t.name));
                }
            }
        }
        #endregion
    }

}



