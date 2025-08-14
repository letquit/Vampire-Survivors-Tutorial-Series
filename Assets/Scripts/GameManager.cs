using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 游戏管理器类，用于控制游戏状态、暂停、结束以及UI显示等功能。
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    
    /// <summary>
    /// 游戏状态枚举，包含游戏进行中、暂停和游戏结束三种状态。
    /// </summary>
    public enum GameState
    {
        Gameplay,
        Paused,
        GameOver
    }

    public GameState currentState;

    public GameState previousState;

    [Header("Screens")] 
    public GameObject pauseScreen;
    public GameObject resultsScreen;
    
    [Header("Current Stat Displays")] 
    public TextMeshProUGUI currentHealthDisplay;
    public TextMeshProUGUI currentRecoveryDisplay;
    public TextMeshProUGUI currentMoveSpeedDisplay;
    public TextMeshProUGUI currentMightDisplay;
    public TextMeshProUGUI currentProjectileSpeedDisplay;
    public TextMeshProUGUI currentMagnetDisplay;

    [Header("Results Screen Displays")] 
    public Image chosenCharacterImage;
    public TextMeshProUGUI chosenCharacterName;
    public TextMeshProUGUI levelReachedDisplay;
    public List<Image> chosenWeaponsUI = new List<Image>(6);
    public List<Image> chosenPassiveItemsUI = new List<Image>(6);

    public bool isGameOver = false;

    /// <summary>
    /// 初始化单例实例，并禁用所有屏幕UI。
    /// </summary>
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogWarning("EXTRA " + this + " DELETED");
            Destroy(gameObject);
        } 
        
        DisableScreens();
    }

    /// <summary>
    /// 每帧检查当前游戏状态并执行对应逻辑。
    /// </summary>
    private void Update()
    {
        switch (currentState)
        {
            case GameState.Gameplay:
                CheckForPauseAndResume();
                break;
            case GameState.Paused:
                CheckForPauseAndResume();
                break;
            case GameState.GameOver:
                if (!isGameOver)
                {
                    isGameOver = true;
                    Time.timeScale = 0f;
                    Debug.Log("GAME IS OVER");
                    DisplayResults();
                }
                break;
            default:
                Debug.LogWarning("STATE DOES NOT EXIST");
                break;
        }
    }

    /// <summary>
    /// 更改当前游戏状态。
    /// </summary>
    /// <param name="newState">新的游戏状态</param>
    public void ChangeState(GameState newState)
    {
        currentState = newState;
    }

    /// <summary>
    /// 暂停游戏，保存当前状态并冻结时间。
    /// </summary>
    public void PauseGame()
    {
        if (currentState != GameState.Paused)
        {
            previousState = currentState;
            ChangeState(GameState.Paused);
            Time.timeScale = 0f;
            pauseScreen.SetActive(true);
            Debug.Log("Game is paused");
        }
    }
    
    /// <summary>
    /// 恢复游戏，恢复之前的状态并恢复正常时间流速。
    /// </summary>
    public void ResumeGame()
    {
        if (currentState == GameState.Paused)
        {
            
            ChangeState(previousState);
            Time.timeScale = 1f;
            pauseScreen.SetActive(false);
            Debug.Log("Game is resumed");
        }
    }

    /// <summary>
    /// 检查用户是否按下ESC键以切换暂停/恢复状态。
    /// </summary>
    private void CheckForPauseAndResume()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (currentState == GameState.Paused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    /// <summary>
    /// 禁用所有屏幕UI（暂停界面和结果界面）。
    /// </summary>
    private void DisableScreens()
    {
        pauseScreen.SetActive(false);
        resultsScreen.SetActive(false);
    }

    /// <summary>
    /// 设置游戏状态为GameOver。
    /// </summary>
    public void GameOver()
    {
        ChangeState(GameState.GameOver);
    }

    /// <summary>
    /// 显示游戏结果界面。
    /// </summary>
    private void DisplayResults()
    {
        resultsScreen.SetActive(true);
    }

    /// <summary>
    /// 将选中的角色信息显示在结果界面上。
    /// </summary>
    /// <param name="choseCharacterData">角色数据对象</param>
    public void AssignChosenCharacterUI(CharacterScriptableObject choseCharacterData)
    {
        chosenCharacterImage.sprite = choseCharacterData.icon;
        chosenCharacterName.text = choseCharacterData.name;
    }

    /// <summary>
    /// 显示玩家达到的等级。
    /// </summary>
    /// <param name="levelReachedData">达到的等级数值</param>
    public void AssignLevelReachedUI(int levelReachedData)
    {
        levelReachedDisplay.text = levelReachedData.ToString();
    }

    /// <summary>
    /// 根据传入的武器与被动道具图像列表更新结果界面中的UI显示。
    /// </summary>
    /// <param name="chosenWeaponsData">已选择的武器图像列表</param>
    /// <param name="chosenPassiveItemsData">已选择的被动道具图像列表</param>
    public void AssignChosenWeaponsAndPassiveItemsUI(List<Image> chosenWeaponsData, List<Image> chosenPassiveItemsData)
    {
        // 检查传入的数据长度是否匹配UI容器数量
        if (chosenWeaponsData.Count != chosenWeaponsUI.Count || chosenPassiveItemsData.Count != chosenPassiveItemsUI.Count)
        {
            Debug.Log("Chosen weapons and passive items data lists have different lengths");
            return;
        }

        // 更新武器UI显示
        for (int i = 0; i < chosenWeaponsUI.Count; i++)
        {
            if (chosenWeaponsData[i].sprite)
            {
                chosenWeaponsUI[i].enabled = true;
                chosenWeaponsUI[i].sprite = chosenWeaponsData[i].sprite;
            }
            else
            {
                chosenWeaponsUI[i].enabled = false;
            }
        }

        // 更新被动道具UI显示
        for (int i = 0; i < chosenPassiveItemsUI.Count; i++)
        {
            if (chosenPassiveItemsData[i].sprite)
            {
                chosenPassiveItemsUI[i].enabled = true;
                chosenPassiveItemsUI[i].sprite = chosenPassiveItemsData[i].sprite;
            }
            else
            {
                chosenPassiveItemsUI[i].enabled = false;
            }
        }
    }
}
