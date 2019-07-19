using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace ZCFrame
{
    
    /// <summary>
    /// 资源组件
    /// </summary>
    public class ResourceComponent : ZCBaseComponent
    {
        public string LocalFilePath;

        protected override void OnAwake()
        {
            base.OnAwake();

            LocalFilePath = Application.dataPath;
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


        public override void Shutdown()
        {
            
        } 
    }
}


