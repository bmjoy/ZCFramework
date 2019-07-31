using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace ZCFrame
{
  

    [CustomEditor(typeof(LocalizationComponent), true)]
    public class LocalizationInspector : Editor
    {

        private SerializedProperty LanguageInfoList;
     
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            serializedObject.Update();

            GUILayout.Space(10);
            GUILayout.BeginHorizontal("box");
            GUILayout.Label("语言编码" );
            GUILayout.Label("字体");
            GUILayout.EndHorizontal();

            for (int i = 0; i < LanguageInfoList.arraySize; i++)
            {
                SerializedProperty info = LanguageInfoList.GetArrayElementAtIndex(i);
                var codeProperty = info.FindPropertyRelative("_LanguageCode");
                var fontProperty = info.FindPropertyRelative("_font");
                fontProperty.objectReferenceValue = EditorGUILayout.ObjectField(codeProperty.stringValue, fontProperty.objectReferenceValue, typeof(Font), true) as Font;
            }

            serializedObject.ApplyModifiedProperties();
            //重绘
            Repaint();
        }

        private void OnEnable()
        {

            LanguageInfoList = serializedObject.FindProperty("LanguageInfoList");
            int length = LocalizationEditorInformation.Instance.Codes.Length;
            LanguageInfoList.arraySize = length;

            for (int i = 0; i < length; i++)
            {
                SerializedProperty info = LanguageInfoList.GetArrayElementAtIndex(i);
                var codeProperty = info.FindPropertyRelative("_LanguageCode");
                codeProperty.stringValue = LocalizationEditorInformation.Instance.Codes[i];
            }

            serializedObject.ApplyModifiedProperties();
        }



    }
}


