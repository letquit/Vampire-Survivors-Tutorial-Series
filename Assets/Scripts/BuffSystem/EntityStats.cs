using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// EntityStats 是一个抽象类，被 PlayerStats 和 EnemyStats 继承。
/// 它提供了一种统一的方式来管理实体的属性、增益系统、着色系统和动画速度控制。
/// </summary>
public abstract class EntityStats : MonoBehaviour
{
    protected float health;
    
    /// <summary>
    /// 着色系统相关字段。
    /// </summary>
    protected SpriteRenderer sprite;
    protected Animator animator;
    protected Color originalColor;
    protected List<Color> appliedTints = new List<Color>();
    public const float TINT_FACTOR = 4f;

    /// <summary>
    /// Buff 类表示一个活动的增益效果。
    /// 包含增益数据、持续时间、粒子效果、颜色着色以及动画速度调整等信息。
    /// </summary>
    [Serializable]
    public class Buff
    {
        public BuffData data;
        public float remainingDuration, nextTick;
        public int variant;

        public ParticleSystem effect;
        public Color tint;
        public float animationSpeed = 1f;
        
        /// <summary>
        /// 构造函数初始化一个新的 Buff 实例。
        /// </summary>
        /// <param name="d">增益的数据源。</param>
        /// <param name="owner">拥有该增益的实体。</param>
        /// <param name="variant">增益的变体索引。</param>
        /// <param name="durationMultiplier">持续时间倍数。</param>
        public Buff(BuffData d, EntityStats owner, int variant = 0, float durationMultiplier = 1f)
        {
            data = d;
            BuffData.Stats buffStats = d.Get(variant);
            remainingDuration = buffStats.duration * durationMultiplier;
            nextTick = buffStats.tickInterval;
            this.variant = variant;

            if (buffStats.effect) effect = Instantiate(buffStats.effect, owner.transform);
            if (buffStats.tint.a > 0)
            {
                tint = buffStats.tint;
                owner.ApplyTint(buffStats.tint);
            }

            animationSpeed = buffStats.animationSpeed;
            owner.ApplyAnimationMultiplier(animationSpeed);
        }

        /// <summary>
        /// 获取当前变体的增益统计数据。
        /// </summary>
        /// <returns>当前变体的 BuffData.Stats 对象。</returns>
        public BuffData.Stats GetData()
        {
            return data.Get(variant);
        }
    }

    protected List<Buff> activeBuffs = new List<Buff>();

    /// <summary>
    /// BuffInfo 类用于存储增益应用时的信息，包括数据、变体和触发概率。
    /// </summary>
    [Serializable]
    public class BuffInfo
    {
        public BuffData data;
        public int variant;
        [Range(0f, 1f)] public float probability = 1f;
    }
    
