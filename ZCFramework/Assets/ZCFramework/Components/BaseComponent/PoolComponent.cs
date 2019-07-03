using System.Collections.Generic;
using UnityEngine;


namespace ZCFrame
{
 
    /// <summary>
    /// 对象池组件
    /// </summary>
    public class PoolComponent : ZCBaseComponent, IUpdateComponent
    {
        
        /// <summary>
        /// 游戏对象池源数据
        /// </summary>
        [SerializeField]
        public ObjectPoolSource ObjectPoolSource = null;
        
        /// <summary>
        /// 释放池中对象的时间间隔
        /// </summary>
        public int ClearInterval = 30;

        private float m_NextClearTime;

        public PoolManager PoolManager
        {
            get;
            private set;
        }
        
        
        protected override void OnAwake()
        {
            base.OnAwake();
            
            ObjectPoolSource.Init();
            PoolManager = new PoolManager(ObjectPoolSource);
            
            GameEntry.RegisterUpdateComponent(this);
            m_NextClearTime = Time.time;
            
        }
        
        
        protected override void OnStart()
        {
            base.OnStart();
            InitReside();
        }
        
        
        /// <summary>
        /// 设置常用类的常驻数量
        /// </summary>
        private void InitReside()
        {
//            SetClassObjectResideCount<HttpRoutine>(3);
              SetClassObjectResideCount<Dictionary<string,object>>(3);
        }
        
        
        /// <summary>
        /// 设置类对象的常驻数量
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="count"></param>
        public void SetClassObjectResideCount<T>(byte count) where T : class
        {
            PoolManager.ClassObjectPool.SetResideCount<T>(count);
        }
        
        
        #region 类对象的出池和回池
        /// <summary>
        /// 类对象池：出池
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T DequeueClassObject<T>() where T : class, new()
        {
            return PoolManager.ClassObjectPool.Dequeue<T>();
        }

        /// <summary>
        /// 类对象池：回池
        /// </summary>
        /// <param name="obj"></param>
        public void EnqueueClassObject(object obj)
        {
            PoolManager.ClassObjectPool.Enqueue(obj);
        }
        #endregion
        
        
        #region 游戏对象的出池回池
       /// <summary>
       /// 从对象池中获取对象
       /// </summary>
       /// <param name="poolId">对象池编号</param>
       /// <typeparam name="T">对象类型</typeparam>
       /// <returns></returns>
        public T GameObjectSpawn<T>(ObjectTag tag) where T : ObjectBase
        {
            return PoolManager.GameObjectPool.Spawn<T>(tag);
        }
        
       
        /// <summary>
        /// 对象回池
        /// </summary>
        /// <param name="t"></param>
        /// <typeparam name="T"></typeparam>
        public void GameObjectDespawn<T>(T t) where T : ObjectBase
        {
            PoolManager.GameObjectPool.Despawn(t);
        }
        #endregion
        
        
        public void OnUpdate()
        {
            if (Time.time > m_NextClearTime + ClearInterval)
            {
                //类对象池该释放了
                m_NextClearTime = Time.time;
                PoolManager.ClearClassObjectPool();
            }
        }
        
        
        public override void Shutdown()
        {
            GameEntry.RemoveUpdateComponent(this);
            PoolManager.Dispose();
        }
       
    }
}


