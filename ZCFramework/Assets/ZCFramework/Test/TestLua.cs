
using UnityEngine;
using ZCFrame;

public class TestLua : MonoBehaviour
{

    void Start()
    {
        
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            GameEntry.UI.OpenUIForm<LuaForm>(1);
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            GameEntry.UI.CloseUIForm(1);
        }
    }
}
