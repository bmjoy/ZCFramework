using System;
using UnityEditor;
using UnityEngine;

namespace ZCFrame
{
    /// <summary>
    /// 重绘自定义GFText组件
    /// </summary>
    [CustomEditor(typeof(GFText))]
    public class GFTextInspector : UnityEditor.UI.TextEditor
    {

        private SerializedProperty m_Term;
        private int popupindex;

        protected override void OnEnable()
        {
            base.OnEnable();

            m_Term = serializedObject.FindProperty("Term");

            int length = LocalizationEditorInformation.Instance.Terms.Length;

            for (int i = 0; i < length; i++)
            {
                if(m_Term.stringValue.Equals(LocalizationEditorInformation.Instance.Terms[i]))
                {
                    popupindex = i;
                    break;
                }
            }
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            serializedObject.Update();
           
            //一个选择框，每个选择框里表示一个Int数
            popupindex = EditorGUILayout.IntPopup("本地化语言KEY", popupindex, LocalizationEditorInformation.Instance.Terms, LocalizationEditorInformation.Instance.OptionValues);
            m_Term.stringValue = LocalizationEditorInformation.Instance.Terms[popupindex];
            serializedObject.ApplyModifiedProperties();
        }

    }
}



