using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZCFrame;

public class TestProcedure : MonoBehaviour {

    void Start()
    {
//
//       Variable<int> a = Variable<int>.Alloc(10);
//       
//       Variable<bool> b = Variable<bool>.Alloc();
//       StartCoroutine(ReleaseVar(a, b));
    }

    private IEnumerator ReleaseVar(Variable<int> a)
    {
      
        a.Retain();
        yield return new WaitForSeconds(1);
       a.Release();

       // Variable<int> b = Variable<int>.Alloc(100);
       // b.Release();


        Variable< List<int>> intList = Variable<List<int>>.Alloc();
       // int x = GameEntry.Pool.DequeueClassObject<int>();
        List<int> intList1 =  GameEntry.Pool.DequeueClassObject<List<int>>();
    }

    // Update is called once per frame
    void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.A))
       {
           Variable<int> a = Variable<int>.Alloc(10);
           StartCoroutine(ReleaseVar(a));
           a.Release();
         
       }
        
        
        if (Input.GetKeyDown(KeyCode.P))
        {
            Variable<int> b = Variable<int>.Alloc(10086);
            GameEntry.Procedure.SetData("zc", b);
            Debug.Log("当前的流程 = " + GameEntry.Procedure.CurrProcedure);
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
          
            GameEntry.Procedure.ChangeState(ProcedureState.Preload);
        }
    }

}
