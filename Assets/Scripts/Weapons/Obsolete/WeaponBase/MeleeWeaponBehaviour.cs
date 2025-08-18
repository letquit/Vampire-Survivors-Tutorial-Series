using System;
using UnityEngine;

/// <summary>
/// 近战武器行为类，用于控制近战武器的生命周期
/// </summary>
public class MeleeWeaponBehaviour : WeaponBehaviour
{
    protected override void OnEnemyHit(EnemyStats enemy)
    {
        enemy.TakeDamage(GetCurrentDamage(), transform.position);
    }
}
