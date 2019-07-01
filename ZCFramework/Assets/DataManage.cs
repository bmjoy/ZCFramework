using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;


public class DataManage 
{
	Dictionary<Type, System.Object> dataBase_forever = null;
	Dictionary<Type, System.Object> dataBase_zeroHour = null;


	private static DataManage dataManage;
	public static DataManage Manage
	{
		get
		{
			if(dataManage == null)
			{
				dataManage = new DataManage ();
			}

			return dataManage;
		}
	}

	private DataManage ()
	{
		dataBase_forever = new Dictionary<Type, System.Object> ();
		dataBase_zeroHour = new Dictionary<Type, System.Object> ();

		initForeverTable ();
	}

	/// <summary>
	/// 销毁方法
	/// </summary>
	public void destroy ()
	{
		clearForeverDataTable ();
		clearZeroHourDataTable ();

		dataManage = null;
	}



	/// <summary>
	/// 创建一张零时有效的表，这张表会在loading的时候清理掉
	/// </summary>
	/// <param name="tableName">json文件名.</param>
	/// <param name="tablePath">json文件路径</param>
	/// <param name="tableBundleNma">joson文件所在资源包名</param>
	/// <typeparam name="T">数据模版类</typeparam>
	public void createZeroHourDataTable<T> (string tableName) where T : Data
	{
		DataTable<T> table = new DataTable<T> (tableName);

		if(table != null)
		{
			dataBase_zeroHour.Add (typeof(T), table);
		}
		else
		{
			Debug.Log ("创建数据表失败！");
		}
	}

	/// <summary>
	/// 创建一张永久有效的表
	/// </summary>
	/// <param name="tableName">json文件名.</param>
	/// <param name="tablePath">json文件路径</param>
	/// <param name="tableBundleNma">joson文件所在资源包名</param>
	/// <typeparam name="T">数据模版类</typeparam>
	public void createForeverDataTable<T> (string tableName) where T : Data
	{
		DataTable<T> table = new DataTable<T> (tableName);

		if(table != null && !dataBase_forever.ContainsKey(typeof(T)))
		{
			dataBase_forever.Add (typeof(T), table);
		}
		else
		{
			Debug.Log ("创建数据表失败！");
		}
	}

	/// <summary>
	/// 清理永久数据表
	/// </summary>
	public void clearForeverDataTable ()
	{
		if (dataBase_forever != null) 
		{
			dataBase_forever.Clear ();
		}
	}

	/// <summary>
	/// 清除临时数据表
	/// </summary>
	public void clearZeroHourDataTable ()
	{
		if (dataBase_zeroHour != null) 
		{
			dataBase_zeroHour.Clear ();
		}
	}

	/// <summary>
	/// 获取数据表
	/// </summary>
	/// <returns>返回数据表</returns>
	/// <typeparam name="T">数据类型模版</typeparam>
	public DataTable<T> getTable<T> () where T : Data
	{
		Type type = typeof(T);
		
		if(dataBase_forever != null && dataBase_forever.ContainsKey(type))
		{
			return (DataTable<T>)dataBase_forever [type];
		}
 		else if(dataBase_zeroHour != null && dataBase_zeroHour.ContainsKey(type))
		{
			return (DataTable<T>)dataBase_zeroHour [type];
		}

		return null;
	}



	/// <summary>
	/// 初始永久数据表
	/// </summary>
	public bool initForeverTable ()
	{
		createForeverDataTable<PoolData> ("tmpl_pool");
		return true;
	}

}



public class DataTable<T> where T : Data
{
	/// <summary>
	/// 数据字典
	/// </summary>
	private Dictionary<int, T> dataTable = null;

	/// <summary>
	/// 构造函数
	/// </summary>
	/// <param name="name">json文件名</param>
	/// <param name="path">json文件路径</param>
	/// <param name="bundleNma">json文件所在资源包</param>
	public DataTable (string name)
	{
		dataTable = new Dictionary<int, T> ();

		try
		{
			TextAsset tt = Resources.Load<TextAsset>(string.Format("Data/{0:s}", name));

			string json = tt.text;

			if (!string.IsNullOrEmpty(json))
			{
				DataArray<T> data = JsonUtility.FromJson<DataArray<T>>(json);

				for(int i = 0; i < data.dataArray.Length; i++)
				{
					if(!dataTable.ContainsKey(data.dataArray[i].id))
					{
						dataTable.Add(data.dataArray[i].id, data.dataArray[i]);
					}
				}
			}

		}
		catch (Exception e)
		{
			Debug.LogError(name + "解析失败！   " +e.Message);
		}
	}

	/// <summary>
	/// 析构函数
	/// </summary>
	~DataTable ()
	{
		if (dataTable != null)
		{
			dataTable.Clear ();
		}

		dataTable = null;
	}


	/// <summary>
	/// 获取数据表
	/// </summary>
	public Dictionary<int, T> table
	{
		get{ return dataTable; }
	}

	/// <summary>
	/// 通过数据id获取数据
	/// </summary>
	public T getData (int modelId)
	{
		if(dataTable.ContainsKey(modelId))
		{
			return dataTable [modelId];
		}

		Debug.Log (string.Format ("没有找到 {0:s} {1:d} 的数据", typeof(T).ToString (), modelId));

		return default(T);
	}
}

[Serializable]
public class DataArray <T> where T : Data
{
	public T[] dataArray = null;
}

/// <summary>
/// 数据父类
/// </summary>
[Serializable]
public class Data
{
	/// <summary>
	/// 模版id
	/// </summary>
	public int id = 0;
	/// <summary>
	/// 名称
	/// </summary>
	public string name = "";
}
	



/// <summary>
/// 对象池表
/// </summary>
[Serializable]
public class PoolData : Data
{
	
	/// <summary>
	/// 路径
	/// </summary>
	public string path = "";
	/// <summary>
	/// 预制体数量
	/// </summary>
	public int prefabCount = 0;
	

}






	
