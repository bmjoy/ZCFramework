using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace ZCFrame
{
    
    /// <summary>
    /// Lua组件
    /// </summary>
    public class LuaComponent : ZCBaseComponent
    {

        private LuaManager m_LuaManager;

        /// <summary>
        /// 是否打印协议日志
        /// </summary>
        public bool DebugLogProto = false;


        protected override void OnAwake()
        {
            base.OnAwake();
            m_LuaManager = new LuaManager();
        }

        protected override void OnStart()
        {
            base.OnStart();
            LoadDataTableMS = new MMO_MemoryStream();
            m_LuaManager.Init();
        }

        public MMO_MemoryStream LoadDataTableMS
        {
            get;private set;
        }


        /// <summary>
        /// 加载数据表
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public MMO_MemoryStream LoadDataTable(string tableName)
        {

            byte[] buffer = GameEntry.Resource.GetFileBuffer(string.Format("{0}/download/DataTable/{1}.bytes", GameEntry.Resource.LocalFilePath, tableName));

            LoadDataTableMS.SetLength(0);
            LoadDataTableMS.Write(buffer, 0, buffer.Length);
            LoadDataTableMS.Position = 0;

            return LoadDataTableMS;
        }



        public override void Shutdown()
        {
            LoadDataTableMS.Dispose();
            LoadDataTableMS.Close();
        }
    } 

}


