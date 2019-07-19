using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using ZCFrame;


public class TestPool : MonoBehaviour
{

    private Role role = null;
    
    
    void Start ()
    {
        GameEntry.Pool.SetClassObjectResideCount<Person>(2);
        GameEntry.Pool.SetClassObjectResideCount<Animal>(3);


        Variable<List<int>> xx = Variable<List<int>>.Alloc();
        Debug.Log(xx.Value.Count);
        //Dictionary<int, GameObject> dic = new Dictionary<int, GameObject>();
        //for (int i = 0; i < 5; i++)
        //{
        //    GameObject go = new GameObject();
        //    go.name = i + "";
        //    dic.Add(i, go);
        //}


    }


    void Update()
    {
        
        
        if (Input.GetKeyDown(KeyCode.P))
        {
            Person p = GameEntry.Pool.DequeueClassObject<Person>();
            p.Init(1,"zhang");
            Debug.Log(p.Age);
            Debug.Log(p.Name);
            StartCoroutine(EnqueueClassObject(p, 5));

            Animal a = GameEntry.Pool.DequeueClassObject<Animal>();
            a.Init(1, "cat");
            Debug.Log(a.Age);
            Debug.Log(a.Name);
            StartCoroutine(EnqueueClassObject(a, 5));
        }
        
        
        if (Input.GetKeyDown(KeyCode.A))
        {
            role = GameEntry.Pool.GameObjectSpawn<Role>(ObjectTag.Role);
        }
        
        if (Input.GetKeyDown(KeyCode.B))
        {
            GameEntry.Pool.GameObjectDespawn(role);
        }
        
        if (Input.GetKeyDown(KeyCode.C))
        {
            Enemy enemy = GameEntry.Pool.GameObjectSpawn<Enemy>(ObjectTag.Enemy);
        }
        
        if (Input.GetKeyDown(KeyCode.D))
        {
            Enemy enemy = GameEntry.Pool.GameObjectSpawn<Enemy>(ObjectTag.Enemy1);
        }
        
    }
    
    
    IEnumerator EnqueueClassObject(object obj, float time)
    {
        yield return new WaitForSeconds(time);
        GameEntry.Pool.EnqueueClassObject(obj);
    }
    
}


class Person
{
    public int Age;
    public string Name;
    public void Init(int age, string name)
    {
        Age = age;
        Name = name;
    }
}

class Animal
{
    public int Age;
    public string Name;
    public void Init(int age, string name)
    {
        Age = age;
        Name = name;
    }
}


public class Role : ObjectBase
{
    
}


public class Enemy : ObjectBase
{
    
}