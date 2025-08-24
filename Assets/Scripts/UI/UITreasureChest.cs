using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 宝箱UI控制器，用于处理宝箱开启时的视觉效果、物品展示和交互逻辑。
/// </summary>
public class UITreasureChest : MonoBehaviour
{
    public static UITreasureChest instance;
    PlayerCollector collector;
    TreasureChest _currentChest;
    TreasureChestDropProfile dropProfile;

    [Header("Visual Elements")]
    public GameObject openingVFX;
    public GameObject beamVFX;
    public GameObject fireworks;
    public GameObject doneButton;
    public GameObject curvedBeams;
    public List<ItemDisplays> items;
    Color originalColor = new Color32(0x42, 0x41, 0x87, 255);

    [Header("UI Elements")]
    public GameObject chestCover;
    public GameObject chestButton;

    [Header("UI Components")]
    public Image chestPanel;
    public TextMeshProUGUI coinText;
    private float coins;

    // 内部状态
    private List<Sprite> icons = new List<Sprite>();
    private bool isAnimating = false;
    private Coroutine chestSequenceCoroutine;

    // 音效
    private AudioSource audiosource;
    public AudioClip pickUpSound;

    [System.Serializable]
    public struct ItemDisplays
    {
        public GameObject beam;
        public Image spriteImage;
        public GameObject sprite;
        public GameObject weaponBeam;
    }
    
    private void Awake()
    {
        audiosource = GetComponent<AudioSource>();
        gameObject.SetActive(false);

        // 确保场景中只有一个实例存在
        if (instance != null && instance != this)
        {
            Debug.LogWarning("发现多个UI宝箱。多余的已删除。");
            Destroy(gameObject);
            return;
        }

        instance = this;
    }
    
    /// <summary>
    /// 激活宝箱UI并初始化相关参数。
    /// </summary>
    /// <param name="collector">玩家收集器组件，用于后续添加硬币。</param>
    /// <param name="levelChest">当前宝箱对象，用于获取掉落配置。</param>
    public static void Activate(PlayerCollector collector, TreasureChest levelChest)
    {
        if (!instance) Debug.LogWarning("未找到宝箱UI GameObject。");

        // 保存重要变量。
        instance.collector = collector;
        instance._currentChest = levelChest;
        instance.dropProfile = levelChest.GetCurrentDropProfile();
        Debug.Log(instance.dropProfile);

        // 激活GameObject。
        GameManager.instance.ChangeState(GameManager.GameState.TreasureChest);
        instance.gameObject.SetActive(true);
    }
    
    /// <summary>
    /// 通知UI接收到一个物品图标，并将其添加到图标列表中。
    /// </summary>
    /// <param name="icon">物品的图标精灵。</param>
    public static void NotifyItemReceived(Sprite icon)
    {
        // 如果无法使用图标更新此类，则包含警告消息通知用户问题所在。
        if (instance) instance.icons.Add(icon);
        else Debug.LogWarning("不存在UITreasureChest实例。无法更新宝箱UI。");
    }
    
    /// <summary>
    /// 使指定图像闪烁白色指定次数。
    /// </summary>
    /// <param name="image">要闪烁的图像组件。</param>
    /// <param name="times">闪烁次数。</param>
    /// <param name="flashDuration">每次闪烁的持续时间（秒）。</param>
    /// <returns>协程枚举器。</returns>
    private IEnumerator FlashWhite(Image image, int times, float flashDuration = 0.2f)
    {
        originalColor = image.color;

        // 让宝箱面板闪烁x次
        for (int i = 0; i < times; i++)
        {
            image.color = Color.white;
            yield return new WaitForSecondsRealtime(flashDuration);

            image.color = originalColor;
            yield return new WaitForSecondsRealtime(0.2f);
        }
    }

    /// <summary>
    /// 在指定延迟后激活弯曲光束。
    /// </summary>
    /// <param name="spawnTime">延迟时间（秒）。</param>
    /// <returns>协程枚举器。</returns>
    IEnumerator ActivateCurvedBeams(float spawnTime)
    {
        yield return new WaitForSecondsRealtime(spawnTime);
        curvedBeams.SetActive(true);
    }
    
