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
    private CharacterScriptableObject characterData;

    float currentHealth;
    float currentRecovery;
    float currentMoveSpeed;
    float currentMight;
    float currentProjectileSpeed;
    float currentMagnet;

    #region Current Stats Properties
    /// <summary>
    /// 获取或设置当前生命值，并更新UI显示。
    /// </summary>
    public float CurrentHealth
    {
        get { return currentHealth; }
        set
        {
            if (currentHealth != value)
            {
                currentHealth = value;
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.currentHealthDisplay.text = "Health: " + currentHealth;
                }
                UpdateHealthBar();
            }
        }
    }

    /// <summary>
    /// 获取或设置当前恢复速度，并更新UI显示。
    /// </summary>
    public float CurrentRecovery
    {
        get { return currentRecovery; }
        set
        {
            if (currentRecovery != value)
            {
                currentRecovery = value;
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.currentRecoveryDisplay.text = "Recovery: " + currentRecovery;
                }
            }
        }
    }
    
    /// <summary>
    /// 获取或设置当前移动速度，并更新UI显示。
    /// </summary>
    public float CurrentMoveSpeed
    {
        get { return currentMoveSpeed; }
        set
        {
            if (currentMoveSpeed != value)
            {
                currentMoveSpeed = value;
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.currentMoveSpeedDisplay.text = "Move Speed: " + currentMoveSpeed;
                }
            }
        }
    }
    
    /// <summary>
    /// 获取或设置当前力量值，并更新UI显示。
    /// </summary>
    public float CurrentMight
    {
        get { return currentMight; }
        set
        {
            if (currentMight != value)
            {
                currentMight = value;
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.currentMightDisplay.text = "Might: " + currentMight;
                }
            }
        }
    }
    
    /// <summary>
    /// 获取或设置当前投射物速度，并更新UI显示。
    /// </summary>
    public float CurrentProjectileSpeed
    {
        get { return currentProjectileSpeed; }
        set
        {
            if (currentProjectileSpeed != value)
            {
                currentProjectileSpeed = value;
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.currentProjectileSpeedDisplay.text = "Projectile Speed: " + currentProjectileSpeed;
                }
            }
        }
    }
    
    /// <summary>
    /// 获取或设置当前磁力值，并更新UI显示。
    /// </summary>
    public float CurrentMagnet
    {
        get { return currentMagnet; }
        set
        {
            if (currentMagnet != value)
            {
                currentMagnet = value;
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.currentMagnetDisplay.text = "Magnet: " + currentMagnet;
                }
            }
        }
    }
    #endregion
    
    [Header("Experience/Level")]
    public int experience = 0;
    public int level = 1;
    public int experienceCap;
    // [Header("Experience/Level")]
    // public int experience = 0;
    // public int level = 1;
    // public int experienceCap = 100;
    // public int experienceCapIncrease;

    [Header("I-Frames")] 
    public float invincibilityDuration;
    private float invincibilityTimer;
    private bool isInvincible;

    public List<LevelRange> levelRanges;

    private InventoryManager inventory;
    public int weaponIndex;
    public int passiveItemIndex;

    [Header("UI")]
    public Image healthBar;
    public Image expBar;
    public TextMeshProUGUI levelText;

    public GameObject secondWeaponTest;
    public GameObject firstPassiveItemTest, secondPassiveItemTest;
    
    /// <summary>
    /// 在 Awake 阶段初始化角色数据，设置初始属性并生成初始武器。
    /// </summary>
    private void Awake()
    {
        characterData = CharacterSelector.GetData();
        CharacterSelector.Instance.DestroySingleton();
        
        inventory = GetComponent<InventoryManager>();
        
        CurrentHealth = characterData.maxHealth;
        CurrentRecovery = characterData.recovery;
        CurrentMoveSpeed = characterData.moveSpeed;
        CurrentMight = characterData.might;
        CurrentProjectileSpeed = characterData.projectileSpeed;
        CurrentMagnet = characterData.magnet;
        
        SpawnWeapon(characterData.startingWeapon);
        // SpawnWeapon(secondWeaponTest);
        // SpawnPassiveItem(firstPassiveItemTest);
        SpawnPassiveItem(secondPassiveItemTest);
    }

    // public void IncreaseExperience(int amount)
    // {
    //     experience += amount;
    //
    //     LevelUpChecker();
    // }
    //
    // private void LevelUpChecker()
    // {
    //     if (experience >= experienceCap)
    //     {
    //         level++;
    //         experience -= experienceCap;
    //         experienceCap += experienceCapIncrease;
    //     }
    // }

    /// <summary>
    /// 在 Start 阶段初始化经验上限，根据第一个等级范围设定初始经验值上限。
    /// </summary>
    private void Start()
    {
        experienceCap = levelRanges[0].experienceCapIncrease;

        GameManager.Instance.currentHealthDisplay.text = "Health: " + currentHealth;
        GameManager.Instance.currentRecoveryDisplay.text = "Recovery: " + currentRecovery;
        GameManager.Instance.currentMoveSpeedDisplay.text = "Move Speed: " + currentMoveSpeed;
        GameManager.Instance.currentMightDisplay.text = "Might: " + currentMight;
        GameManager.Instance.currentProjectileSpeedDisplay.text = "Projectile Speed: " + currentProjectileSpeed;
        GameManager.Instance.currentMagnetDisplay.text = "Magnet: " + currentMagnet;
        
        GameManager.Instance.AssignChosenCharacterUI(characterData);

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
            
            GameManager.Instance.StartLevelUp();
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
    /// 对玩家造成伤害，若处于无敌状态则忽略伤害。
    /// 若生命值降至0或以下，则调用死亡方法。
    /// </summary>
    /// <param name="dmg">造成的伤害值。</param>
    public void TakeDamage(float dmg)
    {
        if (!isInvincible)
        {
            CurrentHealth -= dmg;
            
            invincibilityTimer = invincibilityDuration;
            isInvincible = true;
        
            if (CurrentHealth <= 0)
            {
                Kill();
            }
        }
    }

    /// <summary>
    /// 更新生命条的填充比例。
    /// </summary>
    private void UpdateHealthBar()
    {
        healthBar.fillAmount = currentHealth / characterData.maxHealth;
    }

    /// <summary>
    /// 玩家死亡时调用的方法，输出日志信息。
    /// </summary>
    public void Kill()
    {
        if (!GameManager.Instance.isGameOver)
        {
            GameManager.Instance.AssignLevelReachedUI(level);
            GameManager.Instance.AssignChosenWeaponsAndPassiveItemsUI(inventory.weaponUISlots, inventory.passiveItemUISlots);
            GameManager.Instance.GameOver();
        }
    }

    /// <summary>
    /// 恢复指定数量的生命值，不超过最大生命值。
    /// </summary>
    /// <param name="amount">要恢复的生命值数量。</param>
    public void RestoreHealth(int amount)
    {
        if (CurrentHealth < characterData.maxHealth)
        {
            CurrentHealth += amount;
            if (CurrentHealth > characterData.maxHealth)
            {
                CurrentHealth = characterData.maxHealth;
            }
        }
    }

    /// <summary>
    /// 每帧自动恢复生命值，基于恢复速度属性进行恢复。
    /// </summary>
    private void Recover()
    {
        if (CurrentHealth < characterData.maxHealth)
        {
            CurrentHealth += currentRecovery * Time.deltaTime;
            
            if (CurrentHealth > characterData.maxHealth)
            {
                CurrentHealth = characterData.maxHealth;
            }
        }
    }
    
    /// <summary>
    /// 在玩家位置生成一个武器实例，并将其添加到已生成武器列表中。
    /// </summary>
    /// <param name="weapon">要生成的武器预制体。</param>
    public void SpawnWeapon(GameObject weapon)
    {
        if (weaponIndex >= inventory.weaponSlots.Count - 1)
        {
            Debug.LogError("InventorySlots already full");
            return;
        }
        
        GameObject spawnedWeapon = Instantiate(weapon, transform.position, Quaternion.identity);
        spawnedWeapon.transform.SetParent(transform);
        inventory.AddWeapon(weaponIndex, spawnedWeapon.GetComponent<WeaponController>());

        weaponIndex++;
    }
    
    /// <summary>
    /// 在玩家位置生成一个被动道具实例，并将其添加到已生成被动道具列表中。
    /// </summary>
    /// <param name="passiveItem">要生成的被动道具预制体。</param>
    public void SpawnPassiveItem(GameObject passiveItem)
    {
        if (passiveItemIndex >= inventory.passiveItemSlots.Count - 1)
        {
            Debug.LogError("InventorySlots already full");
            return;
        }
        
        GameObject spawnedPassiveItem = Instantiate(passiveItem, transform.position, Quaternion.identity);
        spawnedPassiveItem.transform.SetParent(transform);
        inventory.AddPassiveItem(passiveItemIndex, spawnedPassiveItem.GetComponent<PassiveItem>());

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
