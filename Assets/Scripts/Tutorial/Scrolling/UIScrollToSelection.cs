using System.Collections.Generic;
using UnityEngine.EventSystems;

namespace UnityEngine.UI.Extensions
{
    /// <summary>
    /// 自动滚动到当前选中UI元素的组件
    /// </summary>
    [RequireComponent(typeof(ScrollRect))]
    [AddComponentMenu("UI/Extensions/UIScrollToSelection")]
    public class UIScrollToSelection : MonoBehaviour
    {

        //*** ATTRIBUTES ***//
        [Header("[ Settings ]")]
        [SerializeField]
        private ScrollType scrollDirection = ScrollType.BOTH;
        [SerializeField]
        private float scrollSpeed = 10f;
        public float yPadding = 20f; 

        [Header("[ Input ]")]
        [SerializeField]
        private bool cancelScrollOnInput = false;
        [SerializeField]
        private List<KeyCode> cancelScrollKeycodes = new List<KeyCode>();

        //*** PROPERTIES ***//
        // REFERENCES
        /// <summary>
        /// 获取滚动矩形的内容区域
        /// </summary>
        protected RectTransform LayoutListGroup
        {
            get { return TargetScrollRect != null ? TargetScrollRect.content : null; }
        }

        // SETTINGS
        /// <summary>
        /// 获取滚动方向设置
        /// </summary>
        protected ScrollType ScrollDirection
        {
            get { return scrollDirection; }
        }
        
        /// <summary>
        /// 获取滚动速度设置
        /// </summary>
        protected float ScrollSpeed
        {
            get { return scrollSpeed; }
        }

        // INPUT
        /// <summary>
        /// 获取是否在输入时取消滚动的设置
        /// </summary>
        protected bool CancelScrollOnInput
        {
            get { return cancelScrollOnInput; }
        }
        
        /// <summary>
        /// 获取取消滚动的按键码列表
        /// </summary>
        protected List<KeyCode> CancelScrollKeycodes
        {
            get { return cancelScrollKeycodes; }
        }

        // CACHED REFERENCES
        /// <summary>
        /// 滚动窗口的RectTransform引用
        /// </summary>
        protected RectTransform ScrollWindow { get; set; }
        
        /// <summary>
        /// 目标ScrollRect组件引用
        /// </summary>
        protected ScrollRect TargetScrollRect { get; set; }

        // SCROLLING
        /// <summary>
        /// 获取当前事件系统
        /// </summary>
        protected EventSystem CurrentEventSystem
        {
            get { return EventSystem.current; }
        }
        
        /// <summary>
        /// 上一次检查的游戏对象
        /// </summary>
        protected GameObject LastCheckedGameObject { get; set; }
        
        /// <summary>
        /// 当前选中的游戏对象
        /// </summary>
        protected GameObject CurrentSelectedGameObject
        {
            get { return EventSystem.current.currentSelectedGameObject; }
        }
        
        /// <summary>
        /// 当前目标的RectTransform
        /// </summary>
        protected RectTransform CurrentTargetRectTransform { get; set; }
        
        /// <summary>
        /// 手动滚动是否可用
        /// </summary>
        protected bool IsManualScrollingAvailable { get; set; }

        //*** METHODS - PUBLIC ***//


        //*** METHODS - PROTECTED ***//
        /// <summary>
        /// 初始化组件引用
        /// </summary>
        protected virtual void Awake()
        {
            TargetScrollRect = GetComponent<ScrollRect>();
            ScrollWindow = TargetScrollRect.GetComponent<RectTransform>();
        }

        /// <summary>
        /// 启动时的初始化方法
        /// </summary>
        protected virtual void Start()
        {

        }

        /// <summary>
        /// 每帧更新方法，处理滚动逻辑
        /// </summary>
        protected virtual void Update()
        {
            UpdateReferences();
            CheckIfScrollingShouldBeLocked();
            ScrollRectToLevelSelection();
        }

        //*** METHODS - PRIVATE ***//
        /// <summary>
        /// 更新引用和检查游戏对象变化
        /// </summary>
        private void UpdateReferences()
        {
            // update current selected rect transform
            if (CurrentSelectedGameObject != LastCheckedGameObject)
            {
                CurrentTargetRectTransform = (CurrentSelectedGameObject != null) ?
                    CurrentSelectedGameObject.GetComponent<RectTransform>() :
                    null;

                // unlock automatic scrolling
                if (CurrentSelectedGameObject != null &&
                    CurrentSelectedGameObject.transform.parent == LayoutListGroup.transform)
                {
                    IsManualScrollingAvailable = false;
                }
            }

            LastCheckedGameObject = CurrentSelectedGameObject;
        }

