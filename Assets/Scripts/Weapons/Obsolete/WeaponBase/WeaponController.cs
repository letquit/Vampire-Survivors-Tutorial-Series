using System;
using UnityEngine;

/// <summary>
/// 武器控制器基类，负责管理武器的基本属性和攻击逻辑
/// </summary>
public class WeaponController : MonoBehaviour
{
    [Header("Weapon Stats")] 
    public WeaponScriptableObject weaponData;
    private float currentCooldown;      // 当前冷却剩余时间

    protected PlayerMovement pm;        // 玩家移动组件引用
    
    /// <summary>
    /// 初始化武器控制器，获取玩家移动组件并设置初始冷却时间
    /// </summary>
    protected virtual void Start()
    {
        pm = FindFirstObjectByType<PlayerMovement>();
        currentCooldown = weaponData.cooldownDuration;
    }

    /// <summary>
    /// 每帧更新武器冷却状态，当冷却结束时执行攻击
    /// </summary>
    protected virtual void Update()
    {
        // 更新冷却计时器
        currentCooldown -= Time.deltaTime;
        if (currentCooldown <= 0f)
        {
            Attack();
        }
    }

    /// <summary>
    /// 执行武器攻击逻辑，重置冷却时间
    /// </summary>
    protected virtual void Attack()
    {
        currentCooldown = weaponData.cooldownDuration;
    }
}
