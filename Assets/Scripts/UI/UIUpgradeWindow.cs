// 我们需要在附加此组件的GameObject上使用VerticalLayoutGroup，
// 因为它使用该组件来确保按钮均匀分布。

using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UIUpgradeWindow 是一个用于显示升级选项窗口的 MonoBehaviour 组件。
/// 该组件依赖 VerticalLayoutGroup 来管理布局，并支持动态生成和显示多个升级选项按钮。
/// </summary>
[RequireComponent(typeof(VerticalLayoutGroup))]
public class UIUpgradeWindow : MonoBehaviour
{
    // 我们需要访问布局上的填充/间距属性。
    VerticalLayoutGroup verticalLayout;

    // 我们需要分配的按钮和工具提示GameObjects。
    public RectTransform upgradeOptionTemplate;
    public TextMeshProUGUI tooltipTemplate;

    [Header("Settings")] 
    public int maxOptions = 4; // 我们不能显示超过这个数量的选项。
    public string newText = "New!"; // 显示新升级时的文字。

    // “New!”文本和常规文本的颜色。
    public Color newTextColor = Color.yellow, levelTextColor = Color.white;

    // 这些是<upgradeOptionTemplate>中不同UI元素的路径。
    [Header("Paths")] 
    public string iconPath = "Icon/Item Icon";
    public string namePath = "Name";
    public string descriptionPath = "Description";
    public string buttonPath = "Button";
    public string levelPath = "Level";

    // 这些是私有变量，用于函数跟踪UIUpgradeWindow中不同事物的状态。
    RectTransform rectTransform; // 该元素的RectTransform，便于引用。
    float optionHeight; // 升级选项模板的默认高度。
    int activeOptions; // 跟踪当前激活的选项数量。

    // 这是窗口上所有升级按钮的列表。
    List<RectTransform> upgradeOptions = new List<RectTransform>();

    // 用于跟踪上一帧的屏幕宽度/高度。
    // 以检测屏幕尺寸变化，以便我们知道何时需要重新计算大小。
    Vector2 lastScreen;

    /// <summary>
    /// 设置并显示升级选项窗口。
    /// 根据传入的物品数据列表随机选取指定数量的升级项进行展示，并为每个选项设置对应的UI内容与交互逻辑。
    /// </summary>
    /// <param name="inventory">玩家库存对象，用于获取或升级物品。</param>
    /// <param name="possibleUpgrades">可供选择的所有升级项的数据列表。</param>
    /// <param name="pick">要显示的升级选项数量，默认为3个。</param>
    /// <param name="tooltip">可选的底部提示文本，如果提供则会显示在窗口底部。</param>
    public void SetUpgrades(PlayerInventory inventory, List<ItemData> possibleUpgrades, int pick = 3,
        string tooltip = "")
    {
        pick = Mathf.Min(maxOptions, pick);

        // 如果我们没有足够的升级选项框，则创建它们。
        if (maxOptions > upgradeOptions.Count)
        {
            for (int i = upgradeOptions.Count; i < pick; i++)
            {
                GameObject go = Instantiate(upgradeOptionTemplate.gameObject, transform);
                upgradeOptions.Add((RectTransform)go.transform);
            }
        }

        // 如果提供了字符串，则启用工具提示。
        tooltipTemplate.text = tooltip;
        tooltipTemplate.gameObject.SetActive(tooltip.Trim() != "");

        // 只激活我们需要的升级选项数量，并为按钮和不同属性（如描述等）设置准备状态。
        activeOptions = 0;
        int totalPossibleUpgrades = possibleUpgrades.Count; // 我们需要从多少个升级中选择？
        foreach (RectTransform r in upgradeOptions)
        {
            if (activeOptions < pick && activeOptions < totalPossibleUpgrades)
            {
                r.gameObject.SetActive(true);

                // 从可能的升级中选择一个，然后将其从列表中移除。
                ItemData selected = possibleUpgrades[Random.Range(0, possibleUpgrades.Count)];
                possibleUpgrades.Remove(selected);
                Item item = inventory.Get(selected);

                // 插入物品名称。
                TextMeshProUGUI name = r.Find(namePath).GetComponent<TextMeshProUGUI>();
                if (name)
                {
                    name.text = selected.name;
                }

                // 插入物品当前等级，如果是新武器则插入"New!"文本。
                TextMeshProUGUI level = r.Find(levelPath).GetComponent<TextMeshProUGUI>();
                if (level)
                {
                    if (item)
                    {
                        if (item.currentLevel >= item.maxLevel)
                        {
                            level.text = "Max!";
                            level.color = newTextColor;
                        }
                        else
                        {
                            level.text = selected.GetLevelData(item.currentLevel + 1).name;
                            level.color = levelTextColor;
                        }
                    }
                    else
                    {
                        level.text = newText;
                        level.color = newTextColor;
                    }
                }

                // 插入物品的描述。
                TextMeshProUGUI desc = r.Find(descriptionPath).GetComponent<TextMeshProUGUI>();
                if (desc)
                {
                    if (item)
                    {
                        desc.text = selected.GetLevelData(item.currentLevel + 1).description;
                    }
                    else
                    {
                        desc.text = selected.GetLevelData(1).description;
                    }
                }

                // 插入物品的图标。
                Image icon = r.Find(iconPath).GetComponent<Image>();
                if (icon)
                {
                    icon.sprite = selected.icon;
                }

                // 插入按钮动作绑定。
                Button b = r.Find(buttonPath).GetComponent<Button>();
                if (b)
                {
                    b.onClick.RemoveAllListeners();
                    if (item)
                        b.onClick.AddListener(() => inventory.LevelUp(item));
                    else
                        b.onClick.AddListener(() => inventory.Add(selected));
                }

                activeOptions++;
            }
            else r.gameObject.SetActive(false);
        }

        // 调整所有元素的大小，使其不超过盒子的大小。
        RecalculateLayout();
    }

