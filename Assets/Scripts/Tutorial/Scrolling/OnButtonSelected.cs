using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 处理按钮选中状态的组件，实现鼠标悬停、离开、选中、取消选中事件
/// </summary>
public class OnButtonSelected : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
{

    /// <summary>
    /// 当对象被选中时调用
    /// </summary>
    /// <param name="eventData">事件数据</param>
    public void OnSelect(BaseEventData eventData)
    {
        // 缩放动画：将对象放大到1.075倍，动画时长0.075秒，使用InOutQuad缓动
        transform.DOScale(1.075f, 0.075f).SetEase(Ease.InOutQuad);
        
        // 记录当前选中的卡片对象
        MenuController.Instance.LastCardSelected = gameObject;
        // 遍历卡片列表，找到当前卡片的索引并记录
        for (int i = 0; i < MenuController.Instance.cards.Count; i++)
        {
            if (MenuController.Instance.cards[i] == gameObject)
            {
                // 记录当前选中卡片的索引
                MenuController.Instance.LastSelectedIndex = i;
                return;
            }
        }
    }

    /// <summary>
    /// 当对象被取消选中时调用
    /// </summary>
    /// <param name="eventData">事件数据</param>
    public void OnDeselect(BaseEventData eventData)
    {
        // 缩放动画：将对象恢复到原始大小1倍，动画时长0.075秒
        transform.DOScale(1f, 0.075f);
    }
    
    /// <summary>
    /// 当鼠标指针进入对象时调用
    /// </summary>
    /// <param name="eventData">指针事件数据</param>
    public void OnPointerEnter(PointerEventData eventData)
    {
        // 设置当前对象为选中对象
        eventData.selectedObject = gameObject;
    }

    /// <summary>
    /// 当鼠标指针离开对象时调用
    /// </summary>
    /// <param name="eventData">指针事件数据</param>
    public void OnPointerExit(PointerEventData eventData)
    {
        // 清空选中对象
        eventData.selectedObject = null;
    }
}