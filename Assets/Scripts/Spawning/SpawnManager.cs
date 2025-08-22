using UnityEngine;

/// <summary>
/// 敌人生成管理器，用于控制游戏中的敌人波次生成逻辑。
/// </summary>
public class SpawnManager : MonoBehaviour
{
    int currentWaveIndex; // 当前波的索引 [记住，列表从0开始]
    int currentWaveSpawnCount = 0; // 跟踪当前波已经生成了多少敌人。

    public WaveData[] data;
    public Camera referenceCamera;

    [Tooltip("如果敌人数量超过这个数值，停止生成更多敌人。为了性能考虑。")]
    public int maximumEnemyCount = 300;

    float spawnTimer; // 用于确定何时生成下一组敌人的计时器。
    float currentWaveDuration = 0f;
    public bool boostedByCurse = true;

    public static SpawnManager instance;

    /// <summary>
    /// 初始化单例实例。
    /// 如果场景中存在多个SpawnManager实例，会输出警告信息。
    /// </summary>
    void Start()
    {
        if (instance) Debug.LogWarning("场景中有多个Spawn Manager！请移除多余的实例。");
        instance = this;
    }

    /// <summary>
    /// 每帧更新敌人生成逻辑和波次状态。
    /// 包括计时器更新、波次切换判断以及敌人生成处理。
    /// </summary>
    void Update()
    {
        // 每帧更新生成计时器。
        spawnTimer -= Time.deltaTime;
        currentWaveDuration += Time.deltaTime;

        if (spawnTimer <= 0)
        {
            // 检查我们是否准备好进入下一波。
            if (HasWaveEnded())
            {
                currentWaveIndex++;
                currentWaveDuration = currentWaveSpawnCount = 0;

                // 如果已经通过所有波次，禁用此组件。
                if (currentWaveIndex >= data.Length)
                {
                    Debug.Log("所有波次已生成！关闭。", this);
                    enabled = false;
                }

                return;
            }

            // 如果不满足生成条件，则不生成敌人。
            if (!CanSpawn())
            {
                ActivateCooldown();
                return;
            }

            // 获取本次tick要生成的敌人数组。
            GameObject[] spawns = data[currentWaveIndex].GetSpawns(EnemyStats.count);

            // 遍历并生成所有预制体。
            foreach (GameObject prefab in spawns)
            {
                // 如果超过限制，则停止生成敌人。
                if (!CanSpawn()) continue;

                // 生成敌人。
                Instantiate(prefab, GeneratePosition(), Quaternion.identity);
                currentWaveSpawnCount++;
            }

            ActivateCooldown();
        }
    }
    
    /// <summary>
    /// 根据当前波次配置激活冷却时间，可能受到诅咒加成影响。
    /// </summary>
    public void ActivateCooldown()
    {
        float curseBoost = boostedByCurse ? GameManager.GetCumulativeCurse() : 1;
        spawnTimer += data[currentWaveIndex].GetSpawnInterval() / curseBoost;
    }

    /// <summary>
    /// 判断当前是否可以继续生成敌人。
    /// </summary>
    /// <returns>如果可以生成敌人返回true，否则返回false。</returns>
    public bool CanSpawn()
    {
        // 如果超过最大限制，则不再生成。
        if (HasExceededMaxEnemies()) return false;

        // 如果超过当前波的最大生成数量，则不再生成。
        if (instance.currentWaveSpawnCount > instance.data[instance.currentWaveIndex].totalSpawns) return false;

        // 如果超过波的持续时间，则不再生成。
        if (instance.currentWaveDuration > instance.data[instance.currentWaveIndex].duration) return false;
        return true;
    }

    /// <summary>
    /// 判断当前场景中的敌人数量是否超过了最大限制。
    /// </summary>
    /// <returns>如果敌人数量超过限制返回true，否则返回false。</returns>
    public static bool HasExceededMaxEnemies()
    {
        if (!instance) return false; // 如果没有生成管理器，不限制最大敌人数量。
        if (EnemyStats.count > instance.maximumEnemyCount) return true;
        return false;
    }

    /// <summary>
    /// 判断当前波次是否已经结束。
    /// 根据波次配置的退出条件进行判断（如持续时间、总生成数、必须全部击杀等）。
    /// </summary>
    /// <returns>如果当前波次已结束返回true，否则返回false。</returns>
    public bool HasWaveEnded()
    {
        WaveData currentWave = data[currentWaveIndex];

        // 如果波次持续时间是退出条件之一，检查波次已经运行了多长时间。
        // 如果当前波次持续时间不大于波次持续时间，则不退出。
        if ((currentWave.exitConditions & WaveData.ExitCondition.waveDuration) > 0)
            if (currentWaveDuration < currentWave.duration)
                return false;

        // 如果达到总生成数是退出条件之一，检查是否已经生成了足够的敌人。如果没有，则返回false。
        if ((currentWave.exitConditions & WaveData.ExitCondition.reachedTotalSpawns) > 0)
            if (currentWaveSpawnCount < currentWave.totalSpawns)
                return false;

        // 否则，如果必须全部击杀被选中，我们需要确保没有更多的敌人存在。
        if (currentWave.mustKillAll && EnemyStats.count > 0)
            return false;

        return true;
    }

    /// <summary>
    /// 在编辑器中重置时调用，设置默认参考相机为主相机。
    /// </summary>
    void Reset()
    {
        referenceCamera = Camera.main;
    }
    
    /// <summary>
    /// 生成一个位于相机视口边界外的新位置，用于放置敌人。
    /// </summary>
    /// <returns>可用于生成敌人的世界坐标位置。</returns>
    public static Vector3 GeneratePosition()
    {
        // 如果没有参考相机，则获取一个。
        if (!instance.referenceCamera) instance.referenceCamera = Camera.main;

        // 如果参考相机不是正交的，请发出警告。
        if (!instance.referenceCamera.orthographic)
            Debug.LogWarning("参考相机不是正交的！这将导致敌人有时出现在相机边界内！");

        // 使用两个随机数生成一个在相机边界外的位置。
        float x = Random.Range(0f, 1f), y = Random.Range(0f, 1f);

        // 然后，随机选择我们是否要四舍五入x或y值。
        switch (Random.Range(0, 2))
        {
            case 0: default:
                return instance.referenceCamera.ViewportToWorldPoint(new Vector3(Mathf.Round(x), y));
            case 1:
                return instance.referenceCamera.ViewportToWorldPoint(new Vector3(x, Mathf.Round(y)));
        }
    }

    /// <summary>
    /// 检查指定对象是否在相机视口边界内。
    /// </summary>
    /// <param name="checkedObject">需要检查的游戏对象变换组件。</param>
    /// <returns>如果对象在相机视口内返回true，否则返回false。</returns>
    public static bool IsWithinBoundaries(Transform checkedObject)
    {
        // 获取相机以检查我们是否在边界内。
        Camera c = instance && instance.referenceCamera ? instance.referenceCamera : Camera.main;

        Vector2 viewport = c.WorldToViewportPoint(checkedObject.position);
        if (viewport.x < 0f || viewport.x > 1f) return false;
        if (viewport.y < 0f || viewport.y > 1f) return false;
        return true;
    }
}
