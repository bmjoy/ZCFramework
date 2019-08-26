using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;


namespace ZCFrame
{

    //USER_SOURCEC_RES 是否使用原始资源

    /// <summary>
    /// 资源组件
    /// </summary>
    public class ResourceComponent : ZCBaseComponent
    {
        public string LocalFilePath
        {
            get
            {
                return Application.dataPath;
            }
        }

        /// <summary>
        /// 主依赖的文件名
        /// </summary>
        private const string MainManifestName = "TapFun";
        /// <summary>
        /// 主依赖关系
        /// </summary>
        private AssetBundleManifest MainManifest = null;

        /// <summary>
        /// 加载的资源包
        /// </summary>
        private Dictionary<string, AssetBundle> m_LoadedAssetBundle = null;

        /// <summary>
        /// 需要持有不清除的资源包
        /// </summary>
        private List<string> m_NeedHoldAssetBundleList = null;

        // <summary>
        /// 下载过的资源包信息
        /// </summary>
        private List<string> m_DownladedAssetInfo = null;
        /// <summary>
        /// 下载过在资源包信息文件路径
        /// </summary>
        private string downdladeAssetInfoFilePath = "";


        /// <summary>
        /// 下载的资源资源地址
        /// </summary>
        private string downloadResPath_Read = "";
        /// <summary>
        /// 下载的资源资源地址
        /// </summary>
        private string downloadResPath_Write = "";
        /// <summary>
        /// 本地资源地址
        /// </summary>
        [HideInInspector]
        public string NativeResPath { get; private set; }



        protected override void OnAwake()
        {
            base.OnAwake();
            
            downloadResPath_Write = Application.persistentDataPath + "/resources/";

#if UNITY_IPHONE && !UNITY_EDITOR
		NativeResPath = "file://" + Application.dataPath +"/Raw/";
		downloadResPath_Read = "file://" + downloadResPath_Write;
#elif UNITY_ANDROID && !UNITY_EDITOR
		NativeResPath = "jar:file://" + Application.dataPath + "!/assets/";
		downloadResPath_Read = "file://" + downloadResPath_Write;
#else
            NativeResPath = "file://" + Application.dataPath + "/StreamingAssets/";
            downloadResPath_Read = "file:///" + downloadResPath_Write;
#endif

            //不存在创建
            if (!Directory.Exists(downloadResPath_Write))
            Directory.CreateDirectory(downloadResPath_Write);

            //下载过在资源包信息文件路径
            downdladeAssetInfoFilePath = downloadResPath_Write + "DowndladeAssetInfo";

            m_LoadedAssetBundle = new Dictionary<string, AssetBundle>();
            m_NeedHoldAssetBundleList = new List<string>();

            ReadDownloadAssetInfo();
        }

        /// <summary>
        /// 读取本地文件到byte数组
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public byte[] GetFileBuffer(string path)
        {
            byte[] buffer = null;

            using (FileStream fs = new FileStream(path, FileMode.Open))
            {
                buffer = new byte[fs.Length];
                fs.Read(buffer, 0, buffer.Length);
            }
           
            return buffer;
        }

        #region Hold Asset Bund
        /// <summary>
        /// 添加需要持有不清除的资源包
        /// </summary>
        /// <param name="names"></param>
        public void AddHoldAssetBundName(params string[] names)
        {
            int length = names.Length;
            for (int i = 0; i < length; i++)
            {
                m_NeedHoldAssetBundleList.Add(names[i]);
            }
        }

        /// <summary>
        /// 删除持有资源包标记
        /// </summary>
        public void RemoveHoldAssetBundName(params string[] names)
        {
            for (int i = 0; i < names.Length; i++)
            {
                m_NeedHoldAssetBundleList.Remove(names[i]);
            }
        }

        /// <summary>
        /// 清空持有资源包标记
        /// </summary>
        public void ClearHoldAssetBundName()
        {
            m_NeedHoldAssetBundleList.Clear();
        }
        #endregion

        #region Clear Asset Bundle
        /// <summary>
        /// 清理所有资源包
        /// </summary>
        /// <param name="unloadAllLoadedObjects"></param>
        public void ClearAssetBundle(bool unloadAllLoadedObjects = false)
        {
            if (m_LoadedAssetBundle != null)
            {
                foreach (AssetBundle ab in m_LoadedAssetBundle.Values)
                {
                    //参数为false时，bundle内的序列化数据将被释放，但是任何从这个bundle中实例化的物体都将完好，所以不能从这个bundle中加载更多物体
                    //参数为true时，所有从该bundle中加载的物体也将被销毁，如果场景中有物体引用该资源，引用会丢失
                    ab.Unload(unloadAllLoadedObjects);
                }
            }
        }

