using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ZCFrame;


public class TestUI : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            GameEntry.UI.OpenUIForm<TaskUIForm>(1);
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            GameEntry.UI.CloseUIForm(1);
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            GameEntry.UI.OpenUIForm<TaskUIForm>(2);
        }

    }
}


public class TaskUIForm : UIFormBase
{
    protected override void OnInit()
    {
       
        Debug.Log("初始化"); 
    }


    protected override void OnBeforeDestroy()
    {
        Debug.Log("释放结束");
    }
}
