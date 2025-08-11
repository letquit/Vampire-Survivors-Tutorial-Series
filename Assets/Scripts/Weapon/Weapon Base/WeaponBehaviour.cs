using UnityEngine;

/// <summary>
/// 武器行为基类，包含所有武器的共同属性和方法
/// </summary>
public abstract class WeaponBehaviour : MonoBehaviour
{
    public WeaponScriptableObject weaponData;
    public float destroyAfterSeconds;

    protected float currentDamage;
    protected float currentSpeed;
    protected float currentCooldownDuration;
    protected int currentPierce;

    protected virtual void Awake()
    {
        currentDamage = weaponData.Damage;
        currentSpeed = weaponData.Speed;
        currentCooldownDuration = weaponData.CooldownDuration;
        currentPierce = weaponData.Pierce;
    }

    protected virtual void Start()
    {
        Destroy(gameObject, destroyAfterSeconds);
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            EnemyStats enemy = other.GetComponent<EnemyStats>();
            OnEnemyHit(enemy);
        }
    }

    /// <summary>
    /// 当武器击中敌人时调用的抽象方法，由子类实现具体逻辑
    /// </summary>
    /// <param name="enemy">被击中的敌人</param>
    protected abstract void OnEnemyHit(EnemyStats enemy);
}