        /// <summary>
        /// 释放资源包
        /// </summary>
        /// <param name="unloadAllLoadedObjects"></param>
        public void UnloadUnusedAssetBundle(bool unloadAllLoadedObjects = false)
        {
            if (m_LoadedAssetBundle != null)
            {
                List<string> unloadABN = new List<string>();

                foreach (string key in m_LoadedAssetBundle.Keys)
                {
                    bool isUnloadFalse = m_NeedHoldAssetBundleList.Contains(key)? false : true;

                    if (isUnloadFalse)
                    {
                        m_LoadedAssetBundle[key].Unload(unloadAllLoadedObjects);
                        unloadABN.Add(key);
                    }
                }

                for (int i = 0; i < unloadABN.Count; i++)
                {
                    m_LoadedAssetBundle.Remove(unloadABN[i]);
                }
            }
        }

        /// <summary>
        /// 卸载资源包
        /// </summary>
        /// <param name="abName"></param>
        /// <param name="unloadAllLoadedObjects"></param>
        public void UnloadAssetBundle(string abName, bool unloadAllLoadedObjects = false)
        {
            if (m_LoadedAssetBundle != null)
            {
                if (m_LoadedAssetBundle.ContainsKey(abName))
                {
                    m_LoadedAssetBundle[abName].Unload(unloadAllLoadedObjects);
                    m_LoadedAssetBundle.Remove(abName);
                }
            }
        }
        #endregion

        #region Load Asset
        /// <summary>
        /// 加载资源  如果资源包没有加载返回default
        /// </summary>
        public T LoadAsset<T>(string path, string bundleName, string assetName) where T : UnityEngine.Object
        {

#if USER_SOURCEC_RES
            string name = string.Format("ResAssetBundle/{0:s}{1:s}", path, assetName);
            T t = Resources.Load<T>(name);
            return t;
#else
            //已经加载过
            if (m_LoadedAssetBundle.TryGetValue(bundleName, out AssetBundle bundle))
            {
                T obj = bundle.LoadAsset<T>(assetName);
                return obj;
            }
            else
            {
                Debug.LogWarningFormat("{0:s} 资源包没有加载", bundleName);
                return default;
            }
#endif
        }


        /// <summary>
        /// 加载资源  如果资源包没有加载会先加载资源包
        /// </summary>
        public void LoadAsset<T>(string path, string bundleName, string assetName, Action<T> callback) where T : UnityEngine.Object
        {

#if USER_SOURCEC_RES
            string name = string.Format("ResAssetBundle/{0:s}{1:s}", path, assetName);
            StartCoroutine(LoadFromUnityResourceRequest(name, callback));
#else
            //已经加载过了
            if (m_LoadedAssetBundle.TryGetValue(bundleName, out AssetBundle bundle))
            {
                T obj = bundle.LoadAsset<T>(assetName);
                callback(obj);
            }
            else
            {
                void action(AssetBundle assetBundle)
                {
                    T obj = assetBundle.LoadAsset<T>(assetName);
                    callback(obj);
                }

                LoadAssetBundle(bundleName, action);
            }
#endif
        }


        /// <summary>
        /// 加载资源包队列
        /// </summary>
        public void LoadQueue(Queue<string> needLoadQueue, Action<int> remain)
        {
            if (needLoadQueue.Count > 0)
            {
                string needLoadAssetName = needLoadQueue.Dequeue();

                //没有加载才加载
                if (m_LoadedAssetBundle.ContainsKey(needLoadAssetName))
                {
                    LoadQueue(needLoadQueue, remain);
                }
                else
                {
                    LoadAssetBundle(needLoadAssetName, (assetBundle) =>
                    {
                        remain?.Invoke(needLoadQueue.Count);
                        if (needLoadQueue.Count > 0) LoadQueue(needLoadQueue, remain);
                    });
                }
            }
            else
            {
                remain?.Invoke(needLoadQueue.Count);
            }
        }

        /// <summary>
        /// 加载资源包
        /// </summary>
        private void LoadAssetBundle(string bundleName, Action<AssetBundle> callBack)
        {
            LoadDependenceAssets(bundleName, callBack);
        }