    /// <summary>
    /// Start 方法在对象启用时调用，用于初始化 SpriteRenderer 和 Animator 组件。
    /// </summary>
    protected virtual void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        originalColor = sprite.color;
        animator = GetComponent<Animator>();
    }
    
    /// <summary>
    /// 应用动画速度倍数因子。
    /// </summary>
    /// <param name="factor">要应用的速度倍数。</param>
    public virtual void ApplyAnimationMultiplier(float factor)
    {
        animator.speed *= Mathf.Approximately(0, factor) ? 0.000001f : factor;
    }

    /// <summary>
    /// 移除动画速度倍数因子。
    /// </summary>
    /// <param name="factor">要移除的速度倍数。</param>
    public virtual void RemoveAnimationMultiplier(float factor)
    {
        animator.speed /= Mathf.Approximately(0, factor) ? 0.000001f : factor;
    }

    /// <summary>
    /// 添加一个颜色着色到实体上。
    /// </summary>
    /// <param name="c">要添加的颜色。</param>
    public virtual void ApplyTint(Color c)
    {
        appliedTints.Add(c);
        UpdateColor();
    }

    /// <summary>
    /// 从实体上移除一个颜色着色。
    /// </summary>
    /// <param name="c">要移除的颜色。</param>
    public virtual void RemoveTint(Color c)
    {
        appliedTints.Remove(c);
        UpdateColor();
    }

    /// <summary>
    /// 更新实体的颜色，基于所有已应用的着色进行混合计算。
    /// </summary>
    protected virtual void UpdateColor()
    {
        Color targetColor = originalColor;
        float totalWeight = 1f;
        foreach (Color c in appliedTints)
        {
            targetColor = new Color(
                targetColor.r + c.r * c.a * TINT_FACTOR,
                targetColor.g + c.g * c.a * TINT_FACTOR,
                targetColor.b + c.b * c.a * TINT_FACTOR,
                targetColor.a
            );
            totalWeight += c.a * TINT_FACTOR;
        }
        targetColor = new Color(
            targetColor.r / totalWeight,
            targetColor.g / totalWeight,
            targetColor.b / totalWeight,
            targetColor.a
        );

        sprite.color = targetColor;
    }
    
    /// <summary>
    /// 获取指定增益数据和变体的活动增益实例。
    /// </summary>
    /// <param name="data">增益数据。</param>
    /// <param name="variant">增益变体索引，默认为 -1 表示不检查变体。</param>
    /// <returns>匹配的 Buff 实例，如果没有找到则返回 null。</returns>
    public virtual Buff GetBuff(BuffData data, int variant = -1)
    {
        foreach (Buff b in activeBuffs)
        {
            if (b.data == data)
            {
                if (variant >= 0)
                {
                    if (b.variant == variant) return b;
                }
                else
                {
                    return b;
                }
            }
        }
        return null;
    }
    
    /// <summary>
    /// 根据 BuffInfo 中的概率尝试应用一个增益。
    /// </summary>
    /// <param name="info">包含增益数据和概率的 BuffInfo 对象。</param>
    /// <param name="durationMultiplier">持续时间倍数。</param>
    /// <returns>是否成功应用了增益。</returns>
    public virtual bool ApplyBuff(BuffInfo info, float durationMultiplier = 1f)
    {
        if (Random.value <= info.probability)
            return ApplyBuff(info.data, info.variant, durationMultiplier);
        return false;
    }

    /// <summary>
    /// 直接应用一个增益到实体上。
    /// </summary>
    /// <param name="data">增益数据。</param>
    /// <param name="variant">增益变体索引。</param>
    /// <param name="durationMultiplier">持续时间倍数。</param>
    /// <returns>是否成功应用了增益。</returns>
    public virtual bool ApplyBuff(BuffData data, int variant = 0, float durationMultiplier = 1f)
    {
        Buff b;
        BuffData.Stats s = data.Get(variant);

        switch (s.stackType)
        {
            case BuffData.StackType.stacksFully:
                activeBuffs.Add(new Buff(data, this, variant, durationMultiplier));
                RecalculateStats();
                return true;
            case BuffData.StackType.refreshDurationOnly:
                b = GetBuff(data, variant);
                if (b != null)
                {
                    b.remainingDuration = s.duration * durationMultiplier;
                }
                else
                {
                    activeBuffs.Add(new Buff(data, this, variant, durationMultiplier));
                    RecalculateStats();
                }
                return true;
            case BuffData.StackType.doesNotStack:
                b = GetBuff(data, variant);
                if (b != null)
                {
                    activeBuffs.Add(new Buff(data, this, variant, durationMultiplier));
                    RecalculateStats();
                    return true;
                }
                return false;
        }
        
        return false;
    }
    
    /// <summary>
    /// 移除指定增益的所有副本。
    /// </summary>
    /// <param name="data">要移除的增益数据。</param>
    /// <param name="variant">增益变体索引，默认为 -1 表示移除所有变体。</param>
    /// <returns>是否有增益被移除。</returns>
    public virtual bool RemoveBuff(BuffData data, int variant = -1)
    {
        List<Buff> toRemove = new List<Buff>();
        foreach (Buff b in activeBuffs)
        {
            if (b.data == data)
            {
                if (variant >= 0)
                {
                    if (b.variant == variant) toRemove.Add(b);
                }
                else
                {
                    toRemove.Add(b);
                }
            }
        }

        if (toRemove.Count > 0)
        {
            foreach (Buff b in toRemove)
            {
                if (b.effect) Destroy(b.effect.gameObject);
                if (b.tint.a > 0) RemoveTint(b.tint);
                RemoveAnimationMultiplier(b.animationSpeed);
                activeBuffs.Remove(b);
            }
            RecalculateStats();
            return true;
        }
        return false;
    }
    
    /// <summary>
    /// 抽象方法：处理实体受到的伤害。
    /// </summary>
    /// <param name="dmg">伤害值。</param>
    public abstract void TakeDamage(float dmg);

    /// <summary>
    /// 抽象方法：恢复实体的生命值。
    /// </summary>
    /// <param name="amount">恢复的生命值数量。</param>
    public abstract void RestoreHealth(float amount);

    /// <summary>
    /// 抽象方法：处理实体死亡逻辑。
    /// </summary>
    public abstract void Kill();
    
    /// <summary>
    /// 抽象方法：强制实体重新计算其统计数据。
    /// </summary>
    public abstract void RecalculateStats();

    /// <summary>
    /// Update 方法每帧更新活动增益的状态，包括计时器和到期处理。
    /// </summary>
    protected virtual void Update()
    {
        List<Buff> expired = new List<Buff>();
        foreach (Buff b in activeBuffs)
        {
            BuffData.Stats s = b.data.Get(b.variant);

            b.nextTick -= Time.deltaTime;
            if (b.nextTick < 0)
            {
                float tickDmg = b.data.GetTickDamage(b.variant);
                if (tickDmg > 0) TakeDamage(tickDmg);
                float tickHeal = b.data.GetTickHeal(b.variant);
                if (tickHeal > 0) RestoreHealth(tickHeal);
                b.nextTick = s.tickInterval;
            }

            if (s.duration <= 0) continue;

            b.remainingDuration -= Time.deltaTime;
            if (b.remainingDuration < 0) expired.Add(b);
        }
        
        foreach (Buff b in expired)
        {
            if (b.effect) Destroy(b.effect.gameObject);
            if (b.tint.a > 0) RemoveTint(b.tint);
            RemoveAnimationMultiplier(b.animationSpeed);
            activeBuffs.Remove(b);
        }
        RecalculateStats(); 
    }
}
