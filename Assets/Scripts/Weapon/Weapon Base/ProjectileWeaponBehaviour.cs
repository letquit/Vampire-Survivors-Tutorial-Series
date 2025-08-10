using System;
using UnityEngine;

/// <summary>
/// 抛射体武器行为类，用于控制抛射体的飞行方向和自动销毁
/// </summary>
public class ProjectileWeaponBehaviour : MonoBehaviour
{
    protected Vector3 direction;
    public float destroyAfterSeconds;

    /// <summary>
    /// 初始化函数，在对象启动时调用
    /// 设置抛射体在指定时间后自动销毁
    /// </summary>
    protected virtual void Start()
    {
        Destroy(gameObject, destroyAfterSeconds);
    }

    /// <summary>
    /// 检查并设置抛射体的飞行方向
    /// 根据方向向量计算旋转角度，使抛射体朝向正确的方向
    /// </summary>
    /// <param name="dir">抛射体的飞行方向向量</param>
    public void DirectionChecker(Vector3 dir)
    {
        direction = dir;

        float dirx = direction.x;
        float diry = direction.y;

        Vector3 scale = transform.localScale;

        // 计算方向向量与右方向的夹角，用于确定抛射体的旋转角度
        float zAngle = Vector3.Angle(Vector3.right, new Vector3(dirx, diry, 0));
        if (diry < 0)
        {
            zAngle = -zAngle;
        }
        

        transform.localScale = scale;
        transform.rotation = Quaternion.Euler(0, 0, zAngle-45f );    //Can't simply set the vector because cannot convert
    }
}

