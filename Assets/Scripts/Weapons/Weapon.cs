using System;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// 武器组件，应附加到所有武器预制体上。该组件与 WeaponData ScriptableObject 协作，
/// 用于管理游戏中所有武器的行为和属性。
/// </summary>
public class Weapon : Item
{
    /// <summary>
    /// 武器的统计数据结构，包含武器的名称、描述、视觉效果、数值等信息。
    /// </summary>
    [Serializable]
    public class Stats : LevelData
    {
        [Header("视觉效果")]
        public Projectile projectilePrefab; // 若设置，则每次武器冷却完成后生成一个投射物
        public Aura auraPrefab;             // 若设置，则在武器装备时生成一个光环
        public ParticleSystem hitEffect, procEffect;    // 命中时播放的粒子效果
        public Rect spawnVariance;          // 生成位置的随机偏移范围
        
        [Header("数值")]
        public float lifespan;         // 生命时间，若为0则永久存在
        public float damage;           // 基础伤害
        public float damageVariance;   // 伤害浮动范围
        public float area;             // 攻击范围
        public float speed;            // 移动速度或发射速度
        public float cooldown;         // 攻击冷却时间
        public float projectileInterval; // 投射物生成间隔
        public float knockback;        // 击退力度
        public int number;             // 每次攻击生成的数量
        public int piercing;           // 穿透次数
        public int maxInstances;       // 最大实例数量

        public EntityStats.BuffInfo[] appliedBuffs;
        
        /// <summary>
        /// 重载加法运算符，用于将两个 Stats 结构相加。
        /// 主要用于升级武器时叠加属性。
        /// </summary>
        /// <param name="s1">第一个 Stats 结构</param>
        /// <param name="s2">第二个 Stats 结构</param>
        /// <returns>相加后的 Stats 结构</returns>
        public static Stats operator +(Stats s1, Stats s2)
        {
            Stats result = new Stats();
            result.name = s2.name ?? s1.name;
            result.description = s2.description ?? s1.description;
            result.projectilePrefab = s2.projectilePrefab ?? s1.projectilePrefab;
            result.auraPrefab = s2.auraPrefab ?? s1.auraPrefab;
            result.hitEffect = s2.hitEffect == null ? s1.hitEffect : s2.hitEffect;
            result.procEffect = s2.procEffect == null ? s1.procEffect : s2.procEffect;
            result.spawnVariance = s2.spawnVariance;
            result.lifespan = s1.lifespan + s2.lifespan;
            result.damage = s1.damage + s2.damage;
            result.damageVariance = s1.damageVariance + s2.damageVariance;
            result.area = s1.area + s2.area;
            result.speed = s1.speed + s2.speed;
            result.cooldown = s1.cooldown + s2.cooldown;
            result.number = s1.number + s2.number;
            result.piercing = s1.piercing + s2.piercing;
            result.projectileInterval = s1.projectileInterval + s2.projectileInterval;
            result.knockback = s1.knockback + s2.knockback;
            result.appliedBuffs = s2.appliedBuffs == null || s2.appliedBuffs.Length <= 0
                ? s1.appliedBuffs
                : s2.appliedBuffs;
            return result;
        }

        /// <summary>
        /// 获取实际造成的伤害值，包含伤害浮动。
        /// </summary>
        /// <returns>实际伤害值</returns>
        public float GetDamage()
        {
            return damage + Random.Range(0, damageVariance);
        }
    }

    protected Stats currentStats;       // 当前武器的统计数据
    protected float currentCooldown;     // 当前冷却时间
    protected PlayerMovement movement;  // 玩家移动组件的引用

    /// <summary>
    /// 初始化武器组件，用于动态创建的武器。
    /// </summary>
    /// <param name="data">武器数据</param>
    public virtual void Initialise(WeaponData data)
    {
        base.Initialise(data);
        this.data = data;
        currentStats = data.baseStats;
        movement = GetComponentInParent<PlayerMovement>();
        ActivateCooldown();
    }

