using UnityEngine;

/// <summary>
/// 大蒜武器控制器类，继承自WeaponController
/// 负责控制大蒜武器的初始化和攻击行为
/// </summary>
public class GarlicController : WeaponController
{
    /// <summary>
    /// 初始化方法，在对象启用时调用
    /// 调用父类的Start方法进行基础初始化
    /// </summary>
    protected override void Start()
    {
        base.Start();
    }

    /// <summary>
    /// 执行攻击动作的方法
    /// 调用父类攻击方法并实例化大蒜预制体
    /// </summary>
    protected override void Attack()
    {
        base.Attack();
        // 实例化大蒜预制体并设置其位置
        GameObject spawnedGarlic = Instantiate(weaponData.Prefab, transform);
        spawnedGarlic.transform.position = transform.position;
    }
}

