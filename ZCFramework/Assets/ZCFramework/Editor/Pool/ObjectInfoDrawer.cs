using System.Diagnostics;
using UnityEngine;
using UnityEditor;
using Debug = UnityEngine.Debug;


namespace ZCFrame
{
    
    [CustomPropertyDrawer(typeof(ObjectPoolBasicInfo))]
    public class ObjectInfoDrawer : PropertyDrawer
    {

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            using (new EditorGUI.PropertyScope(position, label, property))
            {
                
                EditorGUIUtility.labelWidth = 60;
                position.height = EditorGUIUtility.singleLineHeight;
                
               // var idRect = new Rect(position) {width = 200};

                var entityRect = new Rect(position) { }; 
                
                var prefabRect = new Rect(position)
                {
                    y =  entityRect.y  + 30
                };
                
                var entityProperty = property.FindPropertyRelative("_ObjectTag");
                var prefabProperty = property.FindPropertyRelative("_Prefab");
                
                entityProperty.intValue = EditorGUI.Popup(entityRect,"对象标签",entityProperty.intValue , entityProperty.enumDisplayNames);
                prefabProperty.objectReferenceValue = EditorGUI.ObjectField(prefabRect, "对象预制体",prefabProperty.objectReferenceValue, typeof(GameObject), false);
            }
        }
        
    }
    
}