    /// <summary>
    /// 每帧更新冷却时间，冷却结束后执行攻击。
    /// </summary>
    protected virtual void Update()
    {
        currentCooldown -= Time.deltaTime;
        if (currentCooldown <= 0f) // 冷却结束时执行攻击
        {
            Attack(currentStats.number + owner.Stats.amount);
        }
    }

    /// <summary>
    /// 武器升级一级，并更新对应属性。
    /// </summary>
    /// <returns>是否成功升级</returns>
    public override bool DoLevelUp()
    {
        base.DoLevelUp();
        // 若已达到最大等级则不允许继续升级
        if (!CanLevelUp())
        {
            Debug.LogWarning(string.Format("{0} 无法升级到等级 {1}，已达到最大等级 {2}。", name, currentLevel, data.maxLevel));
            return false;
        }

        // 否则将下一级的属性加到当前属性上
        currentStats += (Stats)data.GetLevelData(++currentLevel);
        return true;
    }

    /// <summary>
    /// 检查当前武器是否可以攻击。
    /// </summary>
    /// <returns>是否可以攻击</returns>
    public virtual bool CanAttack()
    {
        if (Mathf.Approximately(owner.Stats.might, 0)) return false;
        return currentCooldown <= 0;
    }

    /// <summary>
    /// 执行一次攻击操作。
    /// 子类需重写此方法以实现具体攻击逻辑。
    /// </summary>
    /// <param name="attackCount">攻击次数</param>
    /// <returns>是否成功执行攻击</returns>
    protected virtual bool Attack(int attackCount = 1)
    {
        if (CanAttack())
        {
            ActivateCooldown();
            return true;
        }
        return false;
    }

    /// <summary>
    /// 获取武器实际造成的伤害值，包含伤害浮动和角色力量加成。
    /// </summary>
    /// <returns>实际伤害值</returns>
    public virtual float GetDamage()
    {
        return currentStats.GetDamage() * owner.Stats.might;
    }
    
    /// <summary>
    /// 获取武器的攻击范围，包括来自玩家属性的修改。
    /// </summary>
    /// <returns>最终攻击范围</returns>
    public virtual float GetArea()
    {
        return currentStats.area + owner.Stats.area;
    }

    /// <summary>
    /// 获取当前武器的统计数据。
    /// </summary>
    /// <returns>当前武器的 Stats 结构</returns>
    public virtual Stats GetStats() { return currentStats; }
    
    /// <summary>
    /// 刷新武器的冷却时间。
    /// 如果 strict 为 true，则仅在 currentCooldown 小于等于 0 时刷新。
    /// </summary>
    /// <param name="strict">是否启用严格模式</param>
    /// <returns>是否成功激活冷却</returns>
    public virtual bool ActivateCooldown(bool strict = false)
    {
        // 当<strict>启用且冷却时间尚未结束时，
        // 不要刷新冷却时间。
        if (strict && currentCooldown > 0) return false;

        // 计算冷却时间，考虑玩家角色中的冷却减少属性。
        float actualCooldown = currentStats.cooldown * Owner.Stats.cooldown;

        // 将最大冷却时间限制为实际冷却时间，因此我们不能意外多次调用此函数时将冷却时间增加到超过冷却属性。
        currentCooldown = Mathf.Min(actualCooldown, currentCooldown + actualCooldown);
        return true;
    }
    
    /// <summary>
    /// 使武器将其增益应用到目标的 EntityStats 对象上。
    /// </summary>
    /// <param name="e">目标实体的状态对象</param>
    public void ApplyBuffs(EntityStats e)
    {
        // 将所有分配的增益应用到目标上。
        foreach (EntityStats.BuffInfo b in GetStats().appliedBuffs)
            e.ApplyBuff(b, owner.Actual.duration);
    }
}
