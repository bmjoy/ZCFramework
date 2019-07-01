using System;


namespace ZCFrame
{
    
    /// <summary>
    /// 对象池管理
    /// </summary>
    internal class PoolManager : ManagerBase, IDisposable
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

        internal PoolManager()
        {
            ClassObjectPool = new ClassObjectPool();
            GameObjectPool = new GameObjectPool();
        }

        /// <summary>
        /// 释放类对象池
        /// </summary>
        public void ClearClassObjectPool()
        {
//            ClassObjectPool.ClearPool();
        }

        public void Dispose()
        {
//            ClassObjectPool.Dispose();
        }
    }

}