        /// <summary>
        /// 加载依赖资源包
        /// </summary>
        private void LoadDependenceAssets(string bundleName, Action<AssetBundle> callback)
        {

            void dependenceAction(AssetBundleManifest manifest)
            {
                //获取所有的依赖资源包名
                string[] dependences = MainManifest.GetAllDependencies(bundleName);

                //没有依赖
                if (dependences.Length <= 0)
                {
                    //加载目标资源包
                    LoadAssetAsync(bundleName, callback);
                }
                else
                {
                    //完成加载的数量
                    int completeLoadNum = 0;
                    int length = dependences.Length;

                    for (int i = 0; i < length; i++)
                    {
                        string abName = dependences[i];

                        //是否已经加载过了
                        if (m_LoadedAssetBundle.ContainsKey(abName))
                        {
                            //完成加载的数量
                            completeLoadNum++;

                            //是否加载了所有的依赖资源
                            if (completeLoadNum == dependences.Length)
                            {
                                //加载目标资源包
                                LoadAssetAsync(bundleName, callback);
                            }
                        }
                        else
                        {
                            //加载依赖资源
                            LoadAssetAsync(abName, (assetBundle) =>
                             {
                                 //完成加载的数量
                                 completeLoadNum++;

                                 //是否加载了所有的依赖资源
                                 if (completeLoadNum == dependences.Length)
                                 {
                                     //加载目标资源包
                                     LoadAssetAsync(bundleName, callback);
                                 }
                             });
                        }
                    }
                }
            }

            LoadAssetBundleManifest(dependenceAction);
        }


        /// <summary>
        /// 加载AssetBundleManifest
        /// </summary>
        /// <param name="action"></param>
        private void LoadAssetBundleManifest(Action<AssetBundleManifest> action)
        {
            if (MainManifest == null)
            {
                LoadAssetAsync(MainManifestName, (assetBundle)=> 
                {
                    if (MainManifest != null)
                    {
                        action(MainManifest);
                        return;
                    } 

                    MainManifest = assetBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
                    action(MainManifest);
                    assetBundle.Unload(false);
                });
            }
            else
            {
                action(MainManifest);
            }
        }
        #endregion

        #region Load Tool
        /// <summary>
        /// 开启异步加载
        /// </summary>
        /// <param name="bundleName"></param>
        /// <param name="callback"></param>
        private void LoadAssetAsync(string bundleName, Action<AssetBundle> callback)
        {
            if (!m_LoadedAssetBundle.ContainsKey(bundleName))
            {
                StartCoroutine(LoadFromUnityWebRequest(bundleName, callback));
            }
        }

        /// <summary>
        /// 使用协程通过UnityWebRequest加载
        /// </summary>
        private IEnumerator LoadFromUnityWebRequest(string bundleName, Action<AssetBundle> callback)
        {
            string path = (m_DownladedAssetInfo.Contains(bundleName) ? downloadResPath_Read : NativeResPath) + bundleName;

            using (UnityWebRequest request = UnityWebRequestAssetBundle.GetAssetBundle(path))
            {
                yield return request.SendWebRequest();

                ///储存资源包
                if (!m_LoadedAssetBundle.TryGetValue(bundleName, out AssetBundle bundle))
                {
                    bundle = DownloadHandlerAssetBundle.GetContent(request);
                    m_LoadedAssetBundle.Add(bundleName, bundle);
                }
                else
                {
                    Debug.Log(bundleName + "已加载");
                }
               
                callback(bundle);
            } 
        }

        /// <summary>
        /// 使用协程通过ResourceRequest加载本地资源
        /// </summary>
        private IEnumerator LoadFromUnityResourceRequest<T>(string path, Action<T> callback) where T : UnityEngine.Object
        {
            ResourceRequest rr = Resources.LoadAsync<T>(path);
            yield return rr;
            callback?.Invoke((T)rr.asset);
        }
        #endregion

        #region Download Res info tool
        /// <summary>
        /// 读取下载的资源包信息
        /// </summary>
        private void ReadDownloadAssetInfo()
        {
            m_DownladedAssetInfo = new List<string>();

            if (File.Exists(downdladeAssetInfoFilePath))
            {
                StreamReader sr = new StreamReader(downdladeAssetInfoFilePath);

                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    if (!string.IsNullOrEmpty(line))
                    {
                        m_DownladedAssetInfo.Add(line);
                    }
                }

                sr.Close();
            }
        }

