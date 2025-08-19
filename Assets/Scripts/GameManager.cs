using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 游戏管理器类，用于控制游戏状态、暂停、结束以及UI显示等功能。
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    
    /// <summary>
    /// 游戏状态枚举，包含游戏进行中、暂停和游戏结束三种状态。
    /// </summary>
    public enum GameState
    {
        Gameplay,
        Paused,
        GameOver,
        LevelUp,
    }

    public GameState currentState;

    public GameState previousState;

    [Header("Damage Text Settings")] 
    public Canvas damageTextCanvas;
    public float textFontSize = 20;
    public TMP_FontAsset textFont;
    public Camera referenceCamera;
    
    [Header("Screens")] 
    public GameObject pauseScreen;
    public GameObject resultsScreen;
    public GameObject levelUpScreen;
    
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
    public TextMeshProUGUI timeSurvivedDisplay;
    public List<Image> chosenWeaponsUI = new List<Image>(6);
    public List<Image> chosenPassiveItemsUI = new List<Image>(6);

    [Header("Stopwatch")] 
    public float timeLimit;
    private float stopwatchTime;
    public TextMeshProUGUI stopwatchDisplay;
    
    public bool isGameOver = false;

    public bool choosingUpgrade;

    public GameObject playerObject;

    /// <summary>
    /// 初始化单例实例，并禁用所有屏幕UI。
    /// </summary>
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.LogWarning("EXTRA " + this + " DELETED");
            Destroy(gameObject);
        } 
        
        stopwatchTime = timeLimit;
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
                UpdateStopwatch();
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
            case GameState.LevelUp:
                if (!choosingUpgrade)
                {
                    choosingUpgrade = true;
                    Time.timeScale = 0f;
                    levelUpScreen.SetActive(true);
                    Debug.Log("LEVEL UP");
                }
                break;
            default:
                Debug.LogWarning("STATE DOES NOT EXIST");
                break;
        }
    }

    /// <summary>
    /// 协程函数，生成浮动文字效果并在指定时间后销毁。
    /// </summary>
    /// <param name="text">要显示的文本内容</param>
    /// <param name="target">目标位置变换组件</param>
    /// <param name="duration">持续时间，默认为1秒</param>
    /// <param name="speed">上升速度，默认为50单位/秒</param>
    /// <returns>IEnumerator接口，用于协程控制</returns>
    IEnumerator GenerateFloatingTextCoroutine(string text, Transform target, float duration = 1f, float speed = 50f)
    {
        GameObject textObj = new GameObject("Damage Floating Text");
        RectTransform rect = textObj.AddComponent<RectTransform>();
        TextMeshProUGUI tmPro = textObj.AddComponent<TextMeshProUGUI>();
        tmPro.text = text;
        tmPro.horizontalAlignment = HorizontalAlignmentOptions.Center;
        tmPro.verticalAlignment = VerticalAlignmentOptions.Middle;
        tmPro.fontSize = textFontSize;
        tmPro.fontStyle = FontStyles.Bold;
        
        if (textFont) tmPro.font = textFont;
        rect.position = referenceCamera.WorldToScreenPoint(target.position);
        
        Destroy(textObj, duration);
        
        textObj.transform.SetParent(instance.damageTextCanvas.transform);
        textObj.transform.SetSiblingIndex(0);

        WaitForEndOfFrame w = new WaitForEndOfFrame();
        float t = 0;
        float yOffset = 0;
        Vector3 lastKnownPosition = target.position;
        while (t < duration)
        { 
            if (!rect) break;
            
            tmPro.color = new Color(tmPro.color.r, tmPro.color.g, tmPro.color.b, 1 - t / duration);

            if (target)
                lastKnownPosition = target.position;
            
            yOffset += speed * Time.deltaTime;
            rect.position = referenceCamera.WorldToScreenPoint(lastKnownPosition + new Vector3(0, yOffset));
            
            yield return w;
            t += Time.deltaTime;
        }

    }
    
    /// <summary>
    /// 静态方法，用于外部调用生成浮动文字效果。
    /// </summary>
    /// <param name="text">要显示的文字内容</param>
    /// <param name="target">目标位置变换组件</param>
    /// <param name="duration">持续时间，默认为1秒</param>
    /// <param name="speed">上升速度，默认为1单位/秒</param>
    public static void GenerateFloatingText(string text, Transform target, float duration = 1f, float speed = 1f)
    {
        if (!instance.damageTextCanvas) return;
        
        if (!instance.referenceCamera) instance.referenceCamera = Camera.main;

        instance.StartCoroutine(instance.GenerateFloatingTextCoroutine(text, target, duration, speed));
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
        levelUpScreen.SetActive(false);
    }

    /// <summary>
    /// 设置游戏状态为GameOver。
    /// </summary>
    public void GameOver()
    {
        float timeSurvived = timeLimit - stopwatchTime;
        int survivedMinutes = Mathf.FloorToInt(timeSurvived / 60);
        int survivedSeconds = Mathf.FloorToInt(timeSurvived % 60);
        timeSurvivedDisplay.text = string.Format("{0:00}:{1:00}", survivedMinutes, survivedSeconds);
    
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
    public void AssignChosenCharacterUI(CharacterData choseCharacterData)
    {
        chosenCharacterImage.sprite = choseCharacterData.Icon;
        chosenCharacterName.text = choseCharacterData.Name;
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
    public void AssignChosenWeaponsAndPassiveItemsUI(List<PlayerInventory.Slot> chosenWeaponsData, List<PlayerInventory.Slot> chosenPassiveItemsData)
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
            if (chosenWeaponsData[i].image.sprite)
            {
                chosenWeaponsUI[i].enabled = true;
                chosenWeaponsUI[i].sprite = chosenWeaponsData[i].image.sprite;
            }
            else
            {
                chosenWeaponsUI[i].enabled = false;
            }
        }

        // 更新被动道具UI显示
        for (int i = 0; i < chosenPassiveItemsUI.Count; i++)
        {
            if (chosenPassiveItemsData[i].image.sprite)
            {
                chosenPassiveItemsUI[i].enabled = true;
                chosenPassiveItemsUI[i].sprite = chosenPassiveItemsData[i].image.sprite;
            }
            else
            {
                chosenPassiveItemsUI[i].enabled = false;
            }
        }
    }

    /// <summary>
    /// 更新倒计时计时器，当时间归零时触发游戏结束。
    /// </summary>
    private void UpdateStopwatch()
    {
        stopwatchTime -= Time.deltaTime;

        UpdateStopwatchDisplay();
    
        if (stopwatchTime <= 0)
        {
            stopwatchTime = 0;
            playerObject.SendMessage("Kill");
        }
    }

    /// <summary>
    /// 更新倒计时UI显示。
    /// </summary>
    private void UpdateStopwatchDisplay()
    {
        int minutes = Mathf.FloorToInt(stopwatchTime / 60);
        int seconds = Mathf.FloorToInt(stopwatchTime % 60);
    
        stopwatchDisplay.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    /// <summary>
    /// 开始升级流程，切换到升级状态并通知玩家对象处理升级逻辑。
    /// </summary>
    public void StartLevelUp()
    {
        ChangeState(GameState.LevelUp);
        playerObject.SendMessage("RemoveAndApplyUpgrades");
    }

    /// <summary>
    /// 结束升级流程，恢复正常游戏状态。
    /// </summary>
    public void EndLevelUp()
    {
        choosingUpgrade = false;
        Time.timeScale = 1f;
        levelUpScreen.SetActive(false);
        ChangeState(GameState.Gameplay);
    }
}
