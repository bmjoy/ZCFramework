using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace ZCFrame
{

    /// <summary>
    /// 数据表管理器
    /// </summary>
    public class DataTableManager :ManagerBase
    {


        /// <summary>
        /// 游戏关卡表
        /// </summary>
        public GameLevelDBModel GameLevelDBModel { get; private set; }

        public DataTableManager()
        {
            InitDBModel();
        }

        /// <summary>
        /// 初始化表格
        /// </summary>
        private void InitDBModel()
        {
            //每个表都new
            GameLevelDBModel = new GameLevelDBModel();
            //load完毕
        }

        /// <summary>
        /// 异步加载表格
        /// </summary>
        public void LoadDataTableAsync()
        {
            Task.Factory.StartNew(LoadDataTable);
        }


        public void LoadDataTable()
        {
            //可能在lua中读取load，只load自己需要的表
            //每个表都LoadData
            //ChapterDBModel.LoadData();
            GameLevelDBModel.LoadData();
            //Sys_CodeDBModel.LoadData();
            //Sys_EffectDBModel.LoadData();
            //Sys_PrefabDBModel.LoadData();
            //Sys_SoundDBModel.LoadData();
            //Sys_StorySoundDBModel.LoadData();
            //Sys_UIFormDBModel.LoadData();
            //LocalizationDBModel.LoadData();

            //所有表加载完毕
            GameEntry.Event.CommonEvent.Dispatch(SysEventId.LoadDataTableComplete);
        }

        public void Clear()
        {
            //每个表都clear
            GameLevelDBModel.Clear();
        }

    }
}


