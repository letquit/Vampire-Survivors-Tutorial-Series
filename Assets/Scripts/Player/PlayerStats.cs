using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 玩家状态管理类，继承自 EntityStats。
/// 负责管理玩家的生命值、经验值、等级、武器生成以及无敌状态等核心属性和逻辑。
/// </summary>
public class PlayerStats : EntityStats
{
    private CharacterData characterData;
    public CharacterData.Stats baseStats;
    [SerializeField] 
    private CharacterData.Stats actualStats;

    /// <summary>
    /// 获取或设置当前实际属性值。
    /// </summary>
    public CharacterData.Stats Stats
    {
        get { return actualStats; }
        set
        {
            actualStats = value;
        }
    }

    /// <summary>
    /// 获取当前实际属性值的只读副本。
    /// </summary>
    public CharacterData.Stats Actual
    {
        get { return actualStats; }
    }

    #region 当前属性访问器
    /// <summary>
    /// 获取或设置当前生命值，并更新UI显示。
    /// </summary>
    public float CurrentHealth
    {
        get { return health; }
        set
        {
            if (health != value)
            {
                health = value;
                UpdateHealthBar();
            }
        }
    }
    #endregion

    [Header("视觉效果")]
    public ParticleSystem damageEffect; // 如果造成伤害时的粒子效果。
    public ParticleSystem blockedEffect; // 如果护甲完全阻挡伤害时的粒子效果。
    
    [Header("经验/等级")]
    public int experience = 0;
    public int level = 1;
    public int experienceCap;

    [Header("无敌帧")] 
    public float invincibilityDuration;
    private float invincibilityTimer;
    private bool isInvincible;

    public List<LevelRange> levelRanges;

    private PlayerCollector collector;
    private PlayerInventory inventory;

    [Header("UI")]
    public Image healthBar;
    public Image expBar;
    public TextMeshProUGUI levelText;
    
    private PlayerAnimator playerAnimator;

    /// <summary>
    /// 初始化角色数据、基础属性和初始生命值。
    /// </summary>
    void Awake()
    {
        characterData = UICharacterSelector.GetData();

        inventory = GetComponent<PlayerInventory>();
        collector = GetComponentInChildren<PlayerCollector>();

        // 分配变量
        baseStats = actualStats = characterData.stats;
        collector.SetRadius(actualStats.magnet);
        health = actualStats.maxHealth;
        playerAnimator = GetComponent<PlayerAnimator>();
        if (characterData.controller)
            playerAnimator.SetAnimationController(characterData.controller);
        
        UpdateHealthBar();
    }

    /// <summary>
    /// 启动时添加初始武器、初始化经验上限并更新UI显示。
    /// </summary>
    protected override void Start()
    {
        base.Start();
        
        if (UILevelSelector.globalBuff && !UILevelSelector.globalBuffAffectsPlayer)
            ApplyBuff(UILevelSelector.globalBuff);
        
        // 生成初始武器
        inventory.Add(characterData.StartingWeapon);

        // 初始化经验上限为第一个等级范围的经验增长量
        experienceCap = levelRanges[0].experienceCapIncrease;
        
        GameManager.instance.AssignChosenCharacterUI(characterData);

        UpdateExpBar();
        UpdateLevelText();
    }

    /// <summary>
    /// 每帧更新无敌计时器，并执行恢复生命值的逻辑。
    /// </summary>
    protected override void Update()
    {
        base.Update();
        // 更新无敌时间
        if (invincibilityTimer > 0)
        {
            invincibilityTimer -= Time.deltaTime;
        }
        else if (isInvincible)
        {
            isInvincible = false;
        }
        
        Recover();
    }

    /// <summary>
    /// 根据被动道具重新计算实际属性值。
    /// 遍历所有被动槽位中的道具，累加其提供的属性加成。
    /// </summary>
    public override void RecalculateStats()
    {
        actualStats = baseStats;
        foreach (PlayerInventory.Slot s in inventory.passiveSlots)
        {
            Passive p = s.item as Passive;
            if (p)
            {
                actualStats += p.GetBoosts();
            }
        }
        
        // 创建一个变量来存储所有累积的倍数值。
        CharacterData.Stats multiplier = new CharacterData.Stats
        {
            maxHealth = 1f, recovery = 1f, armor = 1f, moveSpeed = 1f, might = 1f,
            area = 1f, speed = 1f, duration = 1f, amount = 1, cooldown = 1f,
            luck = 1f, growth = 1f, greed = 1f, curse = 1f, magnet = 1f, revival = 1
        };

        foreach (Buff b in activeBuffs)
        {
            BuffData.Stats bd = b.GetData();
            switch (bd.modifierType)
            {
                case BuffData.ModifierType.additive:
                    // 将增益效果的加法修饰符添加到实际统计数据中。
                    actualStats += bd.playerModifier;
                    break;
                case BuffData.ModifierType.multiplicative:
                    // 将增益效果的乘法修饰符应用到倍数变量上。
                    multiplier *= bd.playerModifier;
                    break;
            }
        }
        actualStats *= multiplier;
        
        collector.SetRadius(actualStats.magnet);
    }

