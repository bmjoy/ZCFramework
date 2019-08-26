using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class Menu
{

   

    [MenuItem("点趣工具/Settings")]
    public static void Settings()
    {
       // SettingsWindow win = (SettingsWindow)EditorWindow.GetWindow(typeof(SettingsWindow));
    }

    //[MenuItem("点趣工具/AssetBundles Tool")]
    //public static void ExportAB()
    //{
    //    AssetBundleWindow win = EditorWindow.GetWindow<AssetBundleWindow>();
    //    win.titleContent = new GUIContent("资源打包");

    //}
}
