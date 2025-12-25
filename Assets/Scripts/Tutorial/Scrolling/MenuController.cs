using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

/// <summary>
/// 菜单控制器，负责管理菜单的显示/隐藏、卡片生成和导航选择功能
/// </summary>
public class MenuController : MonoBehaviour
{
    public static MenuController Instance;

    public int numOfCardsToSpawn = 15;
    [SerializeField] private GameObject canvasObj;
    [SerializeField] private GameObject cardToSpawn;
    [SerializeField] private Transform cardParentTransform;
    [SerializeField] private GridLayoutGroup group;

    [HideInInspector] public List<GameObject> cards = new List<GameObject>();
    
    /// <summary>
    /// 获取菜单是否打开的状态
    /// </summary>
    public bool IsMenuOpen { get; private set; }

    private bool _cardsHaveSpawned;
    
    /// <summary>
    /// 获取或设置最后选择的卡片对象
    /// </summary>
    public GameObject LastCardSelected { get; set; }
    
    /// <summary>
    /// 获取或设置最后选择的卡片索引
    /// </summary>
    public int LastSelectedIndex { get; set; }

    /// <summary>
    /// 初始化单例实例并设置画布初始状态为隐藏
    /// </summary>
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        
        canvasObj.SetActive(false);
    }

    /// <summary>
    /// 更新方法，处理菜单开关和卡片导航选择逻辑
    /// </summary>
    private void Update()
    {
        if (Keyboard.current.mKey.wasPressedThisFrame || Gamepad.current.startButton.wasPressedThisFrame)
        {
            ToggleMenu();
        }

        if (EventSystem.current.currentSelectedGameObject == null && LastCardSelected != null)
        {
            if (UserInput.MoveInput.x > 0)
            {
                int add = CalculateXAddition(1);
                HandleNextCardSelection(add);
            }
            else if (UserInput.MoveInput.x < 0)
            {
                int add = CalculateXAddition(-1);
                HandleNextCardSelection(add);
            }
            else if (UserInput.MoveInput.y > 0)
            {
                int add = CalculateYAddition(1);
                HandleNextCardSelection(add);
            }
            else if (UserInput.MoveInput.y < 0)
            {
                int add = CalculateYAddition(-1);
                HandleNextCardSelection(add);
            }
        }
    }

    /// <summary>
    /// 处理下一个卡片的选择逻辑
    /// </summary>
    /// <param name="addition">要添加到当前索引的偏移量</param>
    private void HandleNextCardSelection(int addition)
    {
        int newIndex = LastSelectedIndex + addition;
        if (newIndex < 0)
        {
            EventSystem.current.SetSelectedGameObject(LastCardSelected);
        }
        else
        {
            EventSystem.current.SetSelectedGameObject(cards[newIndex]);
        }
    }

    /// <summary>
    /// 计算X轴方向的索引偏移量
    /// </summary>
    /// <param name="direction">移动方向，1为向右，-1为向左</param>
    /// <returns>计算出的索引偏移量</returns>
    private int CalculateXAddition(int direction)
    {
        Vector2Int count = GridLayoutGroupHelper.Size(group);
        if (direction > 0)
        {
            if (LastSelectedIndex % count.x == count.x - 1)
            {
                return 0;
            }
            else
            {
                return 1;
            }
        }

        if (direction < 0)
        {
            if (LastSelectedIndex % count.x == 0)
            {
                return 0;
            }
            else 
            {
                return -1;
            }
        }
        return 0;
    }

    /// <summary>
    /// 计算Y轴方向的索引偏移量
    /// </summary>
    /// <param name="direction">移动方向，1为向上，-1为向下</param>
    /// <returns>计算出的索引偏移量</returns>
    private int CalculateYAddition(int direction)
    {
        if (direction > 0)
        {
            Vector2Int count = GridLayoutGroupHelper.Size(group);
            if (LastSelectedIndex - count.x < 0)
            {
                return 0;
            }
            else
            {
                return -count.x;
            }
        }
        else if (direction < 0)
        {
            Vector2Int count = GridLayoutGroupHelper.Size(group);
            if (LastSelectedIndex + count.x > cards.Count)
            {
                return 0;
            }
            else
            {
                return count.x;
            }
        }
        
        return 0;
    }

    /// <summary>
    /// 切换菜单的显示/隐藏状态
    /// </summary>
    private void ToggleMenu()
    {
        if (IsMenuOpen)
        {
            canvasObj?.SetActive(false);
            IsMenuOpen = false;
        }
        else
        {
            canvasObj?.SetActive(true);
            IsMenuOpen = true;
            
            if (!_cardsHaveSpawned)
            {
                SpawnCards();
            }
        }
    }

    /// <summary>
    /// 生成指定数量的卡片并设置初始选择
    /// </summary>
    private void SpawnCards()
    {
        for (int i = 0; i < numOfCardsToSpawn; i++)
        {
            GameObject card = Instantiate(cardToSpawn, cardParentTransform);
            cards.Add(card);

            if (i == 0)
            {
                EventSystem.current.SetSelectedGameObject(card);
            }
        }
        
        _cardsHaveSpawned = true;
    }
}