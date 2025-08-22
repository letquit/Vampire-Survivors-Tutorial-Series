using UnityEngine;

/// <summary>
/// 角色数据类，用于存储和管理角色的基本属性和配置信息
/// 继承自ScriptableObject，可以在Unity编辑器中创建资产文件
/// </summary>
[CreateAssetMenu(fileName = "Character Data", menuName = "2D Top-down Rogue-like/Character Data")]
public class CharacterData : ScriptableObject
{
    /// <summary>
    /// 角色图标精灵
    /// </summary>
    [SerializeField]
    private Sprite icon;
    
    /// <summary>
    /// 获取或设置角色图标
    /// </summary>
    public Sprite Icon { get => icon; private set => icon = value; }

    /// <summary>
    /// 运行时动画控制器
    /// </summary>
    public RuntimeAnimatorController controller;
    
    /// <summary>
    /// 角色名称
    /// </summary>
    [SerializeField]
    new string name;
    
    /// <summary>
    /// 获取或设置角色名称
    /// </summary>
    public string Name { get => name; private set => name = value; }

    /// <summary>
    /// 起始武器数据
    /// </summary>
    [SerializeField]
    private WeaponData startingWeapon;
    
    /// <summary>
    /// 获取或设置起始武器数据
    /// </summary>
    public WeaponData StartingWeapon { get => startingWeapon; private set => startingWeapon = value; }

    /// <summary>
    /// 角色属性结构体，包含角色的各项基础和增益属性
    /// </summary>
    [System.Serializable]
    public struct Stats
    {
        public float maxHealth, recovery, armor;
        [Range(-1, 10)] public float moveSpeed, might, area;
        [Range(-1, 5)] public float speed, duration;
        [Range(-1, 10)] public int amount;
        [Range(-1, 1)] public float cooldown;
        [Min(-1)] public float luck, growth, greed, curse;
        public float magnet;
        public int revival;
        
        /// <summary>
        /// 重载加法运算符，用于属性叠加计算
        /// </summary>
        /// <param name="s1">第一个属性结构体</param>
        /// <param name="s2">第二个属性结构体</param>
        /// <returns>叠加后的属性结构体</returns>
        public static Stats operator +(Stats s1, Stats s2)
        {
            s1.maxHealth += s2.maxHealth;
            s1.recovery += s2.recovery;
            s1.armor += s2.armor;
            s1.moveSpeed += s2.moveSpeed;
            s1.might += s2.might;
            s1.area += s2.area;
            s1.speed += s2.speed;
            s1.duration += s2.duration;
            s1.amount += s2.amount;
            s1.cooldown += s2.cooldown;
            s1.luck += s2.luck;
            s1.growth += s2.growth;
            s1.greed += s2.greed;
            s1.curse += s2.curse;
            s1.magnet += s2.magnet;
            return s1;
        }
        
        /// <summary>
        /// 重载乘法运算符，用于属性倍率计算
        /// </summary>
        /// <param name="s1">第一个属性结构体（被乘数）</param>
        /// <param name="s2">第二个属性结构体（乘数）</param>
        /// <returns>相乘后的属性结构体</returns>
        public static Stats operator *(Stats s1, Stats s2)
        {
            // 将s2的最大生命值乘以s1的最大生命值。
            s1.maxHealth *= s2.maxHealth;
            // 将s2的恢复值乘以s1的恢复值。
            s1.recovery *= s2.recovery;
            // 将s2的护甲值乘以s1的护甲值。
            s1.armor *= s2.armor;
            // 将s2的移动速度乘以s1的移动速度。
            s1.moveSpeed *= s2.moveSpeed;
            // 将s2的力量值乘以s1的力量值。
            s1.might *= s2.might;
            // 将s2的区域值乘以s1的区域值。
            s1.area *= s2.area;
            // 将s2的速度值乘以s1的速度值。
            s1.speed *= s2.speed;
            // 将s2的持续时间乘以s1的持续时间。
            s1.duration *= s2.duration;
            // 将s2的数量乘以s1的数量。
            s1.amount *= s2.amount;
            // 将s2的冷却时间乘以s1的冷却时间。
            s1.cooldown *= s2.cooldown;
            // 将s2的幸运值乘以s1的幸运值。
            s1.luck *= s2.luck;
            // 将s2的成长值乘以s1的成长值。
            s1.growth *= s2.growth;
            // 将s2的贪婪值乘以s1的贪婪值。
            s1.greed *= s2.greed;
            // 将s2的诅咒值乘以s1的诅咒值。
            s1.curse *= s2.curse;
            // 将s2的磁力值乘以s1的磁力值。
            s1.magnet *= s2.magnet;
            return s1;
        }
    }
    

    /// <summary>
    /// 角色基础属性，初始化为默认值
    /// </summary>
    public Stats stats = new Stats {
        maxHealth = 100,
        moveSpeed = 1,
        might = 1,
        amount = 0,
        area = 1,
        speed = 1,
        duration = 1,
        cooldown = 1,
        luck = 1,
        greed = 1,
        growth = 1,
        curse = 1
    };
}

