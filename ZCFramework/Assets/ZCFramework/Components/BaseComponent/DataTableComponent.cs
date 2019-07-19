
namespace ZCFrame
{
    
    /// <summary>
    /// 数据表组件
    /// </summary>
    public class DataTableComponent : ZCBaseComponent
    {
        /// <summary>
        /// 数据表管理器
        /// </summary>
        public DataTableManager DataTableManager { get; private set; }

        protected override void OnAwake()
        {
            base.OnAwake();
            DataTableManager = new DataTableManager();
        }

        /// <summary>
        /// 预加载表格
        /// </summary>
        public void LoadDataTableAsync()
        {
            DataTableManager.LoadDataTableAsync();
        }

        public override void Shutdown()
        {
            DataTableManager.Clear();
        }
    }

}

