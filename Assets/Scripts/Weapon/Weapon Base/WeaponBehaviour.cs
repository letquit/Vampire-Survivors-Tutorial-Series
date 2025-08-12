using UnityEngine;

/// <summary>
/// 武器行为基类，包含所有武器的共同属性和方法
/// </summary>
public abstract class WeaponBehaviour : MonoBehaviour
{
    /// <summary>
    /// 武器数据配置文件，包含武器的基本属性
    /// </summary>
    public WeaponScriptableObject weaponData;
    
    /// <summary>
    /// 武器销毁时间（秒）
    /// </summary>
    public float destroyAfterSeconds;

    /// <summary>
    /// 当前武器伤害值
    /// </summary>
    protected float currentDamage;
    
    /// <summary>
    /// 当前武器速度
    /// </summary>
    protected float currentSpeed;
    
    /// <summary>
    /// 当前武器冷却时间
    /// </summary>
    protected float currentCooldownDuration;
    
    /// <summary>
    /// 当前武器穿透次数
    /// </summary>
    protected int currentPierce;

    /// <summary>
    /// 初始化武器属性，从武器数据配置文件中读取初始值
    /// </summary>
    protected virtual void Awake()
    {
        currentDamage = weaponData.damage;
        currentSpeed = weaponData.speed;
        currentCooldownDuration = weaponData.cooldownDuration;
        currentPierce = weaponData.pierce;
    }

    /// <summary>
    /// 在指定时间后销毁武器游戏对象
    /// </summary>
    protected virtual void Start()
    {
        Destroy(gameObject, destroyAfterSeconds);
    }

    /// <summary>
    /// 处理武器与其他物体的碰撞事件
    /// </summary>
    /// <param name="other">碰撞的另一个碰撞体</param>
    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        // 检查是否与敌人碰撞
        if (other.CompareTag("Enemy"))
        {
            EnemyStats enemy = other.GetComponent<EnemyStats>();
            OnEnemyHit(enemy);
        }
        // 检查是否与可破坏道具碰撞
        else if (other.CompareTag("Prop"))
        {
            if (other.gameObject.TryGetComponent(out BreakableProps breakable))
            {
                breakable.TakeDamage(currentDamage);
                // 如果是投射物武器，则减少穿透次数
                if (this is ProjectileWeaponBehaviour)
                {
                    ReducePierce();
                }
            }
        }
    }
    
    /// <summary>
    /// 减少武器穿透次数，当穿透次数用完时销毁武器
    /// </summary>
    private void ReducePierce()
    {
        currentPierce--;
        if (currentPierce <= 0)
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 当武器击中敌人时调用的抽象方法，由子类实现具体逻辑
    /// </summary>
    /// <param name="enemy">被击中的敌人</param>
    protected abstract void OnEnemyHit(EnemyStats enemy);
}