        /// <summary>
        /// 检查是否应该锁定滚动
        /// </summary>
        private void CheckIfScrollingShouldBeLocked()
        {
            if (CancelScrollOnInput == false || IsManualScrollingAvailable == true)
            {
                return;
            }

            for (int i = 0; i < CancelScrollKeycodes.Count; i++)
            {
                if (Input.GetKeyDown(CancelScrollKeycodes[i]) == true)
                {
                    IsManualScrollingAvailable = true;

                    break;
                }
            }
        }

        /// <summary>
        /// 将滚动矩形滚动到选中元素位置
        /// </summary>
        private void ScrollRectToLevelSelection()
        {
            // check main references
            bool referencesAreIncorrect = (TargetScrollRect == null || LayoutListGroup == null || ScrollWindow == null);

            if (referencesAreIncorrect == true || IsManualScrollingAvailable == true)
            {
                return;
            }

            RectTransform selection = CurrentTargetRectTransform;

            // check if scrolling is possible
            if (selection == null || selection.transform.parent != LayoutListGroup.transform)
            {
                return;
            }

            // depending on selected scroll direction move the scroll rect to selection
            switch (ScrollDirection)
            {
                case ScrollType.VERTICAL:
                    UpdateVerticalScrollPosition(selection);
                    break;
                case ScrollType.HORIZONTAL:
                    UpdateHorizontalScrollPosition(selection);
                    break;
                case ScrollType.BOTH:
                    UpdateVerticalScrollPosition(selection);
                    UpdateHorizontalScrollPosition(selection);
                    break;
            }
        }

        /// <summary>
        /// 更新垂直滚动位置
        /// </summary>
        /// <param name="selection">选中的RectTransform</param>
        private void UpdateVerticalScrollPosition(RectTransform selection)
        {
            // move the current scroll rect to correct position
            float selectionPosition = -selection.anchoredPosition.y - (selection.rect.height * (1 - selection.pivot.y) - yPadding);

            float elementHeight = selection.rect.height;
            float maskHeight = ScrollWindow.rect.height;
            float listAnchorPosition = LayoutListGroup.anchoredPosition.y;

            // get the element offset value depending on the cursor move direction
            float offlimitsValue = GetScrollOffset(selectionPosition, listAnchorPosition, elementHeight, maskHeight);

            float normalizedPosition = TargetScrollRect.verticalNormalizedPosition +
                                       (offlimitsValue / LayoutListGroup.rect.height);

            if (normalizedPosition < 0)
            {
                normalizedPosition -= (Mathf.Abs(offlimitsValue) / LayoutListGroup.rect.height);
            }
            else if (normalizedPosition > 0)
            {
                normalizedPosition += (offlimitsValue / LayoutListGroup.rect.height);
            }
            
            normalizedPosition = Mathf.Clamp01(normalizedPosition);
            
            // move the target scroll rect
            TargetScrollRect.verticalNormalizedPosition = Mathf.SmoothStep(TargetScrollRect.verticalNormalizedPosition,
                normalizedPosition, Time.unscaledDeltaTime * scrollSpeed);
        }

        /// <summary>
        /// 更新水平滚动位置
        /// </summary>
        /// <param name="selection">选中的RectTransform</param>
        private void UpdateHorizontalScrollPosition(RectTransform selection)
        {
            // move the current scroll rect to correct position
            float selectionPosition = -selection.anchoredPosition.x - (selection.rect.width * (1 - selection.pivot.x));

            float elementWidth = selection.rect.width;
            float maskWidth = ScrollWindow.rect.width;
            float listAnchorPosition = -LayoutListGroup.anchoredPosition.x;

            // get the element offset value depending on the cursor move direction
            float offlimitsValue = -GetScrollOffset(selectionPosition, listAnchorPosition, elementWidth, maskWidth);

            // move the target scroll rect
            TargetScrollRect.horizontalNormalizedPosition +=
                (offlimitsValue / LayoutListGroup.rect.width) * Time.unscaledDeltaTime * scrollSpeed;
        }

        /// <summary>
        /// 计算滚动偏移值
        /// </summary>
        /// <param name="position">目标位置</param>
        /// <param name="listAnchorPosition">列表锚点位置</param>
        /// <param name="targetLength">目标长度</param>
        /// <param name="maskLength">遮罩长度</param>
        /// <returns>滚动偏移值</returns>
        private float GetScrollOffset(float position, float listAnchorPosition, float targetLength, float maskLength)
        {
            if (position < listAnchorPosition + (targetLength / 2))
            {
                return (listAnchorPosition + maskLength) - (position - targetLength);
            }
            else if (position + targetLength > listAnchorPosition + maskLength)
            {
                return (listAnchorPosition + maskLength) - (position + targetLength);
            }

            return 0;
        }

        //*** ENUMS ***//
        /// <summary>
        /// 滚动类型枚举
        /// </summary>
        public enum ScrollType
        {
            VERTICAL,
            HORIZONTAL,
            BOTH
        }
    }
}