using UnityEngine;

/// <summary>
/// 这是一个可以被其他类继承的类，使该类的精灵自动按Y轴排序。
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public abstract class Sortable : MonoBehaviour
{

    SpriteRenderer sorted;
    public bool sortingActive = true; // 允许我们在某些对象上禁用此功能。
    public float minimumDistance = 0.2f; // 排序值更新前的最小距离。
    int lastSortOrder = 0;

    /// <summary>
    /// 在第一个帧更新之前调用Start方法
    /// 初始化SpriteRenderer组件引用
    /// </summary>
    protected virtual void Start()
    {
        sorted = GetComponent<SpriteRenderer>();
    }

    /// <summary>
    /// 每帧调用一次Update方法
    /// 根据对象的Y轴位置计算并更新精灵的排序顺序
    /// 只有当排序值发生变化时才更新，以提高性能
    /// </summary>
    protected virtual void LateUpdate()
    {
        // 根据Y轴位置计算新的排序值，距离越小变化越不频繁
        int newSortOrder = (int)(-transform.position.y / minimumDistance);
        // 只有当排序值发生变化时才更新SpriteRenderer的sortingOrder属性
        if (lastSortOrder != newSortOrder) sorted.sortingOrder = newSortOrder;
    }
}
