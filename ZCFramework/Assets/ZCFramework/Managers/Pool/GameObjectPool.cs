using System.Collections.Generic;
using UnityEngine;


public class GameObjectPool
{


    public static GameObjectPool Manager
    {
        get
        {
            if (_Manager == null)
            {
                _Manager = new GameObjectPool();
                _Manager.Init();

            }

            return _Manager;
        }
        
    }

    private static GameObjectPool _Manager = null;
    
    
    private Dictionary<ushort, GameObjcetBasePool> m_GameObjectPoolDic = null;


    public void Init()
    {
        m_GameObjectPoolDic = new Dictionary<ushort, GameObjcetBasePool>();
    }



    #region 类对象池的出池和回池
    /// <summary>
    /// 类对象池：出池
    /// </summary>
    /// <param name="对象池编号"></param>
    /// <typeparam name="对象类型"></typeparam>
    /// <returns></returns>
    public T Dequeue<T>(ushort poolId) where T : GameObjectItem
    {
        return Dequeue<T>(poolId, 0);
    }
    

    /// <summary>
    /// 类对象池：出池
    /// </summary>
    /// <param name="对象池编号"></param>
    /// <param name="对象预制体编号"></param>
    /// <typeparam name="对象类型"></typeparam>
    /// <returns></returns>
    public T Dequeue<T>(ushort poolId, short prefabId) where T : GameObjectItem
    {
        lock (m_GameObjectPoolDic)
        {

            GameObjcetBasePool pool;

            if (!m_GameObjectPoolDic.TryGetValue(poolId, out pool))
            {
                pool = new GameObjcetBasePool();
                pool.Init(poolId);
                m_GameObjectPoolDic.Add(poolId, pool);
            }

            return pool.Dequeue<T>(prefabId);
        }
    }

    /// <summary>
    /// 类对象池：回池
    /// </summary>
    /// <param name="对象"></param>
    /// <typeparam name="对象类型"></typeparam>
    public void Enqueue<T>(T t) where T : GameObjectItem
    {
        lock (m_GameObjectPoolDic)
        {
            
            GameObjcetBasePool pool;

            if (m_GameObjectPoolDic.TryGetValue(t.Id,out pool))
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


public class GameObjcetBasePool
{
    
    private Dictionary<int, Queue< GameObject>> m_ClassObjectPoolDic = null;
    /// <summary>
    /// 预制体字典
    /// </summary>
    private Dictionary<int, GameObject> m_PrefabDic = null;
    /// <summary>
    /// 预制题路径
    /// </summary>
    private string m_PrefabPath = "";
    /// <summary>
    /// 对象池数据
    /// </summary>
    private PoolData Pooldata = null;
    
    public void Init(ushort poolId)
    {
        
        Pooldata = DataManage.Manage.getTable<PoolData>().getData(poolId);
        m_ClassObjectPoolDic = new Dictionary<int, Queue<GameObject>>();
        m_PrefabDic = new Dictionary<int, GameObject>();
    
    }
    
    
    #region 类对象池的出池和回池
    public T Dequeue<T>(short prefabIndex) where T : GameObjectItem
    {
        lock (m_ClassObjectPoolDic)
        {
            
            Queue<GameObject> queue;

            if (!m_ClassObjectPoolDic.TryGetValue(prefabIndex,out queue))
            {

                //判断预制体索引是否超出界限
                if (prefabIndex >= Pooldata.prefabCount)
                {
                    Debug.LogError(string.Format("加载预制体 {0:s} 失败，预制体索引 {1:d} 超出界限 {2:d}", Pooldata.name, prefabIndex, Pooldata.prefabCount));
                    return default(T);
                }
                
                OnNeedLoadNewPrefab(prefabIndex);
                
                queue = new Queue<GameObject>();
                m_ClassObjectPoolDic.Add(prefabIndex, queue);
            }

            T t;
           
            if (queue.Count > 0)
            {
                GameObject go = queue.Dequeue();
                go.gameObject.SetActive(true);
                t = go.GetComponent<T>();
            }
            else
            {
                t = OnNeedCreateNewObject<T>(prefabIndex);
            }

            return t;

        }

    }

    /// <summary>
    /// 类对象池：回池
    /// </summary>
    /// <param name="对象"></param>
    /// <typeparam name="对象类型"></typeparam>
    public void Enqueue<T>(T t) where T : GameObjectItem
    {
        lock (m_ClassObjectPoolDic)
        {
            
            Queue<GameObject> queue;

            if (m_ClassObjectPoolDic.TryGetValue(t.PrefabId,out queue))
            {
                t.gameObject.SetActive(false);
                queue.Enqueue(t.gameObject);
            }
            else
            {

                Debug.LogError(string.Format("回池的类对象，不是从池中创建的，请检查，对象:{0:s}  对象编号{1:d}  对象预制体编号:{2:d}", t.name, t.Id, t.PrefabId));
            }
        }
    }
    
    #endregion
    
    /// <summary>
    /// 需要加载新的Prefab
    /// </summary>
    /// <param name="预制体编号"></param>
    private void OnNeedLoadNewPrefab (int prefabIndex)
    {
        
        GameObject go;

        if (!m_PrefabDic.TryGetValue(prefabIndex, out go))
        {
            string path = Pooldata.path + prefabIndex;
            go = Resources.Load<GameObject>(path);
            m_PrefabDic.Add(prefabIndex, go);
        }
        
    }

    /// <summary>
    /// 需要生成新的对象
    /// </summary>
    /// <param name="对象编号"></param>
    /// <param name="对象预制体编号"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    private T OnNeedCreateNewObject<T>(short prefabIndex) where T : GameObjectItem
    {
        
        string typeNmae = typeof(T).Name;
     
        if (!typeNmae.Equals(Pooldata.name))
        {
            Debug.LogError(string.Format("创建对象池失败，对象类型:{0:s} 与 实际对象类型:{1:s} 不一致, 请检查对象池编号:{2:D}",typeNmae, Pooldata.name, Pooldata.id));
            return default(T);
        }

        GameObject go;
        
        if (m_PrefabDic.TryGetValue(prefabIndex, out go))
        {
            go = GameObject.Instantiate(go);
            T t = go.AddComponent<T>();
            t.Init((ushort)Pooldata.id, prefabIndex);
            return t;
        }
        
        Debug.LogError(string.Format("未找到对象的预制体，请检查: {0:s}  对象池编号:{1:D}  预制体编号:{2:D}",typeof(T).Name, Pooldata.id, prefabIndex));
        return default(T);
    }
    
}


public abstract class GameObjectItem : MonoBehaviour
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


