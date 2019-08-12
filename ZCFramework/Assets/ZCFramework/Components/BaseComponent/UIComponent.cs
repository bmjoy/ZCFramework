using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace ZCFrame
{
    
    /// <summary>
    /// UI组件
    /// </summary>
    public class UIComponent : ZCBaseComponent , IUpdateComponent
    {

        [SerializeField, Header("标准分辨率宽度")]
        private int m_StandardWidth = 1280;
        
        [SerializeField, Header("标准分辨率高度")]
        private int m_StandardHeight = 720;

        [Header("UI摄像机")]
        public Camera UICamera;
        
        [SerializeField,Header("根画布")]
        private Canvas m_UIRootCanvas;

        [SerializeField, Header("根画布的缩放")]
        private CanvasScaler m_UIRootCanvasScaler = null;

        [SerializeField, Header("UI分组")]
        private UIGroup[] m_Groups = null;

        private Dictionary<byte, UIGroup> m_UIGroupDic;

        private float m_Standard = 0;
        private float m_Curr = 0;

        private UIManager m_UIManager;


        [SerializeField, Header("释放间隔（秒）")]
        private float m_ClearInterval = 120f;
        /// <summary>
        /// 下次运行时间
        /// </summary>
        private float m_NextRunTime = 0f;
        /// <summary>
        /// UI关闭后过期时间
        /// </summary>
        public float UIExpire = 120f;


        protected override void OnAwake()
        {
            base.OnAwake();

            m_Standard = m_StandardWidth / (float)m_StandardHeight;
            m_Curr = Screen.width / (float)Screen.height;
            NormalFormCanvasScaler();

            m_UIGroupDic = new Dictionary<byte, UIGroup>();

            int len = m_Groups.Length;
            for (int i = 0; i < len; i++)
            {
                UIGroup group = m_Groups[i];
                m_UIGroupDic[group.Id] = group;
            }

            m_UIManager = new UIManager();

            m_NextRunTime = Time.time + m_ClearInterval;
            GameEntry.RegisterUpdateComponent(this);
        }
        
        #region UI适配
        /// <summary>
        /// LoadingForm适配缩放
        /// </summary>
        public void LoadingFormCanvasScaler()
        {
            m_UIRootCanvasScaler.matchWidthOrHeight = m_Curr > m_Standard ? 0 : m_Standard - m_Curr;
          
        }

        /// <summary>
        /// FullForm适配缩放
        /// </summary>
        public void FullFormCanvasScaler()
        {
            m_UIRootCanvasScaler.matchWidthOrHeight = 1;
        }

        /// <summary>
        /// NormalForm适配缩放
        /// </summary>
        public void NormalFormCanvasScaler()
        {
            m_UIRootCanvasScaler.matchWidthOrHeight = m_Curr >= m_Standard ? 1 : 0;
        }

        #endregion

        /// <summary>
        /// 根据UI分组编号获取UI分组
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public UIGroup GetUIGroup(byte id)
        {
            m_UIGroupDic.TryGetValue(id, out UIGroup group);
            return group;
        }

        /// <summary>
        /// 打开UI窗体
        /// </summary>
        /// <typeparam name="T">窗体类型</typeparam>
        /// <param name="uiFormId">窗体编号</param>
        internal void OpenUIForm<T>(int uiFormId,object userData = null) where T : UIFormBase, new()
        {
            m_UIManager.OpenUIForm<T>(uiFormId, userData);
        }


        /// <summary>
        /// 关闭UI窗体(根据UIFormBase)
        /// </summary>
        /// <param name="formBase"></param>
        public void CloseUIForm(UIFormBase formBase)
        {
            m_UIManager.CloseUIForm(formBase);
        }


        /// <summary>
        /// 关闭UI窗体(根据UIFormId)
        /// </summary>
        /// <param name="formBase"></param>
        public void CloseUIForm(int uiformId)
        {
            m_UIManager.CloseUIForm(uiformId);
        }


        public void OnUpdate()
        {
            if (Time.time > m_NextRunTime)
            {
                m_NextRunTime = Time.time + m_ClearInterval;
                //释放UI对象池
                m_UIManager.CheckClear();
            }
        }


        public override void Shutdown()
        {
            m_UIManager.Dispose();
        }
    }
}


