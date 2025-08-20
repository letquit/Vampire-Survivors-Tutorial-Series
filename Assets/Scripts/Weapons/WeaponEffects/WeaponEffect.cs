using UnityEngine;

/// <summary>
/// 武器发射时产生的效果游戏对象，例如抛射物、光环、脉冲等。
/// </summary>
public abstract class WeaponEffect : MonoBehaviour
{
    [HideInInspector] 
    public PlayerStats owner;
    [HideInInspector] 
    public Weapon weapon;

    public PlayerStats Owner {
        get {
            return owner;
        }
    }
    
    /// <summary>
    /// 获取武器伤害值
    /// </summary>
    /// <returns>武器的伤害数值</returns>
    public float GetDamage()
    {
        return weapon.GetDamage();
    }
}

