using UnityEngine;

/// <summary>
/// 武器行为基类，包含所有武器的共同属性和方法。
/// 该类为抽象类，不能直接实例化，必须由具体的武器行为子类继承实现。
/// </summary>
public abstract class WeaponBehaviour : MonoBehaviour
{
    /// <summary>
    /// 武器数据配置文件，包含武器的基本属性，如伤害、速度、冷却时间等。
    /// </summary>
    public WeaponScriptableObject weaponData;
    
    /// <summary>
    /// 武器销毁时间（秒），用于在指定时间后自动销毁武器对象。
    /// </summary>
    public float destroyAfterSeconds;

    /// <summary>
    /// 当前武器伤害值，用于计算对敌人或可破坏物体造成的伤害。
    /// </summary>
    protected float currentDamage;
    
    /// <summary>
    /// 当前武器速度，用于控制武器移动或攻击的速度。
    /// </summary>
    protected float currentSpeed;
    
    /// <summary>
    /// 当前武器冷却时间，用于限制武器的攻击频率。
    /// </summary>
    protected float currentCooldownDuration;
    
    /// <summary>
    /// 当前武器穿透次数，表示武器可以穿透多少个敌人或物体。
    /// </summary>
    protected int currentPierce;

    /// <summary>
    /// 初始化武器属性，从武器数据配置文件中读取初始值。
    /// 该方法在对象被激活时调用，用于设置武器的基础属性。
    /// </summary>
    protected virtual void Awake()
    {
        currentDamage = weaponData.damage;
        currentSpeed = weaponData.speed;
        currentCooldownDuration = weaponData.cooldownDuration;
        currentPierce = weaponData.pierce;
    }

    /// <summary>
    /// 获取当前武器的实际伤害值，该值会受到玩家当前力量属性的影响。
    /// </summary>
    /// <returns>经过玩家力量加成后的实际伤害值。</returns>
    public float GetCurrentDamage()
    {
        return currentDamage *= FindFirstObjectByType<PlayerStats>().CurrentMight;
    }
    
    /// <summary>
    /// 在指定时间后销毁武器游戏对象。
    /// 该方法在对象初始化后调用，用于自动销毁武器。
    /// </summary>
    protected virtual void Start()
    {
        Destroy(gameObject, destroyAfterSeconds);
    }

    /// <summary>
    /// 处理武器与其他物体的碰撞事件。
    /// 根据碰撞对象的标签判断是否为敌人或可破坏道具，并执行相应的处理逻辑。
    /// </summary>
    /// <param name="other">与武器发生碰撞的另一个碰撞体。</param>
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
                breakable.TakeDamage(GetCurrentDamage());
                // 如果是投射物武器，则减少穿透次数
                if (this is ProjectileWeaponBehaviour)
                {
                    ReducePierce();
                }
            }
        }
    }
    
    /// <summary>
    /// 减少武器穿透次数，当穿透次数用完时销毁武器。
    /// 该方法用于控制投射物类武器的穿透能力。
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
    /// 当武器击中敌人时调用的抽象方法，由子类实现具体逻辑。
    /// 子类需要根据武器类型实现对敌人造成伤害或其他效果的逻辑。
    /// </summary>
    /// <param name="enemy">被击中的敌人对象。</param>
    protected abstract void OnEnemyHit(EnemyStats enemy);
}