    /// <summary>
    /// 重新计算所有元素的高度。
    /// 当窗口大小发生变化时调用此方法。
    /// 我们手动执行此操作是因为VerticalLayoutGroup并不总是均匀地间隔所有元素。
    /// </summary>
    void RecalculateLayout()
    {
        // 计算所有选项的总可用高度，然后除以选项的数量。
        optionHeight = (rectTransform.rect.height - verticalLayout.padding.top - verticalLayout.padding.bottom -
                        (maxOptions - 1) * verticalLayout.spacing);
        if (activeOptions == maxOptions && tooltipTemplate.gameObject.activeSelf)
            optionHeight /= maxOptions + 1;
        else
            optionHeight /= maxOptions;

        // 如果工具提示当前处于活动状态，则重新计算其高度。
        if (tooltipTemplate.gameObject.activeSelf)
        {
            RectTransform tooltipRect = (RectTransform)tooltipTemplate.transform;
            tooltipTemplate.gameObject.SetActive(true);
            tooltipRect.sizeDelta = new Vector2(tooltipRect.sizeDelta.x, optionHeight);
            tooltipTemplate.transform.SetAsLastSibling();
        }

        // 设置每个活动升级选项按钮的高度。
        foreach (RectTransform r in upgradeOptions)
        {
            if (!r.gameObject.activeSelf) continue;
            r.sizeDelta = new Vector2(r.sizeDelta.x, optionHeight);
        }
    }
    
    /// <summary>
    /// 每帧检查屏幕尺寸是否发生变化，若发生变化则重新计算布局。
    /// </summary>
    void Update()
    {
        // 如果屏幕尺寸发生变化，则重新绘制此元素中的框。
        if(lastScreen.x != Screen.width || lastScreen.y != Screen.height)
        {
            RecalculateLayout();
            lastScreen = new Vector2(Screen.width, Screen.height);
        }
    }

    /// <summary>
    /// 在脚本实例被加载时调用，用于初始化关键组件和变量。
    /// </summary>
    void Awake()
    {
        // 初始化所有重要的变量。
        verticalLayout = GetComponentInChildren<VerticalLayoutGroup>();
        if (tooltipTemplate) tooltipTemplate.gameObject.SetActive(false);
        if (upgradeOptionTemplate) upgradeOptions.Add(upgradeOptionTemplate);

        // 获取此对象的RectTransform以进行高度计算。
        rectTransform = (RectTransform)transform;
    }
    
    /// <summary>
    /// 编辑器中的重置回调函数，用于自动查找并赋值模板对象。
    /// 自动搜索名为“Upgrade Option”的GameObject并将其分配为upgradeOptionTemplate，
    /// 然后搜索名为“Tooltip”的GameObject并将其分配为tooltipTemplate。
    /// </summary>
    void Reset()
    {
        upgradeOptionTemplate = (RectTransform)transform.Find("Upgrade Option");
        tooltipTemplate = transform.Find("Tooltip").GetComponentInChildren<TextMeshProUGUI>();
    }
}
