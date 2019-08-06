using System;
using System.Collections.Generic;
using Debug = UnityEngine.Debug;


namespace ZCFrame
{
    
    /// <summary>
    /// 类对象池
    /// </summary>
    public class ClassObjectPool : IDisposable
    {

        private readonly Dictionary<int, Queue<object>> m_ClassObjectPoolDic = null;


        /// <summary>
        /// 类对象在池中的常驻数量
        /// </summary>
        public Dictionary<int, byte> ClassObjectCountDic
        {
            get;
            private set;
        }

        //对象池在编辑器面板中显示时，保存池中对象的计数
        #if UNITY_EDITOR
        public Dictionary<Type, int> InspectorInDic = new Dictionary<Type, int>();
        //对象池在编辑器面板中显示时，保存池外对象的计数
        public Dictionary<Type, int> InspectorOutDic = new Dictionary<Type, int>(); 
        #endif
        
        
        internal ClassObjectPool()
        {
            m_ClassObjectPoolDic = new Dictionary<int, Queue<object>>();
            ClassObjectCountDic = new Dictionary<int, byte>();
        
        }


        /// <summary>
        /// 设置常驻数量
        /// </summary>
        /// <param name="count"></param>
        /// <typeparam name="T"></typeparam>
        public void SetResideCount<T>(byte count) where T : class
        {
            int key = typeof(T).GetHashCode();
            ClassObjectCountDic[key] = count;
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
                
                #if UNITY_EDITOR
                //出池计数
                DequeueAndEnqueueCount(typeof(T), true);
                #endif

                Type type = typeof(T);
                
                if (queue.Count > 0)
                {
                    object obj = queue.Dequeue();
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
                int key = obj.GetType().GetHashCode();
            
                Queue<object> queue = null;

                if (m_ClassObjectPoolDic.TryGetValue(key, out  queue))
                {
                    
                    if (queue.Contains(obj))
                    {
                        Debug.LogError(string.Format("回池失败，{0:s} 重复入池", obj.ToString()));
                        return;
                    }
                    
                    #if UNITY_EDITOR
                    //回池计数
                    DequeueAndEnqueueCount(obj.GetType(), false);
                    #endif
                    
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
        internal void ClearPool()
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

                    #if UNITY_EDITOR
                    Type t = null;
                    #endif
                    
                    queueCount = queue.Count;
                    byte resideCount = 0;
                    
                    if (ClassObjectCountDic.TryGetValue(key, out resideCount))
                    {
                        while (queueCount > resideCount)
                        {
                            queueCount--;
                            object obj = queue.Dequeue();

                            #if UNITY_EDITOR
                            t = obj.GetType();
                            InspectorInDic[t]--;
                            #endif
                        }

                        #if UNITY_EDITOR
//                        if (queueCount <= 0)
//                        {
//                            if (t != null)
//                            {
//                                InspectorInDic.Remove(t);
//                            }
//                        }
                        #endif
                    }
                }
                
                //整个项目有一处GC即可，间隔最好大于60s一次
                GC.Collect();
            }
            
        }

        
        #region 出池与回池计数
        #if UNITY_EDITOR
        /// <summary>
        /// 出池与回池计数
        /// </summary>
        /// <param name="t">对象类型</param>
        /// <param name="dequeueOrEnqueue">True 为出池 False 为入池 </param>
        private void DequeueAndEnqueueCount(Type t, bool dequeueOrEnqueue)
        {
          
            if (InspectorInDic.ContainsKey(t))
            {
                if (dequeueOrEnqueue)
                {
                    if (InspectorInDic[t] > 0)
                    {
                        InspectorInDic[t]--;
                    }

                    InspectorOutDic[t]++;
                }
                else
                {
                    InspectorInDic[t]++;
                    InspectorOutDic[t]--;
                }
            }
            else
            {
                InspectorInDic[t] = 0;
                InspectorOutDic[t] = 1;
            }
        }
        #endif
        #endregion
       

        public void Dispose()
        {
            m_ClassObjectPoolDic.Clear();
        }
        
    }
    
}


