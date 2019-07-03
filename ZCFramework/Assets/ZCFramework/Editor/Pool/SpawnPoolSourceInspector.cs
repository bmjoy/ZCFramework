using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace ZCFrame
{
    [CustomEditor(typeof(ObjectPoolSource))]
    public class SpawnPoolSourceInspector : Editor
    {
        private ReorderableList _ObjectInfoArray;
    
    
 
        //在这里方法中就可以绘制面板。
        public override void OnInspectorGUI() 
        {
            serializedObject.Update();
            //自动布局绘制列表
            _ObjectInfoArray.DoLayoutList();
            serializedObject.ApplyModifiedProperties();
        }
    
        private void OnEnable()
        {
        
            _ObjectInfoArray = new ReorderableList(serializedObject, serializedObject.FindProperty("_ObjectInfoList"), true, true, true,true);
        
            //自定义列表名称
            _ObjectInfoArray.drawHeaderCallback = rect => { GUI.Label(rect, "per-prefab pool options"); };
        
            //定义元素的高度
            _ObjectInfoArray.elementHeight = 60;
        
            //自定义绘制列表元素
            _ObjectInfoArray.drawElementCallback = (rect, index, active, focused) =>
            {
                //根据index获取对应元素
                SerializedProperty item = _ObjectInfoArray.serializedProperty.GetArrayElementAtIndex(index);
                rect.height -= 4;
                rect.y += 2;
                EditorGUI.PropertyField(rect, item, new GUIContent("Index " + index));
            };

            _ObjectInfoArray.onRemoveCallback = list =>
            {
                if (EditorUtility.DisplayDialog("Warnning", "Do you want to remove this element?", "Remove", "Cancel"))
                {
                    ReorderableList.defaultBehaviours.DoRemoveButton(list);
                }
            };
            
        }
    
    }

}


