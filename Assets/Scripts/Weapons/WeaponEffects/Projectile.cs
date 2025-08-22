using UnityEngine;

/// <summary>
/// 附加到所有投射物预制体上的组件。所有生成的投射物将沿着其面向的方向飞行，并在击中物体时造成伤害。
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : WeaponEffect
{
    /// <summary>
    /// 伤害来源枚举，用于指定伤害是由投射物本身还是其拥有者造成的。
    /// </summary>
    public enum DamageSource { projectile, owner };

    /// <summary>
    /// 指定伤害来源，默认为投射物自身。
    /// </summary>
    public DamageSource damageSource = DamageSource.projectile;

    /// <summary>
    /// 是否启用自动瞄准功能。
    /// </summary>
    public bool hasAutoAim = false;

    /// <summary>
    /// 投射物的旋转速度（每轴），仅Z轴通常用于2D游戏中的旋转。
    /// </summary>
    public Vector3 rotationSpeed = new Vector3(0, 0, 0);

    /// <summary>
    /// 投射物绑定的 Rigidbody2D 组件。
    /// </summary>
    protected Rigidbody2D rb;

    /// <summary>
    /// 当前投射物的穿透次数，每次命中敌人或可破坏物体会减少。
    /// </summary>
    protected int piercing;

    /// <summary>
    /// 在第一次帧更新前调用，用于初始化投射物的行为。
    /// </summary>
    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Weapon.Stats stats = weapon.GetStats();

        // 如果是动态刚体，则设置初始线速度和角速度
        if (rb.bodyType == RigidbodyType2D.Dynamic)
        {
            rb.angularVelocity = rotationSpeed.z;
            rb.linearVelocity = transform.right * stats.speed * weapon.Owner.Stats.speed;
        }

        // 防止缩放区域为0导致投射物不可见
        float area = weapon.GetArea();
        if (area <= 0) area = 1;
        transform.localScale = new Vector3(
            area * Mathf.Sign(transform.localScale.x),
            area * Mathf.Sign(transform.localScale.y), 1
        );

        // 设置当前投射物的穿透次数
        piercing = stats.piercing;

        // 如果设置了生命周期，则在生命周期结束后销毁投射物
        if (stats.lifespan > 0) Destroy(gameObject, stats.lifespan);

        // 如果启用了自动瞄准，则获取一个目标方向
        if (hasAutoAim) AcquireAutoAimFacing();
    }

    /// <summary>
    /// 获取自动瞄准的目标方向并调整投射物朝向。
    /// </summary>
    public virtual void AcquireAutoAimFacing()
    {
        float aimAngle; // 确定瞄准角度

        // 查找场景中所有的敌人
        EnemyStats[] targets =  FindObjectsByType<EnemyStats>(FindObjectsSortMode.None);

        // 如果存在敌人，则随机选择一个作为目标；否则随机选择一个角度
        if (targets.Length > 0)
        {
            EnemyStats selectedTarget = targets[Random.Range(0, targets.Length)];
            Vector2 difference = selectedTarget.transform.position - transform.position;
            aimAngle = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
        }
        else
        {
            aimAngle = Random.Range(0f, 360f);
        }

        // 将投射物朝向目标方向
        transform.rotation = Quaternion.Euler(0, 0, aimAngle);
    }

    /// <summary>
    /// 固定更新函数，在物理计算时调用。
    /// </summary>
    protected virtual void FixedUpdate()
    {
        // 只有当刚体类型为运动学时才手动控制移动
        if (rb.bodyType == RigidbodyType2D.Kinematic)
        {
            Weapon.Stats stats = weapon.GetStats();
            transform.position += transform.right * stats.speed * weapon.Owner.Stats.speed * Time.fixedDeltaTime;
            rb.MovePosition(transform.position);
            transform.Rotate(rotationSpeed * Time.fixedDeltaTime);
        }
    }

    /// <summary>
    /// 当触发器与其他碰撞体发生接触时调用。
    /// </summary>
    /// <param name="other">与之发生碰撞的另一个碰撞体。</param>
    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        EnemyStats es = other.GetComponent<EnemyStats>();
        BreakableProps p = other.GetComponent<BreakableProps>();

        // 只对敌人或可破坏物体产生碰撞效果
        if (es)
        {
            // 根据伤害来源决定击退方向的起点位置
            Vector3 source = damageSource == DamageSource.owner && owner ? owner.transform.position : transform.position;

            // 对敌人造成伤害
            es.TakeDamage(GetDamage(), source);

            Weapon.Stats stats = weapon.GetStats();
            
            weapon.ApplyBuffs(es);
            
            piercing--;
            if (stats.hitEffect)
            {
                Destroy(Instantiate(stats.hitEffect, transform.position, Quaternion.identity), 5f);
            }
        }
        else if (p)
        {
            p.TakeDamage(GetDamage());
            piercing--;

            Weapon.Stats stats = weapon.GetStats();
            if (stats.hitEffect)
            {
                Destroy(Instantiate(stats.hitEffect, transform.position, Quaternion.identity), 5f);
            }
        }

        // 如果穿透次数耗尽，则销毁投射物
        if (piercing <= 0) Destroy(gameObject);
    }
}
