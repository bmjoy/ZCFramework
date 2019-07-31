using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZCFrame
{
    public class LocalizationManager :ManagerBase, IDisposable
    {

        private const string LocalizatioCode = "LanguageCode";

        private List<LanguageInfo> m_LanguageInfoList = null;
        private Dictionary<string, TermData> termDataDic;
        private int LocalizatioCodeIndex = 0;

        /// <summary>
        /// 语言变更事件
        /// </summary>
        private event Action LanguageEvent = null;



        public LocalizationManager(TextAsset localization, List<LanguageInfo> LanguageInfoList)
        {

            m_LanguageInfoList = LanguageInfoList;

            LanguageCode code;

            if (PlayerPrefs.HasKey(LocalizatioCode))
            {
                string codeName = PlayerPrefs.GetString(LocalizatioCode, "en");
                code = (LanguageCode)Enum.Parse(typeof(LanguageCode), codeName);
            }
            else
            {
                code =  GetSystemLanguageCode();
            }

            LocalizatioCodeIndex = GetCodeIndex(code);

            ReadCSV(localization);
        }


        private void ReadCSV(TextAsset localization)
        {
            termDataDic = new Dictionary<string, TermData>();
            string[] csvData = localization.text.Split('\n');
            int length = csvData.Length;

            for (int i = 1; i < length; i++)
            {
                string[] line = csvData[i].Split(',');
                string term = line[0];

                string[] languages = new string[line.Length - 1];
                Array.Copy(line, 1, languages, 0, languages.Length);

                TermData termData = new TermData(term, languages);

                if (!termDataDic.ContainsKey(term))
                termDataDic.Add(term, termData);
            }
        }


        /// <summary>
        /// 设置语言
        /// </summary>
        /// <param name="code"></param>
        public void SetLanguage(LanguageCode code)
        {
            LocalizatioCodeIndex = GetCodeIndex(code);
            PlayerPrefs.SetString(LocalizatioCode, code.ToString());
            LanguageEvent?.Invoke();
        }


        /// <summary>
        /// 注册语言变更监听
        /// </summary>
        /// <param name="callback"></param>
        public void RegisterLanguageListener(Action callback)
        {
            LanguageEvent += callback;
        }

        /// <summary>
        /// 移除语言变更监听
        /// </summary>
        /// <param name="callback"></param>
        public void RemoveLanguageListener(Action callback)
        {
            LanguageEvent -= callback;
        }


        public string GetTermString(string term)
        {
            if (termDataDic.TryGetValue(term, out TermData tData))
            return tData.Languages[LocalizatioCodeIndex];

            return string.Empty;
        }

        public Font GetFont()
        {
            return m_LanguageInfoList[LocalizatioCodeIndex].font;
        }


        private int GetCodeIndex(LanguageCode code)
        {
            int codeIndex = 0;

            for (int i = 0; i < m_LanguageInfoList.Count; i++)
            {
                if (code.ToString().Equals(m_LanguageInfoList[i].LanguageCodeName))
                {
                    codeIndex = i;
                    break;
                }
            }

            return codeIndex;
        }


        /// <summary>
        /// 获取系统语言编码
        /// </summary>
        private LanguageCode GetSystemLanguageCode()
        {
            switch (Application.systemLanguage)
            {
                case SystemLanguage.Chinese:
                case SystemLanguage.ChineseSimplified: return LanguageCode.zh_CN;
                case SystemLanguage.ChineseTraditional: return LanguageCode.zh_TW;
                case SystemLanguage.English: return LanguageCode.en;
                case SystemLanguage.French: return LanguageCode.fr;
                case SystemLanguage.Japanese: return LanguageCode.ja;
                case SystemLanguage.Russian: return LanguageCode.ru;
                case SystemLanguage.Thai: return LanguageCode.th;
                default: return LanguageCode.en;
            };
        }

        public void Dispose()
        {
            m_LanguageInfoList.Clear();
            termDataDic.Clear();
            LanguageEvent = null;
        }
    }
}

