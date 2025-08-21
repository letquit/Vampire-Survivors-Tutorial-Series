using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// 掉落管理器类，用于控制游戏对象销毁时的物品掉落逻辑
/// </summary>
public class DropRateManager : MonoBehaviour
{
    public bool active = false;
    public List<Drops> drops;

    /// <summary>
    /// 当游戏对象被销毁时调用，根据掉落概率随机生成掉落物品
    /// </summary>
    private void OnDestroy()
    {
        if(!active) return;
        // 检查游戏对象所在的场景是否已加载，如果未加载则直接返回
        if (!gameObject.scene.isLoaded)
        {
            return;
        }

        
        // 生成0到100之间的随机数，用于概率判断
        float randomNumber = Random.Range(0f, 100f);
        List<Drops> possiableDrops = new List<Drops>();

        // 遍历所有掉落配置，筛选出符合条件的掉落项
        foreach (Drops rate in drops)
        {
            if (randomNumber <= rate.dropRate)
            {
                possiableDrops.Add(rate);
            }
        }

        // 如果存在可掉落的物品，则随机选择一个进行实例化
        if (possiableDrops.Count > 0)
        {
            Drops drops = possiableDrops[Random.Range(0, possiableDrops.Count)];
            Instantiate(drops.itemPrefab, transform.position, Quaternion.identity);
        }
    }

    /// <summary>
    /// 掉落配置类，定义单个物品的掉落信息
    /// </summary>
    [Serializable]
    public class Drops
    {
        public string name;          // 物品名称
        public GameObject itemPrefab; // 物品预制体
        public float dropRate;       // 掉落概率（0-100）
    }
}

