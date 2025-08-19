using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 玩家状态管理类，继承自 MonoBehaviour。
/// 负责管理玩家的生命值、经验值、等级、武器生成以及无敌状态等核心属性和逻辑。
/// </summary>
public class PlayerStats : MonoBehaviour
{
    private CharacterData characterData;
    public CharacterData.Stats baseStats;
    [SerializeField] 
    private CharacterData.Stats actualStats;

    private float health;

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
                if (GameManager.instance != null)
                {
                    GameManager.instance.currentHealthDisplay.text = string.Format(
                        "Health: {0} / {1}",
                        health, actualStats.maxHealth
                    );
                }
            }
        }
    }

    /// <summary>
    /// 获取或设置最大生命值，并更新UI显示。
    /// </summary>
    public float MaxHealth
    {
        get { return actualStats.maxHealth; }
        set
        {
            if (actualStats.maxHealth != value)
            {
                actualStats.maxHealth = value;
                if (GameManager.instance != null)
                {
                    GameManager.instance.currentHealthDisplay.text = string.Format(
                        "Health: {0} / {1}",
                        health, actualStats.maxHealth
                    );
                }
            }
        }
    }
    
    /// <summary>
    /// 获取或设置当前恢复速度。
    /// </summary>
    public float CurrentRecovery
    {
        get { return Recovery; }
        set { Recovery = value; }
    }

    /// <summary>
    /// 获取或设置恢复速度，并更新UI显示。
    /// </summary>
    public float Recovery
    {
        get { return actualStats.recovery; }
        set
        {
            if (actualStats.recovery != value)
            {
                actualStats.recovery = value;
                if (GameManager.instance != null)
                {
                    GameManager.instance.currentRecoveryDisplay.text = "Recovery: " + actualStats.recovery;
                }
            }
        }
    }

    /// <summary>
    /// 获取或设置当前移动速度。
    /// </summary>
    public float CurrentMoveSpeed
    {
        get { return MoveSpeed; }
        set { MoveSpeed = value; }
    }
    
    /// <summary>
    /// 获取或设置移动速度，并更新UI显示。
    /// </summary>
    public float MoveSpeed
    {
        get { return actualStats.moveSpeed; }
        set
        {
            if (actualStats.moveSpeed != value)
            {
                actualStats.moveSpeed = value;
                if (GameManager.instance != null)
                {
                    GameManager.instance.currentMoveSpeedDisplay.text = "Move Speed: " + actualStats.moveSpeed;
                }
            }
        }
    }

    /// <summary>
    /// 获取或设置当前力量值。
    /// </summary>
    public float CurrentMight
    {
        get { return Might; }
        set { Might = value; }
    }

    /// <summary>
    /// 获取或设置力量值，并更新UI显示。
    /// </summary>
    public float Might
    {
        get { return actualStats.might; }
        set
        {
            if (actualStats.might != value)
            {
                actualStats.might = value;
                if (GameManager.instance != null)
                {
                    GameManager.instance.currentMightDisplay.text = "Might: " + actualStats.might;
                }
            }
        }
    }
    
    /// <summary>
    /// 获取或设置当前弹道速度。
    /// </summary>
    public float CurrentProjectileSpeed
    {
        get { return Speed; }
        set { Speed = value; }
    }

    /// <summary>
    /// 获取或设置弹道速度，并更新UI显示。
    /// </summary>
    public float Speed
    {
        get { return actualStats.speed; }
        set
        {
            if (actualStats.speed != value)
            {
                actualStats.speed = value;
                if (GameManager.instance != null)
                {
                    GameManager.instance.currentProjectileSpeedDisplay.text = "Projectile Speed: " + actualStats.speed;
                }
            }
        }
    }

    /// <summary>
    /// 获取或设置当前磁力值。
    /// </summary>
    public float CurrentMagnet
    {
        get { return Magnet; }
        set { Magnet = value; }
    }
    
    /// <summary>
    /// 获取或设置磁力值，并更新UI显示。
    /// </summary>
    public float Magnet
    {
        get { return actualStats.magnet; }
        set
        {
            if (actualStats.magnet != value)
            {
                actualStats.magnet = value;
                if (GameManager.instance != null)
                {
                    GameManager.instance.currentMagnetDisplay.text = "Magnet: " + actualStats.magnet;
                }
            }
        }
    }
    #endregion

    public ParticleSystem damageEffect;
    
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
    public int weaponIndex;
    public int passiveItemIndex;

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
        characterData = CharacterSelector.GetData();
        
        if(CharacterSelector.instance)
            CharacterSelector.instance.DestroySingleton();

        inventory = GetComponent<PlayerInventory>();
        collector = GetComponentInChildren<PlayerCollector>();

        // 分配变量
        baseStats = actualStats = characterData.stats;
        collector.SetRadius(actualStats.magnet);
        health = actualStats.maxHealth;
        playerAnimator = GetComponent<PlayerAnimator>();
        if (characterData.controller)
            playerAnimator.SetAnimationController(characterData.controller);
    }

    /// <summary>
    /// 启动时添加初始武器、初始化经验上限并更新UI显示。
    /// </summary>
    void Start()
    {
        // 生成初始武器
        inventory.Add(characterData.StartingWeapon);

        // 初始化经验上限为第一个等级范围的经验增长量
        experienceCap = levelRanges[0].experienceCapIncrease;

        // 设置当前属性显示
        GameManager.instance.currentHealthDisplay.text = "Health: " + CurrentHealth;
        GameManager.instance.currentRecoveryDisplay.text = "Recovery: " + CurrentRecovery;
        GameManager.instance.currentMoveSpeedDisplay.text = "Move Speed: " + CurrentMoveSpeed;
        GameManager.instance.currentMightDisplay.text = "Might: " + CurrentMight;
        GameManager.instance.currentProjectileSpeedDisplay.text = "Projectile Speed: " + CurrentProjectileSpeed;
        GameManager.instance.currentMagnetDisplay.text = "Magnet: " + CurrentMagnet;

        GameManager.instance.AssignChosenCharacterUI(characterData);

        UpdateHealthBar();
        UpdateExpBar();
        UpdateLevelText();
    }

    /// <summary>
    /// 每帧更新无敌计时器，并执行恢复生命值的逻辑。
    /// </summary>
    private void Update()
    {
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
    /// </summary>
    public void RecalculateStats()
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
    public void TakeDamage(float dmg)
    {
        // 如果玩家当前不是无敌状态，则减少生命值并启动无敌
        if (!isInvincible)
        {
            CurrentHealth -= dmg;

            // 如果有伤害特效，则播放它
            if (damageEffect) Destroy(Instantiate(damageEffect, transform.position, Quaternion.identity), 5f);

            invincibilityTimer = invincibilityDuration;
            isInvincible = true;

            if (CurrentHealth <= 0)
            {
                Kill();
            }

            UpdateHealthBar();
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
    public void Kill()
    {
        if (!GameManager.instance.isGameOver)
        {
            GameManager.instance.AssignLevelReachedUI(level);
            GameManager.instance.AssignChosenWeaponsAndPassiveItemsUI(inventory.weaponSlots, inventory.passiveSlots);
            GameManager.instance.GameOver();
        }
    }

    /// <summary>
    /// 恢复指定数量的生命值，不超过最大生命值。
    /// </summary>
    /// <param name="amount">要恢复的生命值数量。</param>
    public void RestoreHealth(int amount)
    {
        if (CurrentHealth < actualStats.maxHealth)
        {
            CurrentHealth += amount;
            
            if (CurrentHealth > actualStats.maxHealth)
            {
                CurrentHealth = actualStats.maxHealth;
            }
            
            UpdateHealthBar();
        }
    }

    /// <summary>
    /// 每帧自动恢复生命值，基于恢复速度属性进行恢复。
    /// </summary>
    private void Recover()
    {
        if (CurrentHealth < actualStats.maxHealth)
        {
            CurrentHealth += CurrentRecovery * Time.deltaTime;
            CurrentHealth += Recovery * Time.deltaTime;
            
            if (CurrentHealth > actualStats.maxHealth)
            {
                CurrentHealth = actualStats.maxHealth;
            }
            
            UpdateHealthBar();
        }
    }
    
    /// <summary>
    /// 在玩家位置生成一个武器实例，并将其添加到已生成武器列表中。
    /// </summary>
    /// <param name="weapon">要生成的武器预制体。</param>
    [System.Obsolete("旧函数，保留以兼容InventoryManager。将很快移除。")]
    public void SpawnWeapon(GameObject weapon)
    {
        if (weaponIndex >= inventory.weaponSlots.Count - 1)
        {
            Debug.LogError("InventorySlots already full");
            return;
        }
        
        GameObject spawnedWeapon = Instantiate(weapon, transform.position, Quaternion.identity);
        spawnedWeapon.transform.SetParent(transform);
        // inventory.AddWeapon(weaponIndex, spawnedWeapon.GetComponent<WeaponController>());

        weaponIndex++;
    }
    
    /// <summary>
    /// 在玩家位置生成一个被动道具实例，并将其添加到已生成被动道具列表中。
    /// </summary>
    /// <param name="passiveItem">要生成的被动道具预制体。</param>
    [System.Obsolete("现在不需要直接生成被动道具。")]
    public void SpawnPassiveItem(GameObject passiveItem)
    {
        // 检查槽位是否已满，如果满了则返回
        if (passiveItemIndex >= inventory.passiveSlots.Count - 1) // 必须减1因为列表从0开始
        {
            Debug.LogError("InventorySlots already full");
            return;
        }
        
        GameObject spawnedPassiveItem = Instantiate(passiveItem, transform.position, Quaternion.identity);
        spawnedPassiveItem.transform.SetParent(transform);
        // inventory.AddPassiveItem(passiveItemIndex, spawnedPassiveItem.GetComponent<PassiveItem>());

        passiveItemIndex++;
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
