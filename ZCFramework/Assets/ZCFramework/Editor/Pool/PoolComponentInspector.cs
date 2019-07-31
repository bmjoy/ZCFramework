using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace ZCFrame
{
    [CustomEditor(typeof(PoolComponent), true)]
    public class PoolComponentInspector : Editor
    {
        
        //关联PoolComponent组件上的ClearInterval（释放池中对象的时间间隔）字段的值
        private SerializedProperty m_ClearInterval;
        
        
         public override void OnInspectorGUI()
        {
            //base.OnInspectorGUI();
            serializedObject.Update();
            
            PoolComponent component = target as PoolComponent;
            
            //绘制游戏对象池源
            component.ObjectPoolSource = EditorGUILayout.ObjectField("游戏对象池源",component.ObjectPoolSource,typeof(ObjectPoolSource),true) as ObjectPoolSource;
            GUILayout.Space(10);
            
            //绘制滑动条
            int clearIntervalSlider = (int)EditorGUILayout.Slider("释放池中对象的间隔", m_ClearInterval.intValue, 10, 1800);
            if (clearIntervalSlider != m_ClearInterval.intValue)
            {
                component.ClearInterval = clearIntervalSlider;
            }

            //======================类对象池开始========================
            GUILayout.Space(10);
            GUILayout.BeginVertical("box");
            GUILayout.BeginHorizontal("box");
            GUILayout.Label("类名");
            GUILayout.Label("池中数量", GUILayout.Width(60));
            GUILayout.Label("池外数量", GUILayout.Width(60));
            GUILayout.Label("常驻数量", GUILayout.Width(60));
            GUILayout.EndHorizontal();

           
            if (component != null && component.PoolManager != null)
            {
                foreach (var item in component.PoolManager.ClassObjectPool.InspectorInDic)
                {
                    GUILayout.BeginHorizontal("box");
                    
                    
                    if (item.Key.BaseType.Equals(typeof(VariableBase)))    
                    {
                        GUILayout.Label(item.Key.UnderlyingSystemType.ToString());
                    }
                    else
                    {
                        GUILayout.Label(item.Key.Name);
                    }
                    
                    GUILayout.Label(item.Value.ToString(), GUILayout.Width(60));

                    int outCount = 0;
                    component.PoolManager.ClassObjectPool.InspectorOutDic.TryGetValue(item.Key, out outCount);
                    GUILayout.Label(outCount.ToString(), GUILayout.Width(60));
                    
                    int key = item.Key.GetHashCode();
                    byte resideCount = 0;
                    component.PoolManager.ClassObjectPool.ClassObjectCountDic.TryGetValue(key, out resideCount);
                    GUILayout.Label(resideCount.ToString(), GUILayout.Width(60));
                    GUILayout.EndHorizontal();
                }
            }
            GUILayout.EndVertical();
            //======================类对象池结束========================
            
            //======================游戏对象池计数开始========================
            GUILayout.Space(10);
            GUILayout.BeginVertical("box");
            GUILayout.BeginHorizontal("box");
            GUILayout.Label("游戏对象");
            GUILayout.Label("池中数量", GUILayout.Width(60));
            GUILayout.Label("池外数量", GUILayout.Width(60));
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            
            if (component != null && component.PoolManager != null)
            {
                foreach (KeyValuePair<ObjectTag, Queue<GameObject>> objetPool in component.PoolManager.GameObjectPool.ObjectPoolDic)
                {
                    
                    GUILayout.BeginHorizontal("box");
                    GUILayout.Label(objetPool.Key.ToString());
                    GUILayout.Label(objetPool.Value.Count.ToString(),GUILayout.Width(60));
                    
                    int outCount = 0;
                    component.PoolManager.GameObjectPool.ObjectPoolExternalNumDic.TryGetValue(objetPool.Key, out outCount);
                    GUILayout.Label(outCount.ToString(), GUILayout.Width(60));
                    GUILayout.EndHorizontal();
                }
            }
   
            
            //======================游戏对象池计数结束========================
            
            serializedObject.ApplyModifiedProperties();
            //重绘
            Repaint();
        }
        
        
        private void OnEnable()
        {
            //建立字段关联
            m_ClearInterval = serializedObject.FindProperty("ClearInterval");
            
            serializedObject.ApplyModifiedProperties();
        }
        
    }

}


