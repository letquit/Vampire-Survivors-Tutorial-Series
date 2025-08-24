using UnityEngine;

/// <summary>
/// 光环武器类，继承自Weapon基类
/// 该类负责管理具有光环效果的武器，包括光环的装备、卸载和升级功能
/// </summary>
public class AuraWeapon : Weapon
{
    protected Aura currentAura;

    /// <summary>
    /// 每帧调用的更新方法
    /// 重写基类的Update方法，当前实现为空
    /// </summary>
    protected override void Update() { }

    /// <summary>
    /// 武器装备时调用的方法
    /// 尝试使用当前统计数据中的光环预制体创建并装备光环效果
    /// </summary>
    public override void OnEquip()
    {
        // 尝试用新光环替换武器当前的光环
        if (currentStats.auraPrefab)
        {
            if (currentAura) Destroy(currentAura);
            currentAura = Instantiate(currentStats.auraPrefab, transform);
            currentAura.weapon = this;
            currentAura.owner = owner;

            float area = GetArea();
            currentAura.transform.localScale = new Vector3(area, area, area);
        }
    }

    /// <summary>
    /// 武器卸载时调用的方法
    /// 销毁当前装备的光环效果
    /// </summary>
    public override void OnUnequip()
    {
        // 如果存在当前光环，则销毁它
        if (currentAura) Destroy(currentAura);
    }

    /// <summary>
    /// 执行武器升级操作
    /// 调用基类升级方法，如果升级成功则更新光环的缩放大小
    /// </summary>
    /// <returns>升级是否成功执行</returns>
    public override bool DoLevelUp(bool updateUI = true)
    {
        // 调用基类的升级方法，如果失败则直接返回false
        if (!base.DoLevelUp(updateUI)) return false;
        
        OnEquip();
        
        // 如果该武器附加了光环，则更新光环效果
        if (currentAura)
        {
            currentAura.transform.localScale = new Vector3(currentStats.area, currentStats.area, currentStats.area);
        }
        return true;
    }
}
