using UnityEngine;


namespace ZCFrame
{
    [System.Serializable]
    public class LanguageInfo
    {

        [SerializeField]
        private string _LanguageCode = string.Empty;
        [SerializeField]
        private Font _font = null;


        public string LanguageCodeName
        {
            get
            {
                return _LanguageCode;
            }
        }

        public Font font
        {
            get
            {
                return _font;
            }
        }
    }
}



