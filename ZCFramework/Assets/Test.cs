using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Google.Protobuf;
using System.IO;

public class Test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //新建一个Person对象，并赋值
        Protobuf.Person p = new Protobuf.Person();
        p.Name = "IongX";
        p.Age = 22;
      
        p.NameList.Add("熊");
        p.NameList.Add("棒");
        p.NameList.Add("棒");
        //将对象转换成字节数组
        byte[] databytes = p.ToByteArray();

        //将字节数据的数据还原到对象中
        IMessage IMperson = new Protobuf.Person();
        Protobuf.Person p1 = new Protobuf.Person();
        p1 = (Protobuf.Person)IMperson.Descriptor.Parser.ParseFrom(databytes);


        using (MemoryStream stream = new MemoryStream())
        {
            //save the person to stream

            p.WriteTo(stream);
           // bytes = stream.ToArray();
        }

        //输出测试
        Debug.Log(p1.Name);
        Debug.Log(p1.Age);
        for (int i = 0; i < p1.NameList.Count; i++)
        {
            Debug.Log(p1.NameList[i]);
        }
    }


}
