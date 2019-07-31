using System;
using UnityEngine;


namespace ZCFrame
{

#if UNITY_EDITOR
    public class LocalizationEditorInformation
    {


        public string[] Codes = null;
        public string[] Terms { get; private set; }
        public int[] OptionValues { get; private set; }


        public static LocalizationEditorInformation Instance
        {
            get
            {
                if (_Instance == null)
                 _Instance = new LocalizationEditorInformation();
               
                return _Instance;
            }
        }
      
        private  static LocalizationEditorInformation _Instance = null;

        public LocalizationEditorInformation()
        {

            TextAsset csv = GameObject.Find("GameEntry/Components/Localization").GetComponent<LocalizationComponent>().Localization;
            string[] csvData = csv.text.Split('\n');

            string row = csvData[0];
            string[] label = row.Split(',');

            Codes = new string[label.Length - 1];
            Array.Copy(label, 1, Codes, 0, Codes.Length);
           
            int length = csvData.Length;
            Terms = new string[length - 1];
            OptionValues = new int[length - 1];

            for (int i = 1; i < length; i++)
            {
                int index = i - 1;

                OptionValues[index] = index;
                string term = csvData[i].Split(',')[0];
                Terms[index] = term;
            }
        }

    }
#endif

}


