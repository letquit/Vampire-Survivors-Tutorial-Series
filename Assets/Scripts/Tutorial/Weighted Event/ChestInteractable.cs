using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

/// <summary>
/// 宝箱交互组件，处理宝箱的开启、内容物生成等交互逻辑
/// </summary>
public class ChestInteractable : InteractableBase
{
    [Header("Chest Opening Sprites")] 
    [SerializeField] private SpriteRenderer lidSpriteRenderer;
    [SerializeField] private Sprite openChestLidSprite;
    [Space]
    
    [Header("Spawn Coins")]
    [SerializeField] private Rigidbody2D coinToSpawn;
    [SerializeField] private int numberOfCoinsToSpawn = 100;
    [SerializeField] private float explosionForce = 10f;
    [SerializeField, Range(0f, 0.5f)] private float explosionArc = 0.5f;
    [SerializeField] private bool delayBetweenSpawns = false;
    [SerializeField] private Transform spawnTransform;
    [Space]
    
    [Header("Spawn Enemies")] 
    [SerializeField] private GameObject[] enemiesToSpawn;
    [SerializeField] private GameObject enemySpawnParticles;
    [SerializeField] private int numOfEnemiesToSpawn = 3;
    [SerializeField, Range(0f, 15f)] private float enemySpawnOffset = 2f;
    [Space]
    
    [Header("Spawn Health Potion")]
    [SerializeField] private Rigidbody2D healthPotionToSpawn;
    [SerializeField] private float upwardForce = 5f;
    [Space]

    [Header("Chest Drops")]
    [SerializeField] private ChestInteractableEvents[] chestInteractionEvents;
    [Space]
    
    private bool _isOpen;

    /// <summary>
    /// 重写的交互方法，处理宝箱开启逻辑
    /// </summary>
    public override void Interact()
    {
        if (!_isOpen)
        {
            OpenChest();

            DetermineAndFireChestEvent();
        }
    }

    /// <summary>
    /// 开启宝箱，更新宝箱外观和交互状态
    /// </summary>
    private void OpenChest()
    {
        _isOpen = true;
        lidSpriteRenderer.sprite = openChestLidSprite;
        interactableFloatingIcon.SetActive(false);
        CanStillInteract = false;
    }

    /// <summary>
    /// 根据概率确定并触发宝箱事件
    /// </summary>
    private void DetermineAndFireChestEvent()
    {
        // 计算所有事件的总概率
        float totalChance = 0f;
        foreach (ChestInteractableEvents interactableEvents in chestInteractionEvents)
        {
            totalChance += interactableEvents.DropChance;
        }

        // 生成0到总概率之间的随机数
        float rand = Random.Range(0f, totalChance);
        float cumulativeChance = 0f;

        // 遍历事件列表，找到对应的事件并触发
        foreach (ChestInteractableEvents interactableEvents in chestInteractionEvents)
        {
            cumulativeChance += interactableEvents.DropChance;

            if (rand <= cumulativeChance)
            {
                interactableEvents.ChestInteractionEvent.Invoke();
                return;
            }
        }
    }

    #region Event Logic
    
    #region Spawn Coins

    /// <summary>
    /// 生成硬币，根据是否需要延迟决定生成方式
    /// </summary>
    public void SpawnCoins()
    {
        if (!delayBetweenSpawns)
        {
            for (int i = 0; i < numberOfCoinsToSpawn; i++)
            {
                Rigidbody2D coinRb = Instantiate(coinToSpawn, spawnTransform.position, Quaternion.identity);
                Explosion(coinRb);
            }
        }
        else
        {
            StartCoroutine(SpawnCoinsWithDelay());
        }
    }

    /// <summary>
    /// 带延迟的硬币生成协程
    /// </summary>
    /// <returns>协程迭代器</returns>
    private IEnumerator SpawnCoinsWithDelay()
    {
        for (int i = 0; i < numberOfCoinsToSpawn; i++)
        {
            Rigidbody2D coinRb = Instantiate(coinToSpawn, spawnTransform.position, Quaternion.identity);
            Explosion(coinRb);
            yield return null;
        }
    }

    /// <summary>
    /// 为刚体应用爆炸力
    /// </summary>
    /// <param name="rb">需要应用力的刚体组件</param>
    private void Explosion(Rigidbody2D rb)
    {
        Vector2 randDir = new Vector2(Random.Range(-explosionArc, explosionArc), 1f);
        Vector2 force = randDir.normalized * explosionForce;
        rb.AddForce(force, ForceMode2D.Impulse);
    }

    #endregion
    
    #region Spawn Enemies

    /// <summary>
    /// 生成敌人，随机选择敌人类型并在随机位置生成
    /// </summary>
    public void SpawnEnemies()
    {
        for (int i = 0; i < numOfEnemiesToSpawn; i++)
        {
            int randIndex = Random.Range(0, enemiesToSpawn.Length);

            float randX = Random.Range(-enemySpawnOffset, enemySpawnOffset);
            float randY = Random.Range(-enemySpawnOffset, enemySpawnOffset);
            
            Vector2 spawnPos = (Vector2)spawnTransform.position + new Vector2(randX, randY).normalized;
            
            GameObject enemy = Instantiate(enemiesToSpawn[randIndex], spawnPos, Quaternion.identity);
            GameObject spawnParticles = Instantiate(enemySpawnParticles, spawnPos, Quaternion.identity);
            spawnParticles.transform.localScale = enemy.transform.localScale;
        }
    }

    #endregion
    
    #region Spawn Health Potion

    /// <summary>
    /// 生成生命药水并施加向上的力
    /// </summary>
    public void SpawnHealthPotion()
    {
        Rigidbody2D rb = Instantiate(healthPotionToSpawn, spawnTransform.position, Quaternion.identity);
        
        Vector2 force = Vector2.up * upwardForce;
        rb.AddForce(force, ForceMode2D.Impulse);
    }

    #endregion
    
    #endregion
}

/// <summary>
/// 宝箱交互事件数据类，包含事件名称、概率和Unity事件
/// </summary>
[Serializable]
public class ChestInteractableEvents
{
    public string EventName;
    [Space] 
    [Space] 
    [Range(0f, 1f)] public float DropChance = 0.5f;
    public UnityEvent ChestInteractionEvent;
}