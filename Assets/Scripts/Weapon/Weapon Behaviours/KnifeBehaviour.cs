using System;
using UnityEngine;

/// <summary>
/// 刀具行为类，继承自投射物武器行为基类
/// 控制刀具的运动和行为逻辑
/// </summary>
public class KnifeBehaviour : ProjectileWeaponBehaviour
{

    /// <summary>
    /// 初始化函数，在对象启用时调用
    /// 调用基类的Start方法并获取KnifeController组件实例
    /// </summary>
    protected override void Start()
    {
        base.Start();
    }

    /// <summary>
    /// 每帧更新函数
    /// 控制刀具按照指定方向和速度进行移动
    /// </summary>
    private void Update()
    {
        // 根据方向、时间和速度计算刀具的新位置
        transform.position += direction * Time.deltaTime * weaponData.Speed;
    }
}