    /// <summary>
    /// 处理硬币显示动画，包括滚动数字和完成按钮激活。
    /// </summary>
    /// <param name="maxCoins">最大硬币数量。</param>
    /// <returns>协程枚举器。</returns>
    IEnumerator HandleCoinDisplay(float maxCoins)
    {
        coinText.gameObject.SetActive(true);
        float elapsedTime = 0;
        coins = maxCoins;

        // 硬币滚动动画，当达到最大硬币数时停止
        while (elapsedTime < maxCoins)
        {
            elapsedTime += Time.unscaledDeltaTime * 20f;
            coinText.text = string.Format("{0:F2}", elapsedTime);
            yield return null;
        }

        // 只有在硬币达到最大值时才激活完成按钮
        yield return new WaitForSecondsRealtime(2f);
        doneButton.SetActive(true);
    }
    
    /// <summary>
    /// 设置指定索引的光束显示。
    /// </summary>
    /// <param name="index">物品索引。</param>
    private void SetupBeam(int index)
    {
        items[index].weaponBeam.SetActive(true);
        items[index].beam.SetActive(true);
        items[index].spriteImage.sprite = icons[index];
        items[index].beam.GetComponent<Image>().color = dropProfile.beamColors[index];
    }

    /// <summary>
    /// 延迟显示指定范围内的光束。
    /// </summary>
    /// <param name="startIndex">起始索引。</param>
    /// <param name="endIndex">结束索引。</param>
    /// <returns>协程枚举器。</returns>
    private IEnumerator ShowDelayedBeams(int startIndex, int endIndex)
    {
        yield return new WaitForSecondsRealtime(dropProfile.delayTime);

        for (int i = startIndex; i < endIndex; i++)
        {
            SetupBeam(i);
        }
    }
    
    /// <summary>
    /// 根据掉落配置显示指定数量的光束。
    /// </summary>
    /// <param name="noOfBeams">光束数量。</param>
    public void DisplayBeam(float noOfBeams)
    {
        int delayedStartIndex = Mathf.Max(0, (int)noOfBeams - dropProfile.delayedBeams); // 确保光束不会超出索引范围

        // 显示立即光束
        for (int i = 0; i < delayedStartIndex; i++)
        {
            SetupBeam(i);
        }

        // 延迟显示剩余光束
        if (dropProfile.delayedBeams > 0)
            StartCoroutine(ShowDelayedBeams(delayedStartIndex, (int)noOfBeams));

        StartCoroutine(DisplayItems(noOfBeams));
    }
    
    /// <summary>
    /// 显示物品图标，根据数量采用不同的显示策略。
    /// </summary>
    /// <param name="noOfBeams">物品数量。</param>
    /// <returns>协程枚举器。</returns>
    private IEnumerator DisplayItems(float noOfBeams)
    {
        yield return new WaitForSecondsRealtime(dropProfile.animDuration);

        if (noOfBeams == 5)
        {
            // 显示第一个物品
            items[0].weaponBeam.SetActive(false);
            items[0].sprite.SetActive(true);
            yield return new WaitForSecondsRealtime(0.3f);

            // 同时显示第二个和第三个物品
            for (int i = 1; i <= 2; i++)
            {
                items[i].weaponBeam.SetActive(false);
                items[i].sprite.SetActive(true);
            }
            yield return new WaitForSecondsRealtime(0.3f);

            // 同时显示第四个和第五个物品
            for (int i = 3; i <= 4; i++)
            {
                items[i].weaponBeam.SetActive(false);
                items[i].sprite.SetActive(true);
            }
            yield return new WaitForSecondsRealtime(0.3f);
        }
        else
        {
            // 其他物品数量的回退 - 正常逐个显示
            for (int i = 0; i < noOfBeams; i++)
            {
                items[i].weaponBeam.SetActive(false);
                items[i].sprite.SetActive(true);
                yield return new WaitForSecondsRealtime(0.3f);
            }
        }
    }
    