    /// <summary>
    /// 增加指定数量的经验值，并检查是否满足升级条件。
    /// </summary>
    /// <param name="amount">要增加的经验值数量。</param>
    public void IncreaseExperience(int amount)
    {
        experience += amount;
    
        LevelUpChecker();
        
        UpdateExpBar();
    }
    
    /// <summary>
    /// 检查当前经验值是否达到升级所需经验上限，若满足则提升等级并更新经验上限。
    /// </summary>
    private void LevelUpChecker()
    {
        if (experience >= experienceCap)
        {
            level++;
            experience -= experienceCap;

            int experienceCapIncrease = 0;
            foreach (LevelRange range in levelRanges)
            {
                if (level >= range.startLevel && level <= range.endLevel)
                {
                    experienceCapIncrease = range.experienceCapIncrease;
                    break;
                }
            }
            experienceCap += experienceCapIncrease;
            
            UpdateLevelText();
            
            GameManager.instance.StartLevelUp();
            
            //多次升级
            if (experience >= experienceCap)
                LevelUpChecker();
        }
    }

    /// <summary>
    /// 更新经验条的填充比例。
    /// </summary>
    private void UpdateExpBar()
    {
        expBar.fillAmount = (float)experience / experienceCap;
    }

    /// <summary>
    /// 更新等级文本显示。
    /// </summary>
    private void UpdateLevelText()
    {
        levelText.text = "LV " + level.ToString();
    }
    

    /// <summary>
    /// 处理玩家受到伤害的逻辑，包括减血、播放特效、进入无敌状态和死亡判断。
    /// </summary>
    /// <param name="dmg">受到的伤害值。</param>
    public override void TakeDamage(float dmg)
    {
        // 如果玩家当前不是无敌状态，则减少生命值并启动无敌
        if (!isInvincible)
        {
            // 在造成伤害之前考虑护甲。
            dmg -= actualStats.armor;

            if (dmg > 0)
            {
                // 造成伤害。
                CurrentHealth -= dmg;

                // 如果有指定的伤害效果，则播放它。
                if (damageEffect) Destroy(Instantiate(damageEffect, transform.position, Quaternion.identity), 5f);

                if (CurrentHealth <= 0)
                {
                    Kill();
                }
            }
            else
            {
                // 如果有指定的阻挡效果，则播放它。
                if (blockedEffect) Destroy(Instantiate(blockedEffect, transform.position, Quaternion.identity), 5f);
            }

            invincibilityTimer = invincibilityDuration;
            isInvincible = true;
        }
    }

    /// <summary>
    /// 更新生命条的填充比例。
    /// </summary>
    void UpdateHealthBar()
    {
        // 更新生命条
        healthBar.fillAmount = CurrentHealth / actualStats.maxHealth;
    }

    /// <summary>
    /// 处理玩家死亡逻辑，调用游戏结束方法。
    /// </summary>
    public override void Kill()
    {
        if (!GameManager.instance.isGameOver)
        {
            GameManager.instance.AssignLevelReachedUI(level);
            GameManager.instance.GameOver();
        }
    }

    /// <summary>
    /// 恢复指定数量的生命值，不超过最大生命值。
    /// </summary>
    /// <param name="amount">要恢复的生命值数量。</param>
    public override void RestoreHealth(float amount)
    {
        if (CurrentHealth < actualStats.maxHealth)
        {
            CurrentHealth += amount;
            
            if (CurrentHealth > actualStats.maxHealth)
            {
                CurrentHealth = actualStats.maxHealth;
            }
        }
    }

    /// <summary>
    /// 每帧自动恢复生命值，基于恢复速度属性进行恢复。
    /// </summary>
    private void Recover()
    {
        if (CurrentHealth < actualStats.maxHealth)
        {
            CurrentHealth += Stats.recovery * Time.deltaTime;
            
            if (CurrentHealth > actualStats.maxHealth)
            {
                CurrentHealth = actualStats.maxHealth;
            }
        }
    }
    
    /// <summary>
    /// 表示一个等级范围的数据结构，用于定义不同等级区间对应的经验值增长量。
    /// </summary>
    [Serializable]
    public class LevelRange
    {
        /// <summary>
        /// 当前等级范围的起始等级。
        /// </summary>
        public int startLevel;

        /// <summary>
        /// 当前等级范围的结束等级。
        /// </summary>
        public int endLevel;

        /// <summary>
        /// 经验值上限的增长量。
        /// </summary>
        public int experienceCapIncrease;
    }
}
