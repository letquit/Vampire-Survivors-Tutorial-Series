using System;
using UnityEngine;

/// <summary>
/// 刀具控制器类，继承自武器控制器基类
/// 负责控制刀具的攻击行为和实例化逻辑
/// </summary>
[Obsolete]
public class KnifeController : WeaponController
{
    /// <summary>
    /// 初始化方法，在对象启用时调用
    /// 调用基类的Start方法进行初始化
    /// </summary>
    protected override void Start()
    {
        base.Start();
    }
    
    /// <summary>
    /// 执行攻击逻辑的方法
    /// 重写基类的Attack方法，实现刀具特有的攻击行为
    /// </summary>
    protected override void Attack()
    {
        base.Attack();
        
        // 实例化刀具预制体并设置位置
        GameObject spawnedKnife = Instantiate(weaponData.prefab);
        spawnedKnife.transform.position = transform.position; //Assign the position to be the same as this object which is parented to the player
        
        // 获取刀具行为组件并设置方向
        spawnedKnife.GetComponent<KnifeBehaviour>().DirectionChecker(pm.lastMovedVector);
    }
}

