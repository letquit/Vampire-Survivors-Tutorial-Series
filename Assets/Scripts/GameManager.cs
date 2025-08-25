using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

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
        TreasureChest
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
    private int stackedLevelUps = 0;

    [Header("Results Screen Displays")] 
    public Image chosenCharacterImage;
    public TextMeshProUGUI chosenCharacterName;
    public TextMeshProUGUI levelReachedDisplay;
    public TextMeshProUGUI timeSurvivedDisplay;

    private const float DEFAULT_TIME_LIMIT = 1800f;
    private const float DEFAULT_CLOCK_SPEED = 1f;
    private float ClockSpeed => UILevelSelector.currentLevel?.clockSpeed ?? DEFAULT_CLOCK_SPEED;
    private float TimeLimit => UILevelSelector.currentLevel?.timeLimit ?? DEFAULT_TIME_LIMIT;

    
    [Header("Stopwatch")] 
    public float timeLimit;
    private float stopwatchTime;
    public TextMeshProUGUI stopwatchDisplay;
    
    private bool levelEnded = false;
    public GameObject reaperPrefab;

    private PlayerStats[] players;
    
    /// <summary>
    /// 获取游戏是否已结束。
    /// </summary>
    public bool isGameOver { get { return currentState == GameState.GameOver; } }

    /// <summary>
    /// 获取当前是否处于升级选择状态。
    /// </summary>
    public bool choosingUpgrade { get { return currentState == GameState.LevelUp; } }
    
    /// <summary>
    /// 获取已经经过的时间（秒）。
    /// </summary>
    /// <returns>从游戏开始到现在的总时间（秒）。</returns>
    public float GetElapsedTime() { return stopwatchTime; }

    /// <summary>
    /// 累加所有玩家的诅咒属性并返回该值。
    /// </summary>
    /// <returns>所有玩家诅咒值之和加1的结果。</returns>
    public static float GetCumulativeCurse()
    {
        if (!instance) return 1;

        float totalCurse = 0;
        foreach (PlayerStats p in instance.players)
        {
            totalCurse += p.Actual.curse;
        }
        return Mathf.Max(1, totalCurse); // 修改为使用Mathf.Max(1, totalCurse)
    }
    
    /// <summary>
    /// 累加所有玩家的等级并返回该值。
    /// </summary>
    /// <returns>所有玩家等级之和。</returns>
    public static int GetCumulativeLevels()
    {
        if (!instance) return 1;

        int totalLevel = 0;
        foreach (PlayerStats p in instance.players)
        {
            totalLevel += p.level;
        }
        return Mathf.Max(1, totalLevel); // 修改为使用Mathf.Max(1, totalLevel)
    }
    
    /// <summary>
    /// 初始化单例实例，并禁用所有屏幕UI。
    /// </summary>
    private void Awake()
    {
        players = FindObjectsByType<PlayerStats>(FindObjectsSortMode.None);
        
        timeLimit = TimeLimit;
        
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.LogWarning("EXTRA " + this + " DELETED");
            Destroy(gameObject);
        } 
        
        stopwatchTime = timeLimit; // 保留倒计时初始化
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
                // 添加Time.timeScale = 1; 确保游戏正常运行
                Time.timeScale = 1;
                CheckForPauseAndResume();
                UpdateStopwatch();
                break;
            case GameState.Paused:
                CheckForPauseAndResume();
                break;
            case GameState.GameOver:
            case GameState.TreasureChest:
                // 添加Time.timeScale = 0; 确保游戏完全停止
                Time.timeScale = 0;
                break;
            case GameState.LevelUp:
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
        previousState = currentState;
        currentState = newState;
    }

    /// <summary>
    /// 暂停游戏，保存当前状态并冻结时间。
    /// </summary>
    public void PauseGame()
    {
        if (currentState != GameState.Paused)
        {
            ChangeState(GameState.Paused);
            Time.timeScale = 0f;
            pauseScreen.SetActive(true);
            
            SpawnManager.instance?.Pause();
            
            // 通知所有玩家进入暂停状态
            foreach (PlayerStats player in players)
            {
                player.OnGamePause();
            }
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
            
            SpawnManager.instance?.Resume();
            
            // 通知所有玩家恢复状态
            foreach (PlayerStats player in players)
            {
                player.OnGameResume();
            }
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
        // 保留第一个脚本的时间计算方式
        float timeSurvived = timeLimit - stopwatchTime;
        int survivedMinutes = Mathf.FloorToInt(timeSurvived / 60);
        int survivedSeconds = Mathf.FloorToInt(timeSurvived % 60);
        timeSurvivedDisplay.text = string.Format("{0:00}:{1:00}", survivedMinutes, survivedSeconds);
    
        ChangeState(GameState.GameOver);
        Time.timeScale = 0f;
        DisplayResults();
        
        // 将所有玩家的所有硬币保存到存档文件中。
        foreach (PlayerStats p in players)
        {
            p.GetComponentInChildren<PlayerCollector>().SaveCoinsToStash();
        }

        // 将所有玩家的硬币添加到他们的保存文件中，因为游戏已经结束。
        foreach(PlayerStats p in players)
        {
            if(p.TryGetComponent(out PlayerCollector c))
            {
                c.SaveCoinsToStash();
            }
        }
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
    /// 获取一个随机玩家的位置。
    /// </summary>
    /// <returns>随机玩家的二维坐标位置</returns>
    public Vector2 GetRandomPlayerLocation()
    {
        int chosenPlayer = Random.Range(0, players.Length);
        return new Vector2(players[chosenPlayer].transform.position.x, players[chosenPlayer].transform.position.y);
    }
    
    /// <summary>
    /// 更新倒计时计时器，当时间归零时触发游戏结束。
    /// </summary>
    private void UpdateStopwatch()
    {
        // 保留第一个脚本的倒计时逻辑
        stopwatchTime -= Time.deltaTime * ClockSpeed;

        UpdateStopwatchDisplay();

        // 当时间归零或以下时结束关卡
        if (stopwatchTime <= 0 && !levelEnded)
        {
            levelEnded = true;
            
            FindFirstObjectByType<SpawnManager>()?.gameObject.SetActive(false);
            foreach (EnemyStats e in FindObjectsByType<EnemyStats>(FindObjectsSortMode.None))
                e.SendMessage("Kill");

            // 生成死神
            Vector2 reaperOffset = Random.insideUnitCircle * 50f;
            Vector2 spawnPosition = GetRandomPlayerLocation() + reaperOffset;
            Instantiate(reaperPrefab, spawnPosition, Quaternion.identity);
        }
    }


    /// <summary>
    /// 更新倒计时UI显示。
    /// </summary>
    private void UpdateStopwatchDisplay()
    {
        // 确保显示的时间不会为负数
        float displayTime = Mathf.Max(0, stopwatchTime);
        
        int minutes = Mathf.FloorToInt(displayTime / 60);
        int seconds = Mathf.FloorToInt(displayTime % 60);

        stopwatchDisplay.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }


    /// <summary>
    /// 开始升级流程，切换到升级状态并通知玩家对象处理升级逻辑。
    /// </summary>
    public void StartLevelUp()
    {
        ChangeState(GameState.LevelUp);
        
        if (levelUpScreen.activeSelf) stackedLevelUps++;
        else
        {
            Time.timeScale = 0f;
            levelUpScreen.SetActive(true);
            
            // 通知所有玩家进入升级状态
            foreach (PlayerStats player in players)
            {
                player.OnLevelUpStart();
                player.SendMessage("RemoveAndApplyUpgrades");
            }
        }
    }

    /// <summary>
    /// 结束升级流程，恢复正常游戏状态。
    /// </summary>
    public void EndLevelUp()
    {
        Time.timeScale = 1f;
        levelUpScreen.SetActive(false);
        ChangeState(GameState.Gameplay);
        
        // 通知所有玩家结束升级状态
        foreach (PlayerStats player in players)
        {
            player.OnLevelUpEnd();
        }
        
        if (stackedLevelUps > 0)
        {
            stackedLevelUps--;
            StartLevelUp();
        }
    }
}
