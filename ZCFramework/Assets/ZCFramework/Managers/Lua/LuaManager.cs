﻿using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using XLua;

namespace ZCFrame
{
    public class LuaManager : ManagerBase
    {

        /// <summary>
        /// 全局的xlua引擎
        /// </summary>
        public static LuaEnv luaEnv;

        public void Init()
        {
            //1. 实例化 xlua引擎
            luaEnv = new LuaEnv();

//#if DISABLE_ASSETBUNDLE && UNITY_EDITOR
            //2.设置xLua的脚本路径
            luaEnv.DoString(string.Format("package.path = '{0}/?.bytes'", Application.dataPath + "/Download/xLuaLogic/"));
//#else
//            luaEnv.AddLoader(MyLoader);
//            //luaEnv.DoString(string.Format("package.path = '{0}/?.bytes'", Application.persistentDataPath));
//#endif
            DoString("require 'Main'");
        }


        /// <summary>
        /// 自定义的Loader
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        private byte[] MyLoader(ref string filePath)
        {
            string path = Application.persistentDataPath + "/" + filePath + ".lua";

            byte[] buffer = null;
            using (FileStream fs = new FileStream(path, FileMode.Open))
            {
                buffer = new byte[fs.Length];
                fs.Read(buffer, 0, buffer.Length);
            }

            buffer = SecurityUtil.Xor(buffer);

            buffer = System.Text.Encoding.UTF8.GetBytes(System.Text.Encoding.UTF8.GetString(buffer).Trim());
            return buffer;

        }


        private void DoString(string str)
        {
            luaEnv.DoString(str);
        }
    }

}


