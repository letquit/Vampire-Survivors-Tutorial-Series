using System;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// 敌人属性管理类，用于控制敌人的移动速度、生命值和伤害等属性
/// </summary>
public class EnemyStats : MonoBehaviour
{
    public EnemyScriptableObject enemyData;
    
    [HideInInspector]
    public float currentMoveSpeed;
    [HideInInspector]
    public float currentHealth;
    [HideInInspector]
    public float currentDamage;

    public float despawnDistance = 20f;
    
    private Transform player;
    private EnemySpawner enemySpawner;
    
    /// <summary>
    /// 在对象唤醒时初始化敌人的各项属性值
    /// 从enemyData中获取初始的移动速度、最大生命值和伤害值
    /// </summary>
    private void Awake()
    {
        currentMoveSpeed = enemyData.moveSpeed;
        currentHealth = enemyData.maxHealth;
        currentDamage = enemyData.damage;
    }

    /// <summary>
    /// 在Start阶段获取玩家对象和敌人生成器的引用
    /// </summary>
    private void Start()
    {
        player = FindFirstObjectByType<PlayerStats>().transform;
        enemySpawner = FindFirstObjectByType<EnemySpawner>();
    }

    /// <summary>
    /// 每帧检查敌人与玩家的距离，如果超出设定的消失距离则重新定位敌人位置
    /// </summary>
    private void Update()
    {
        if (Vector2.Distance(transform.position, player.position) >= despawnDistance)
        {
            ReturnEnemy();
        }
    }

    /// <summary>
    /// 对敌人造成伤害
    /// </summary>
    /// <param name="damage">造成的伤害值</param>
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        // 检查敌人是否死亡
        if (currentHealth <= 0)
        {
            Kill();
        }
    }

    /// <summary>
    /// 销毁敌人游戏对象
    /// </summary>
    private void Kill()
    {
        Destroy(gameObject);
    }

    /// <summary>
    /// 当敌人与玩家发生碰撞时，对玩家造成伤害
    /// </summary>
    /// <param name="other">碰撞的另一个游戏对象的碰撞信息</param>
    private void OnCollisionStay2D(Collision2D other)
    {
        // 检查碰撞的对象是否为玩家
        if (other.gameObject.CompareTag("Player"))
        {
            PlayerStats player = other.gameObject.GetComponent<PlayerStats>();
            player.TakeDamage(currentDamage);
        }
    }

    /// <summary>
    /// 当敌人被销毁时通知生成器减少敌人计数
    /// </summary>
    private void OnDestroy()
    {
        if (enemySpawner != null)
        {
            enemySpawner.OnEnemyKilled();
        }
    }
    
    /// <summary>
    /// 将敌人重新定位到玩家周围的随机位置
    /// 如果存在敌人生成器，则使用其定义的生成范围；否则使用默认范围
    /// </summary>
    private void ReturnEnemy()
    {
        // 使用EnemySpawner的生成范围设置
        if (enemySpawner != null)
        {
            transform.position = player.position + 
                EnemyUtilities.GetRandomEnemyPosition(
                    enemySpawner.minSpawnDistance, 
                    enemySpawner.maxSpawnDistance);
        }
        else
        {
            // 如果找不到EnemySpawner，则使用默认值
            transform.position = player.position + EnemyUtilities.GetRandomEnemyPosition();
        }
    }
}