        /// <summary>
        /// 保存下载的资源包信息
        /// </summary>
        public void SaveDownloadAssetInfo()
        {
            StreamWriter sw = new StreamWriter(downdladeAssetInfoFilePath, false, Encoding.UTF8);

            for (int i = 0; i < m_DownladedAssetInfo.Count; i++)
            {
                sw.WriteLine(m_DownladedAssetInfo[i]);
            }

            sw.Flush();
            sw.Close();
        }


        /// <summary>
        /// 删除下载的文件和下载的信息
        /// </summary>
        public bool DeleteDownload()
        {
            for (int i = 0; i < m_DownladedAssetInfo.Count; i++)
            {
                //当前文件夹下有该文件
                string filePath = downloadResPath_Write + m_DownladedAssetInfo[i];

                if (File.Exists(filePath))
                {
                    //Debug.Log("删除文件  " + filePath);
                    File.Delete(filePath);
                }
            }

            m_DownladedAssetInfo.Clear();

            SaveDownloadAssetInfo();

            return true;
        }
        #endregion



        public bool Unzip(byte[] ZipByte, Action<float> position)
        {
            return Unzip(downloadResPath_Write, ZipByte, null, position);
        }

        /// <summary>
        /// 解压
        /// </summary>
        /// <param name="path">存放路径</param>
        /// <param name="ZipByte">压缩包数据</param>
        /// <param name="password">密码</param>
        /// <param name="position">进度</param>
        public bool Unzip(string path, byte[] ZipByte, string password, Action<float> position)
        {
            FileStream fs = null;
            ZipInputStream zipStream = null;
            ZipEntry ent = null;

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            try
            {
                //直接使用 将byte转换为Stream，省去先保存到本地在解压的过程
                Stream stream = new MemoryStream(ZipByte);
                zipStream = new ZipInputStream(stream);

                if (!string.IsNullOrEmpty(password))
                {
                    zipStream.Password = password;
                }

                while ((ent = zipStream.GetNextEntry()) != null)
                {
                    if (!string.IsNullOrEmpty(ent.Name))
                    {
                        string filePath = Path.Combine(path, ent.Name);

                        //如果是一个新的文件路径　这里需要创建这个文件路径  
                        if (filePath.EndsWith("/"))
                        {
                            if (!Directory.Exists(filePath))
                            {
                                Directory.CreateDirectory(filePath);
                            }
                        }
                        else
                        {
                            //当前文件夹下有该文件  删掉  重新创建  
                            if (File.Exists(filePath))
                            {
                                File.Delete(filePath);
                            }

                            fs = File.Create(filePath);

                            int size = 2048;
                            byte[] data = new byte[2048];
                            //每次读取2MB  直到把这个内容读完  
                            while (true)
                            {
                                size = zipStream.Read(data, 0, data.Length);
                                //小于0， 也就读完了当前的流  
                                if (size > 0)
                                {
                                    fs.Write(data, 0, size);
                                }
                                else
                                {
                                    break;
                                }
                            }

                            fs.Close();

                            m_DownladedAssetInfo.Add(ent.Name);
                        }
                    }

                    if (position != null)
                    {
                        float jd = zipStream.Position * 1.0f / ZipByte.Length;

                        jd = Mathf.Min(jd, 0.99f);

                        position(jd);
                    }
                }

                position?.Invoke(1f);

            }
            catch (Exception e)
            {
                Debug.Log(e.ToString());
            }
            finally
            {
                if (fs != null)
                {
                    fs.Close();
                    fs.Dispose();
                }

                if (zipStream != null)
                {
                    zipStream.Close();
                    zipStream.Dispose();
                }

                if (ent != null)
                {
                    ent = null;
                }

                GC.Collect();
            }

            return true;
        }

        public bool DeleteObsoleteFile()
        {
            //删除不需要的文件
            string deleteFilePath = downloadResPath_Write + "/deleteFile.txt";
            if (File.Exists(deleteFilePath))
            {
                StreamReader sr = new StreamReader(deleteFilePath, Encoding.Default);

                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    if (!string.IsNullOrEmpty(line))
                    {
                        //删除下载信息
                        if (m_DownladedAssetInfo.Contains(line))
                        {
                            m_DownladedAssetInfo.Remove(line);
                        }

                        //当前文件夹下有该文件
                        string filePath = downloadResPath_Write + line;

                        if (File.Exists(filePath))
                        {
                            //Debug.Log("删除文件  " + filePath);
                            File.Delete(filePath);
                        }
                    }
                }

                sr.Close();
            }

            return true;
        }


        public override void Shutdown()
        {
            
        }
        

    }
}


