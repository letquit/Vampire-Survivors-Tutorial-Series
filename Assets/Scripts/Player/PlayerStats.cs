using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public CharacterScriptableObject characterData;

    private float currentHealth;
    private float currentRecovery;
    private float currentMoveSpeed;
    private float currentMight;
    private float currentProjectileSpeed;

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
    
    private void Awake()
    {
        currentHealth = characterData.maxHealth;
        currentRecovery = characterData.recovery;
        currentMoveSpeed = characterData.moveSpeed;
        currentMight = characterData.might;
        currentProjectileSpeed = characterData.projectileSpeed;
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

    private void Start()
    {
        experienceCap = levelRanges[0].experienceCapIncrease;
    }

    private void Update()
    {
        if (invincibilityTimer > 0)
        {
            invincibilityTimer -= Time.deltaTime;
        }
        else if (isInvincible)
        {
            isInvincible = false;
        }
    }

    public void IncreaseExperience(int amount)
    {
        experience += amount;
    
        LevelUpChecker();
    }
    
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

    public void Kill()
    {
        Debug.Log("PLAYER IS DEAD");
    }

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

    [Serializable]
    public class LevelRange
    {
        public int startLevel;
        public int endLevel;
        public int experienceCapIncrease;
    }
}