    /// <summary>
    /// 宝箱开启的视觉效果逻辑，包括烟花、光束和硬币显示。
    /// </summary>
    /// <returns>协程枚举器。</returns>
    public IEnumerator Open()
    {
        // 如果有烟花光束则触发
        if (dropProfile.hasFireworks)
        {
            isAnimating = false; // 如果有烟花，确保动画不能被跳过
            StartCoroutine(FlashWhite(chestPanel, 5)); // 或者任何你想闪烁的UI元素
            fireworks.SetActive(true);
            yield return new WaitForSecondsRealtime(dropProfile.fireworksDelay);
        }

        isAnimating = true; // 允许跳过动画

        // 如果有弯曲光束则触发
        if (dropProfile.hasCurvedBeams)
        {
            StartCoroutine(ActivateCurvedBeams(dropProfile.curveBeamsSpawnTime));
        }

        // 设置要接收的硬币。
        StartCoroutine(HandleCoinDisplay(Random.Range(dropProfile.minCoins, dropProfile.maxCoins)));

        DisplayBeam(dropProfile.noOfItems);
        openingVFX.SetActive(true);
        beamVFX.SetActive(true);

        yield return new WaitForSecondsRealtime(dropProfile.animDuration); // 动画持续时间
        openingVFX.SetActive(false);
    }
    
    /// <summary>
    /// 开始宝箱开启动画序列。
    /// </summary>
    public void Begin()
    {
        chestCover.SetActive(false);
        chestButton.SetActive(false);
        chestSequenceCoroutine = StartCoroutine(Open());
        audiosource.clip = dropProfile.openingSound;
        audiosource.Play();
    }
    
    /// <summary>
    /// 跳过当前动画并直接显示所有奖励。
    /// </summary>
    private void SkipToRewards()
    {
        if (chestSequenceCoroutine != null)
            StopCoroutine(chestSequenceCoroutine);

        StopAllCoroutines(); // 停止所有协程

        // 立即显示所有光束和图标
        for (int i = 0; i < icons.Count; i++)
        {
            SetupBeam(i);
            items[i].weaponBeam.SetActive(false);
            items[i].sprite.SetActive(true);
        }

        // 设置硬币价值
        coinText.gameObject.SetActive(true);
        coinText.text = coins.ToString("F2");
        doneButton.SetActive(true);
        openingVFX.SetActive(false);
        isAnimating = false;
        chestPanel.color = originalColor;

        // 跳到音频的最后一秒
        if (audiosource != null && dropProfile.openingSound != null)
        {
            audiosource.clip = dropProfile.openingSound;

            float skipToTime = Mathf.Max(0, audiosource.clip.length - 3.55f); // 确保它不会低于0
            audiosource.time = skipToTime;
            audiosource.Play();
        }
    }
    
    private void Update()
    {
        // 只有在动画播放且按下Esc键时才允许跳过动画
        if (isAnimating && Input.GetButtonDown("Cancel"))
        {
            SkipToRewards();
        }

        // 打开和完成按钮可使用回车键
        if (Input.GetKeyDown(KeyCode.Return))
        {
            TryPressButton(chestButton);
            TryPressButton(doneButton);
        }
    }

    /// <summary>
    /// 尝试按下指定按钮（如果按钮处于激活状态且可交互）。
    /// </summary>
    /// <param name="buttonObj">要按下的按钮对象。</param>
    private void TryPressButton(GameObject buttonObj)
    {
        if (buttonObj.activeInHierarchy)
        {
            Button btn = buttonObj.GetComponent<Button>();
            if (btn != null && btn.interactable)
            {
                btn.onClick.Invoke();
            }
        }
    }
    
    /// <summary>
    /// 关闭宝箱UI并重置所有相关状态。
    /// </summary>
    public void CloseUI()
{
    // 显示获得的硬币
    collector.AddCoins(coins);

    // 重置 UI 和 VFX 到初始状态
    chestCover.SetActive(true);
    chestButton.SetActive(true);
    icons.Clear();
    beamVFX.SetActive(false);
    coinText.gameObject.SetActive(false);
    gameObject.SetActive(false);
    doneButton.SetActive(false);
    fireworks.SetActive(false);
    curvedBeams.SetActive(false);
    ResetDisplay();

    // 重置音频
    audiosource.clip = pickUpSound;
    audiosource.time = 0f;
    audiosource.Play();

    isAnimating = false;

    GameManager.instance.ChangeState(GameManager.GameState.Gameplay);
    _currentChest.NotifyComplete();
}

    
    /// <summary>
    /// 重置所有物品显示组件到初始状态。
    /// </summary>
    private void ResetDisplay()
    {
        foreach (var item in items)
        {
            item.beam.SetActive(false);
            item.sprite.SetActive(false);
            item.spriteImage.sprite = null;
        }

        dropProfile = null;
        icons.Clear();
    }
}
