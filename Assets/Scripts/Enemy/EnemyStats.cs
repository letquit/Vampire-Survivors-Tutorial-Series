using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// 敌人属性管理类，用于控制敌人的移动速度、生命值和伤害等属性
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class EnemyStats : EntityStats
{
    /// <summary>
    /// 敌人抗性结构体，包含冻结、击杀和减益效果的抗性值
    /// </summary>
    [Serializable]
    public class Resistances
    {
        /// <summary>
        /// 冻结抗性，范围在0到1之间
        /// </summary>
        [Range(-1f, 1f)] public float freeze;
        
        /// <summary>
        /// 击杀抗性，范围在0到1之间
        /// </summary>
        [Range(-1f, 1f)] public float kill;
        
        /// <summary>
        /// 减益效果抗性，范围在0到1之间
        /// </summary>
        [Range(-1f, 1f)] public float debuff;
        
        /// <summary>
        /// 重载乘法运算符，用于按因子缩放抗性值
        /// </summary>
        /// <param name="r">要缩放的抗性对象</param>
        /// <param name="factor">缩放因子</param>
        /// <returns>缩放后的抗性对象</returns>
        public static Resistances operator *(Resistances r, float factor)
        {
            r.freeze = Mathf.Min(1, r.freeze * factor);
            r.kill = Mathf.Min(1, r.kill * factor);
            r.debuff = Mathf.Min(1, r.debuff * factor);
            return r;
        }

        /// <summary>
        /// 重载加法运算符，将两个抗性对象相加
        /// </summary>
        /// <param name="r">第一个抗性对象</param>
        /// <param name="r2">第二个抗性对象</param>
        /// <returns>相加后的抗性对象</returns>
        public static Resistances operator +(Resistances r, Resistances r2)
        {
            r.freeze += r2.freeze;
            r.kill += r2.kill;
            r.debuff += r2.debuff;
            return r;
        }
        
        /// <summary>
        /// 重载乘法运算符，用于两个抗性对象相乘，实现叠加增益效果
        /// </summary>
        /// <param name="r1">第一个抗性对象</param>
        /// <param name="r2">第二个抗性对象</param>
        /// <returns>相乘后的抗性对象</returns>
        public static Resistances operator *(Resistances r1, Resistances r2)
        {
            r1.freeze = Mathf.Min(1, r1.freeze * r2.freeze);
            r1.kill = Mathf.Min(1, r1.kill * r2.kill);
            r1.debuff = Mathf.Min(1, r1.debuff * r2.debuff);
            return r1;
        }
    }

    /// <summary>
    /// 敌人基础属性结构体，包含生命值、移动速度、伤害等属性
    /// </summary>
    [Serializable]
    public struct Stats
    {
        /// <summary>
        /// 最大生命值
        /// </summary>
        public float maxHealth;
        
        /// <summary>
        /// 移动速度
        /// </summary>
        public float moveSpeed;
        
        /// <summary>
        /// 伤害值
        /// </summary>
        public float damage;
        
        /// <summary>
        /// 击退倍数
        /// </summary>
        public float knockbackMultiplier;
        
        /// <summary>
        /// 抗性属性
        /// </summary>
        public Resistances resistances;
        
        /// <summary>
        /// 可增强属性的枚举，使用位标志表示不同属性
        /// </summary>
        [Flags]
        public enum Boostable { 
            /// <summary>
            /// 生命值属性
            /// </summary>
            health = 1, 
            
            /// <summary>
            /// 移动速度属性
            /// </summary>
            moveSpeed = 2, 
            
            /// <summary>
            /// 伤害属性
            /// </summary>
            damage = 4, 
            
            /// <summary>
            /// 击退倍数属性
            /// </summary>
            knockbackMultiplier = 8, 
            
            /// <summary>
            /// 抗性属性
            /// </summary>
            resistances = 16 
        }
        
        /// <summary>
        /// 诅咒增强的属性
        /// </summary>
        public Boostable curseBoosts;
        
        /// <summary>
        /// 等级增强的属性
        /// </summary>
        public Boostable levelBoosts;

        /// <summary>
        /// 根据增强类型和因子增强属性值
        /// </summary>
        /// <param name="s1">要增强的属性结构体</param>
        /// <param name="factor">增强因子</param>
        /// <param name="boostable">要增强的属性类型</param>
        /// <returns>增强后的属性结构体</returns>
        private static Stats Boost(Stats s1, float factor, Boostable boostable)
        {
            if ((boostable & Boostable.health) != 0) s1.maxHealth *= factor;
            if ((boostable & Boostable.moveSpeed) != 0) s1.moveSpeed *= factor;
            if ((boostable & Boostable.damage) != 0) s1.damage *= factor;
            if ((boostable & Boostable.knockbackMultiplier) != 0) s1.knockbackMultiplier /= factor;
            if ((boostable & Boostable.resistances) != 0) s1.resistances *= factor;
            return s1;
        }

        /// <summary>
        /// 重载乘法运算符，用于增强诅咒效果
        /// </summary>
        /// <param name="s1">要增强的属性结构体</param>
        /// <param name="factor">增强因子</param>
        /// <returns>增强后的属性结构体</returns>
        public static Stats operator *(Stats s1, float factor) { return Boost(s1, factor, s1.curseBoosts); }

        /// <summary>
        /// 重载异或运算符，用于增强等级效果
        /// </summary>
        /// <param name="s1">要增强的属性结构体</param>
        /// <param name="factor">增强因子</param>
        /// <returns>增强后的属性结构体</returns>
        public static Stats operator ^(Stats s1, float factor) { return Boost(s1, factor, s1.levelBoosts); }

        /// <summary>
        /// 重载加法运算符，将两个属性结构体相加
        /// </summary>
        /// <param name="s1">第一个属性结构体</param>
        /// <param name="s2">第二个属性结构体</param>
        /// <returns>相加后的属性结构体</returns>
        public static Stats operator +(Stats s1, Stats s2)
        {
            s1.maxHealth += s2.maxHealth;
            s1.moveSpeed += s2.moveSpeed;
            s1.damage += s2.damage;
            s1.knockbackMultiplier += s2.knockbackMultiplier;
            s1.resistances += s2.resistances;
            return s1;
        }
        
        /// <summary>
        /// 重载乘法运算符，用于两个属性结构体相乘，实现叠加增益效果
        /// </summary>
        /// <param name="s1">第一个属性结构体</param>
        /// <param name="s2">第二个属性结构体</param>
        /// <returns>相乘后的属性结构体</returns>
        public static Stats operator *(Stats s1, Stats s2)
        {
            s1.maxHealth *= s2.maxHealth;
            s1.moveSpeed *= s2.moveSpeed;
            s1.damage *= s2.damage; // 原代码中的 maxHealth 应该是 damage，这里进行了修正。
            s1.knockbackMultiplier *= s2.knockbackMultiplier;
            s1.resistances *= s2.resistances;
            return s1;
        }
    }

    /// <summary>
    /// 敌人的基础属性值
    /// </summary>
    public Stats baseStats = new Stats
    {
        maxHealth = 10, moveSpeed = 1, damage = 3, knockbackMultiplier = 1, curseBoosts = (Stats.Boostable)(1 | 2),
        levelBoosts = 0
    };
    
    /// <summary>
    /// 敌人的实际属性值（经过计算后的最终属性）
    /// </summary>
    private Stats actualStats;
    
    /// <summary>
    /// 获取敌人的实际属性值
    /// </summary>
    public Stats Actual
    {
        get { return actualStats; }
    }

    public BuffInfo[] attackEffects;

    /// <summary>
    /// 玩家对象的变换组件引用
    /// </summary>
    private Transform player;

    /// <summary>
    /// 受伤反馈相关属性
    /// </summary>
    [Header("Damage Feedback")]
    
    /// <summary>
    /// 受伤时的颜色
    /// </summary>
    public Color damageColor = new Color(1, 0, 0, 1);
    
    /// <summary>
    /// 受伤闪烁持续时间
    /// </summary>
    public float damageFlashDuration = 0.2f;
    
    /// <summary>
    /// 死亡淡出时间
    /// </summary>
    public float deathFadeTime = 0.6f;
    
    /// <summary>
    /// 敌人移动组件引用
    /// </summary>
    private EnemyMovement movement;

    /// <summary>
    /// 敌人计数器，用于统计当前场景中的敌人数量
    /// </summary>
    public static int count;
    
    /// <summary>
    /// 在对象唤醒时初始化敌人的各项属性值
    /// 从enemyData中获取初始的移动速度、最大生命值和伤害值
    /// </summary>
    private void Awake()
    {
        count++;
    }

    /// <summary>
    /// 在Start阶段获取玩家对象和敌人生成器的引用
    /// </summary>
    protected override void Start()
    {
        base.Start();
        
        RecalculateStats();
        health = actualStats.maxHealth;
        
        movement = GetComponent<EnemyMovement>();
    }
    
    /// <summary>
    /// 应用增益或减益效果到敌人身上
    /// </summary>
    /// <param name="data">增益或减益数据</param>
    /// <param name="variant">变体索引</param>
    /// <param name="durationMultiplier">持续时间倍数</param>
    /// <returns>是否成功应用效果</returns>
    public override bool ApplyBuff(BuffData data, int variant = 0, float durationMultiplier = 1f)
    {
        // 如果减益效果是冻结，我们检查是否有冻结抗性。
        // 随机生成一个数值，如果成功，则忽略冻结效果。
        if ((data.type & BuffData.Type.freeze) > 0)
            if (Random.value <= Actual.resistances.freeze) return false;

        // 如果减益效果是负面状态，我们检查是否有负面状态抗性。
        if ((data.type & BuffData.Type.debuff) > 0)
            if (Random.value <= Actual.resistances.debuff) return false;

        return base.ApplyBuff(data, variant, durationMultiplier);
    }
    
    /// <summary>
    /// 根据各种因素计算敌人的实际属性
    /// </summary>
    public override void RecalculateStats()
    {
        float curse = GameManager.GetCumulativeCurse(),
            level = GameManager.GetCumulativeLevels();
        actualStats = (baseStats * curse) ^ level;
        
        // 创建一个变量来存储所有累积的倍数值。
        Stats multiplier = new Stats{
            maxHealth = 1f, moveSpeed = 1f, damage = 1f, knockbackMultiplier = 1,
            resistances = new Resistances {freeze = 1f, debuff = 1f, kill = 1f}
        };

        foreach (Buff b in activeBuffs)
        {
            BuffData.Stats bd = b.GetData();
            switch (bd.modifierType)
            {
                case BuffData.ModifierType.additive:
                    // 将增益效果的加法修饰符添加到实际统计数据中。
                    actualStats += bd.enemyModifier;
                    break;
                case BuffData.ModifierType.multiplicative:
                    // 将增益效果的乘法修饰符应用到倍数变量上。
                    multiplier *= bd.enemyModifier;
                    break;
            }
        }
        
        // 最后应用所有倍数。
        actualStats *= multiplier;
    }
    
    /// <summary>
    /// 对敌人造成伤害
    /// </summary>
    /// <param name="dmg">造成的伤害值</param>
    public override void TakeDamage(float dmg)
    {
        health -= dmg;

        // 如果伤害正好等于最大生命值，我们假设这是一个瞬间击杀，并检查杀伤抗性以确定是否可以闪避这次伤害。
        if (dmg == actualStats.maxHealth)
        {
            // 投掷骰子以检查是否可以闪避伤害。
            // 获取一个介于0到1之间的随机值，如果该数值低于杀伤抗性，则我们避免被击杀。
            if (Random.value < actualStats.resistances.kill)
            {
                return; // 不承受伤害。
            }
        }

        // 当敌人受到伤害时创建文本弹出框。
        if (dmg > 0)
        {
            StartCoroutine(DamageFlash());
            GameManager.GenerateFloatingText(Mathf.FloorToInt(dmg).ToString(), transform);
        }
        
        // 如果生命值降至零以下，则击杀敌人。
        if (health <= 0)
        {
            Kill();
        }
    }

    /// <summary>
    /// 对敌人造成伤害，并触发击退效果和视觉反馈
    /// </summary>
    /// <param name="dmg">造成的伤害值</param>
    /// <param name="sourcePosition">伤害来源的位置</param>
    /// <param name="knockbackForce">击退力度，默认为5f</param>
    /// <param name="knockbackDuration">击退持续时间，默认为0.2f</param>
    public void TakeDamage(float dmg, Vector2 sourcePosition, float knockbackForce = 5f, float knockbackDuration = 0.2f) 
    {
        TakeDamage(dmg);
        
        if (knockbackForce > 0)
        {
            Vector2 dir = (Vector2)transform.position - sourcePosition;
            movement.Knockback(dir.normalized * knockbackForce, knockbackDuration);
        }
    }
    
    /// <summary>
    /// 恢复敌人的生命值
    /// </summary>
    /// <param name="amount">恢复的生命值数量</param>
    public override void RestoreHealth(float amount)
    {
        // 如果当前生命值小于最大生命值，则恢复生命值。
        if (health < actualStats.maxHealth)
        {
            health += amount;
            // 如果恢复后的生命值超过最大生命值，则将其设置为最大生命值。
            if (health > actualStats.maxHealth)
            {
                health = actualStats.maxHealth;
            }
        }
    }

    /// <summary>
    /// 受伤时的闪烁效果协程
    /// </summary>
    /// <returns>IEnumerator用于协程执行</returns>
    IEnumerator DamageFlash()
    {
        ApplyTint(damageColor);
        yield return new WaitForSeconds(damageFlashDuration);
        RemoveTint(damageColor);
    }
    
    /// <summary>
    /// 销毁敌人游戏对象
    /// </summary>
    public override void Kill()
    {
        DropRateManager drops = GetComponent<DropRateManager>();
        if (drops) drops.active = true;
        
        StartCoroutine(KillFade());
    }

    /// <summary>
    /// 敌人死亡时淡出效果的协程
    /// </summary>
    /// <returns>IEnumerator用于协程执行</returns>
    IEnumerator KillFade()
    {
        WaitForEndOfFrame w = new WaitForEndOfFrame();
        float t = 0, origAlpha = sprite.color.a;

        while (t < deathFadeTime)
        {
            yield return w;
            t += Time.deltaTime;

            sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, (1 - t / deathFadeTime) * origAlpha);
        }
        
        Destroy(gameObject);
    }

    /// <summary>
    /// 当敌人与玩家发生碰撞时，对玩家造成伤害
    /// </summary>
    /// <param name="other">碰撞的另一个游戏对象的碰撞信息</param>
    private void OnCollisionStay2D(Collision2D other)
    {
        if (Mathf.Approximately(Actual.damage, 0)) return;
        
        // 检查是否有我们可以造成伤害的 PlayerStats 对象。
        if (other.collider.TryGetComponent(out PlayerStats p))
        {
            p.TakeDamage(Actual.damage);
            foreach (BuffInfo b in attackEffects)
                p.ApplyBuff(b);
        }
    }

    /// <summary>
    /// 当敌人被销毁时通知生成器减少敌人计数
    /// </summary>
    private void OnDestroy()
    {
        count--;
    }
}
