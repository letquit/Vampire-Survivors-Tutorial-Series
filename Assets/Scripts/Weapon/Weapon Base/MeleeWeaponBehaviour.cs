using System;
using UnityEngine;

/// <summary>
/// 近战武器行为类，用于控制近战武器的生命周期
/// </summary>
public class MeleeWeaponBehaviour : MonoBehaviour
{
    public float destroyAfterSeconds;

    /// <summary>
    /// 初始化函数，在对象启动时调用
    /// 负责设置武器在指定时间后自动销毁
    /// </summary>
    protected virtual void Start()
    {
        // 在指定秒数后销毁当前游戏对象
        Destroy(gameObject, destroyAfterSeconds);
    }
}

