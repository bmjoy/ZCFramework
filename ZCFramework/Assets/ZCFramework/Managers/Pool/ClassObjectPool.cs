using System;
using System.Collections.Generic;
using UnityEngine;


namespace ZCFrame
{
    
    /// <summary>
    /// 类对象池
    /// </summary>
    internal class ClassObjectPool :  IDisposable
    {

        private Dictionary<int, Queue<object>> m_ClassObjectPoolDic = null;


        /// <summary>
        /// 类对象在池中的常驻数量
        /// </summary>
        public Dictionary<int, byte> ClassObjectCountDic
        {
            get;
            private set;
        }

        
        public ClassObjectPool()
        {
            m_ClassObjectPoolDic = new Dictionary<int, Queue<object>>();
            ClassObjectCountDic = new Dictionary<int, byte>();
        
        }


        #region 类对象池的出池和回池
        /// <summary>
        /// 类对象池：出池
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Dequeue<T>() where T : class, new()
        {
            lock (m_ClassObjectPoolDic)
            {
                int key = typeof(T).GetHashCode();
                Queue<object> queue = null;
                if (! m_ClassObjectPoolDic.TryGetValue(key, out  queue))
                {
                    queue = new Queue<object>();
                    m_ClassObjectPoolDic.Add(key, queue);
                }
                
                if (queue.Count > 0)
                {
                    object obj;
                    obj = queue.Dequeue();
                    
                    return (T)obj;
                    
                }
                else
                {
                    return new T(); 
                }
            }
        }

        /// <summary>
        /// 类对象池：回池
        /// </summary>
        /// <param name="obj"></param>
        public void Enqueue(object obj)
        {

            lock (m_ClassObjectPoolDic)
            {
                int key = obj.GetHashCode();
                Queue<object> queue = null;

                if (m_ClassObjectPoolDic.TryGetValue(key, out  queue))
                {
                    queue.Enqueue(obj);
                                    
                }
                else
                {
                    Debug.LogError("回池的类对象，不是从池中创建的，请检查，回池对象：" + obj.GetType().Name);
                }
            }
        }

        #endregion


        /// <summary>
        /// 从池中释放长久未使用的对象
        /// </summary>
        public void ClearPool()
        {
            lock (m_ClassObjectPoolDic)
            {
                Debug.Log("释放类对象池" + DateTime.Now);
                int queueCount = 0;
                var enumerator = m_ClassObjectPoolDic.GetEnumerator();
                
                while (enumerator.MoveNext())
                {
                    int key = enumerator.Current.Key;
                    Queue<object> queue = m_ClassObjectPoolDic[key];

                    queueCount = queue.Count;
                    byte resideCount = 0;
                    
                    if (ClassObjectCountDic.TryGetValue(key, out resideCount))
                    {
                        while (queueCount > resideCount)
                        {
                            queueCount--;
                            object obj = queue.Dequeue();
                        
                        }
                    }
                }
                //整个项目有一处GC即可，间隔最好大于60s一次
                GC.Collect();
            }
            
        }

        
        public void Dispose()
        {
            m_ClassObjectPoolDic.Clear();
        }
    }
}


