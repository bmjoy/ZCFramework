using UnityEngine;
using UnityEngine.UI;


namespace ZCFrame
{

    /// <summary>
    /// 自定义文本组件
    /// </summary>
    public class GFText : Text
    {
        
        [SerializeField,Header("本地化语言Key")]
        private string Term;


        protected override void Start()
        {
            base.Start();
            ResetTerm();

            if (GameEntry.Localization != null)
            GameEntry.Localization.RegisterLanguageListener(ResetTerm);
        }


        public void Reset(string term)
        {
            Term = term;
            ResetTerm();
        }


        private void ResetTerm()
        {
            if (string.IsNullOrEmpty(Term) ||  GameEntry.Localization == null) return;
       
            text = GameEntry.Localization.GetTermString(Term);
            font = GameEntry.Localization.GetFont();
        }


        protected override void OnDestroy()
        {
            base.OnDestroy();

            if(GameEntry.Localization != null)
            GameEntry.Localization.RemoveLanguageListener(ResetTerm);
        }

    }
}


