using UnityEngine;
using ZCFrame;

public class TestLocalization : MonoBehaviour
{
    
    void Start()
    {
        UIGroup group = GameEntry.UI.GetUIGroup(1);

        GameObject go = new GameObject();
        go.name = "TestLocalization";
        go.transform.SetParent(group.Group, false);

        GFText text = go.AddComponent<GFText>();
       text.Reset("Hall_LevelScore");
    }

    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            GameEntry.Localization.SetLanguage(LanguageCode.en);
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            GameEntry.Localization.SetLanguage(LanguageCode.ja);
        }
    }
}
