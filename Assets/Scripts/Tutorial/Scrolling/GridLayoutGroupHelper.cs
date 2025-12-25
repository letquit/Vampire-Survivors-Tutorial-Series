using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// GridLayoutGroup辅助类，提供计算网格布局尺寸的扩展方法
/// </summary>
public static class GridLayoutGroupHelper
{
    /// <summary>
    /// 计算GridLayoutGroup的实际尺寸（行数和列数）
    /// </summary>
    /// <param name="grid">要计算尺寸的GridLayoutGroup组件</param>
    /// <returns>返回Vector2Int，x为列数，y为行数</returns>
    public static Vector2Int Size(this GridLayoutGroup grid)
    {
        int itemsCount = grid.transform.childCount;
        Vector2Int size = Vector2Int.zero;

        if (itemsCount == 0)
            return size;

        switch (grid.constraint)
        {
            case GridLayoutGroup.Constraint.FixedColumnCount:
                size.x = grid.constraintCount;
                size.y = getAnotherAxisCount(itemsCount, size.x);
                break;

            case GridLayoutGroup.Constraint.FixedRowCount:
                size.y = grid.constraintCount;
                size.x = getAnotherAxisCount(itemsCount, size.y);
                break;

            case GridLayoutGroup.Constraint.Flexible:
                size = flexibleSize(grid);
                break;

            default:
                throw new ArgumentOutOfRangeException($"Unexpected constraint: {grid.constraint}");
        }

        return size;
    }

    /// <summary>
    /// 计算Flexible约束下的网格尺寸
    /// 通过遍历子对象的锚点位置来确定网格的实际列数和行数
    /// </summary>
    /// <param name="grid">要计算尺寸的GridLayoutGroup组件</param>
    /// <returns>返回Vector2Int，x为列数，y为行数</returns>
    private static Vector2Int flexibleSize(this GridLayoutGroup grid)
    {
        int itemsCount = grid.transform.childCount;
        float prevX = float.NegativeInfinity;
        int xCount = 0;

        // 遍历子对象，通过X轴位置变化来确定列数
        for (int i = 0; i < itemsCount; i++)
        {
            Vector2 pos = ((RectTransform)grid.transform.GetChild(i)).anchoredPosition;

            if (pos.x <= prevX)
                break;

            prevX = pos.x;
            xCount++;
        }

        int yCount = getAnotherAxisCount(itemsCount, xCount);
        return new Vector2Int(xCount, yCount);
    }

    /// <summary>
    /// 根据总数和一个轴的数量计算另一个轴的数量
    /// </summary>
    /// <param name="totalCount">总项目数量</param>
    /// <param name="axisCount">已知轴的数量</param>
    /// <returns>另一个轴的数量，如果有余数则加1</returns>
    private static int getAnotherAxisCount(int totalCount, int axisCount)
    {
        return totalCount / axisCount + Mathf.Min(1, totalCount % axisCount);
    }
}