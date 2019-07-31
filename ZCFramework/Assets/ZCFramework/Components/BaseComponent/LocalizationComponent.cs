using System.Collections.Generic;
using UnityEngine;
using System;



namespace ZCFrame
{

    /// <summary>
    /// 本地化（多语言）组件
    /// </summary>
    public class LocalizationComponent : ZCBaseComponent
    {

        [SerializeField, Header("本地化文件")]
        private TextAsset  m_localization = null;

#if UNITY_EDITOR
        public TextAsset Localization { get { return m_localization; } }
#endif

        [HideInInspector]
        public List<LanguageInfo> LanguageInfoList = new List<LanguageInfo>();

        private LocalizationManager m_LocalizationManager;

        

        protected override void OnAwake()
        {
            base.OnAwake();
            m_LocalizationManager = new LocalizationManager(m_localization, LanguageInfoList);
        }


        /// <summary>
        /// 切换语言
        /// </summary>
        /// <param name="languageCode"></param>
        public void SetLanguage(LanguageCode code)
        {
            m_LocalizationManager.SetLanguage(code);
        }

        /// <summary>
        /// 注册语言变更监听
        /// </summary>
        /// <param name="callback"></param>
        public void RegisterLanguageListener(Action callback)
        {
            m_LocalizationManager.RegisterLanguageListener(callback);
        }

        /// <summary>
        /// 移除语言变更监听
        /// </summary>
        /// <param name="callback"></param>
        public void RemoveLanguageListener(Action callback)
        {
            m_LocalizationManager.RemoveLanguageListener(callback);
        }


        public string GetTermString(string term)
        {
            return m_LocalizationManager.GetTermString(term);
        }

        public Font GetFont()
        {
            return m_LocalizationManager.GetFont();
        }


        public override void Shutdown()
        {
            m_LocalizationManager.Dispose();
        }

    }

}



