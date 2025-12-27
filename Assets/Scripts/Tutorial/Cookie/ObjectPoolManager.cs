using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 对象池管理器，用于管理游戏对象的创建和回收
/// </summary>
public class ObjectPoolManager : MonoBehaviour
{
    /// <summary>
    /// 存储所有对象池信息的静态列表
    /// </summary>
    public static List<PooledObjectInfo> ObjectPools = new List<PooledObjectInfo>();

    /// <summary>
    /// 从对象池中获取或创建一个游戏对象
    /// </summary>
    /// <param name="objectToSpawn">要创建的游戏对象预制体</param>
    /// <param name="parentTransform">父级变换组件</param>
    /// <returns>返回激活的游戏对象</returns>
    public static GameObject SpawnObject(GameObject objectToSpawn, Transform parentTransform)
    {
        // 查找或创建对应对象类型的对象池
        PooledObjectInfo pool = ObjectPools.Find(p => p.LookupString == objectToSpawn.name);

        if (pool == null)
        {
            pool = new PooledObjectInfo
            {
                LookupString = objectToSpawn.name,
            };
            
            ObjectPools.Add(pool);
        }

        // 尝试从对象池中获取未激活的对象
        GameObject go = pool.InactiveObjects.FirstOrDefault();
        if (go == null)
        {
            // 如果对象池中没有可用对象，则创建新对象
            go = Instantiate(objectToSpawn, parentTransform);
        }
        else
        {
            // 从对象池中移除并激活对象
            pool.InactiveObjects.Remove(go);
            go.SetActive(true);
        }
        
        return go;
    }

    /// <summary>
    /// 将游戏对象返回到对象池中
    /// </summary>
    /// <param name="obj">要回收的游戏对象</param>
    public static void ReturnObjectToPool(GameObject obj)
    {
        // 移除对象名称中的"(Clone)"后缀以获取原始名称
        string goName = obj.name.Substring(0, obj.name.Length - 7);
        
        // 查找对应对象类型的对象池
        PooledObjectInfo pool = ObjectPools.Find(p => p.LookupString == goName);
        
        if (pool != null)
        {
            // 将对象设置为非激活状态并添加到对象池中
            obj.SetActive(false);
            pool.InactiveObjects.Add(obj);
        }
    }
}

/// <summary>
/// 存储对象池信息的类
/// </summary>
public class PooledObjectInfo
{
    /// <summary>
    /// 用于查找对象池的字符串标识
    /// </summary>
    public string LookupString;
    
    /// <summary>
    /// 存储未激活游戏对象的列表
    /// </summary>
    public List<GameObject> InactiveObjects = new List<GameObject>();
}