using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 玩家状态管理类，继承自 MonoBehaviour。
/// 负责管理玩家的生命值、经验值、等级、武器生成以及无敌状态等核心属性和逻辑。
/// </summary>
public class PlayerStats : MonoBehaviour
{
    private CharacterScriptableObject characterData;

    [HideInInspector]
    public float currentHealth;
    [HideInInspector]
    public float currentRecovery;
    [HideInInspector]
    public float currentMoveSpeed;
    [HideInInspector]
    public float currentMight;
    [HideInInspector]
    public float currentProjectileSpeed;
    [HideInInspector]
    public float currentMagnet;

    public List<GameObject> spawnedWeapons;

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
    
    /// <summary>
    /// 在 Awake 阶段初始化角色数据，设置初始属性并生成初始武器。
    /// </summary>
    private void Awake()
    {
        characterData = CharacterSelector.GetData();
        CharacterSelector.Instance.DestroySingleton();
        
        currentHealth = characterData.maxHealth;
        currentRecovery = characterData.recovery;
        currentMoveSpeed = characterData.moveSpeed;
        currentMight = characterData.might;
        currentProjectileSpeed = characterData.projectileSpeed;
        currentMagnet = characterData.magnet;
        
        SpawnWeapon(characterData.startingWeapon);
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
        }
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
            currentHealth -= dmg;
            
            invincibilityTimer = invincibilityDuration;
            isInvincible = true;
        
            if (currentHealth <= 0)
            {
                Kill();
            }   
        }
    }

    /// <summary>
    /// 玩家死亡时调用的方法，输出日志信息。
    /// </summary>
    public void Kill()
    {
        Debug.Log("PLAYER IS DEAD");
    }

    /// <summary>
    /// 恢复指定数量的生命值，不超过最大生命值。
    /// </summary>
    /// <param name="amount">要恢复的生命值数量。</param>
    public void RestoreHealth(int amount)
    {
        if (currentHealth < characterData.maxHealth)
        {
            currentHealth += amount;

            if (currentHealth > characterData.maxHealth)
            {
                currentHealth = characterData.maxHealth;
            }
        }
    }

    /// <summary>
    /// 每帧自动恢复生命值，基于恢复速度属性进行恢复。
    /// </summary>
    private void Recover()
    {
        if (currentHealth < characterData.maxHealth)
        {
            currentHealth += currentRecovery * Time.deltaTime;
            
            if (currentHealth > characterData.maxHealth)
            {
                currentHealth = characterData.maxHealth;
            }
        }
    }
    
    /// <summary>
    /// 在玩家位置生成一个武器实例，并将其添加到已生成武器列表中。
    /// </summary>
    /// <param name="weapon">要生成的武器预制体。</param>
    public void SpawnWeapon(GameObject weapon)
    {
        GameObject spawnedWeapon = Instantiate(weapon, transform.position, Quaternion.identity);
        spawnedWeapon.transform.SetParent(transform);
        spawnedWeapons.Add(spawnedWeapon);
    }

    /// <summary>
    /// 表示一个等级范围的数据结构，用于定义不同等级区间对应的经验值增长量。
    /// </summary>
    [Serializable]
    public class LevelRange
    {
        public int startLevel;
        public int endLevel;
        public int experienceCapIncrease;
    }
}
