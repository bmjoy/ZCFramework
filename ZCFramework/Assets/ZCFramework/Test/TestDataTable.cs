using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TestDataTable : MonoBehaviour
{


	
	// Update is called once per frame
	void Update ()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            ChapterDBModel cp = new ChapterDBModel();
            cp.LoadData();
            List<ChapterEntity> list = cp.GetList();
            for (int i = 0; i < list.Count; i++)
            {
                Debug.Log(list[i].ChapterName);
            }
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
           
            StartCoroutine(Waiter());
        }
    }

    private  IEnumerator Waiter()
    {
        GameLevelDBModel gl = new GameLevelDBModel();
        gl.LoadData();
      
        
        Debug.Log(gl.Get(2).Name);
        yield return new WaitForSeconds(1);
        //List<GameLevelEntity> list = gl.GetList();
        //for (int i = 0; i < list.Count; i++)
        //{
        //    Debug.Log(list[i].Name);
        //}
    }
}
