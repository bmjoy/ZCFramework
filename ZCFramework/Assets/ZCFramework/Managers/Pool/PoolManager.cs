using System;


namespace ZCFrame
{
    
    /// <summary>
    /// 对象池管理
    /// </summary>
    public class PoolManager : ManagerBase, IDisposable
    {

        public ClassObjectPool ClassObjectPool
        {
            get;
            private set;
        }

        public GameObjectPool GameObjectPool
        {
            get;
            private set;
        }

        public PoolManager(ObjectPoolSource ObjectPoolSource)
        {
            ClassObjectPool = new ClassObjectPool();
            GameObjectPool = new GameObjectPool(ObjectPoolSource);
        }

        /// <summary>
        /// 释放类对象池
        /// </summary>
        internal void ClearClassObjectPool()
        {
            ClassObjectPool.ClearPool();
        }

        public void Dispose()
        {
            ClassObjectPool.Dispose();
            GameObjectPool.Dispose();
        }
    }

}


