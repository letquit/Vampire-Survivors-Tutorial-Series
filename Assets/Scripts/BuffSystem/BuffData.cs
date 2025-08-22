using UnityEngine;

/// <summary>
/// BuffData 是一个可以用于在任何 EntityStats 对象上创建基本增益的类。这个基本增益将对拥有者进行治疗或造成伤害，并在一定时间后失效。
/// </summary>
[CreateAssetMenu(fileName = "Buff Data", menuName = "2D Top-down Rogue-like/Buff Data")]
public class BuffData : ScriptableObject
{
    /// <summary>
    /// 增益效果的名称。
    /// </summary>
    public new string name = "New Buff";

    /// <summary>
    /// 增益效果的图标。
    /// </summary>
    public Sprite icon;

    /// <summary>
    /// 增益类型枚举，使用位标志表示不同类型的增益。
    /// </summary>
    [System.Flags]
    public enum Type : byte 
    { 
        /// <summary>
        /// 正面效果（增益）。
        /// </summary>
        buff = 1, 

        /// <summary>
        /// 负面效果（减益）。
        /// </summary>
        debuff = 2, 

        /// <summary>
        /// 冻结效果。
        /// </summary>
        freeze = 4, 

        /// <summary>
        /// 强化效果。
        /// </summary>
        strong = 8 
    }

    /// <summary>
    /// 增益的类型。
    /// </summary>
    public Type type;

    /// <summary>
    /// 堆叠类型枚举，定义增益如何与其他相同增益堆叠。
    /// </summary>
    public enum StackType : byte 
    { 
        /// <summary>
        /// 仅刷新持续时间。
        /// </summary>
        refreshDurationOnly, 

        /// <summary>
        /// 完全堆叠。
        /// </summary>
        stacksFully, 

        /// <summary>
        /// 不堆叠。
        /// </summary>
        doesNotStack 
    }

    /// <summary>
    /// 修饰符类型枚举，定义属性修改的方式。
    /// </summary>
    public enum ModifierType : byte 
    { 
        /// <summary>
        /// 加法修饰符。
        /// </summary>
        additive, 

        /// <summary>
        /// 乘法修饰符。
        /// </summary>
        multiplicative 
    }
    
    /// <summary>
    /// 增益的具体属性和效果配置。
    /// </summary>
    [System.Serializable]
    public class Stats
    {
        /// <summary>
        /// 属性变体的名称。
        /// </summary>
        public string name;

        /// <summary>
        /// 视觉效果相关配置。
        /// </summary>
        [Header("视觉效果")]
        [Tooltip("附加到带有增益的GameObject上的效果。")]
        public ParticleSystem effect;

        [Tooltip("受此增益影响的单位的颜色。")]
        public Color tint = new Color(0, 0, 0, 0);

        [Tooltip("此增益是否减慢或加快受影响的GameObject的动画速度。")]
        public float animationSpeed = 1f;

        /// <summary>
        /// 增益的核心属性配置。
        /// </summary>
        [Header("属性")]
        public float duration;
        public float damagePerSecond, healPerSecond;

        [Tooltip("控制每秒伤害/治疗的频率。")]
        public float tickInterval = 0.25f;

        public StackType stackType;
        public ModifierType modifierType;
        
        /// <summary>
        /// 初始化默认属性值。
        /// </summary>
        public Stats()
        {
            duration = 10f;
            damagePerSecond = 1f;
            healPerSecond = 1f;
            tickInterval = 0.25f;
        }

        /// <summary>
        /// 玩家属性修饰符。
        /// </summary>
        public CharacterData.Stats playerModifier;

        /// <summary>
        /// 敌人属性修饰符。
        /// </summary>
        public EnemyStats.Stats enemyModifier;
    }
    
    /// <summary>
    /// 不同等级或变体的增益属性数组。
    /// </summary>
    public Stats[] variations = new Stats[1] {
        new Stats { name = "Level 1" }
    };

    /// <summary>
    /// 计算指定变体每个刻度造成的伤害值。
    /// </summary>
    /// <param name="variant">增益变体索引，默认为0。</param>
    /// <returns>每个刻度的伤害值。</returns>
    public float GetTickDamage(int variant = 0)
    {
        Stats s = Get(variant);
        return s.damagePerSecond * s.tickInterval;
    }

    /// <summary>
    /// 计算指定变体每个刻度的治疗值。
    /// </summary>
    /// <param name="variant">增益变体索引，默认为0。</param>
    /// <returns>每个刻度的治疗值。</returns>
    public float GetTickHeal(int variant = 0)
    {
        Stats s = Get(variant);
        return s.healPerSecond * s.tickInterval;
    }

    /// <summary>
    /// 获取指定索引的增益变体属性。
    /// </summary>
    /// <param name="variant">增益变体索引，如果为负数则返回第一个变体。</param>
    /// <returns>对应的增益属性变体。</returns>
    public Stats Get(int variant = -1)
    {
        return variations[Mathf.Max(0, variant)];
    }
}
