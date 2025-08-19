using System;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

/// <summary>
/// 拾取物品类，用于处理玩家拾取物品的逻辑
/// </summary>
public class Pickup : MonoBehaviour
{
    /// <summary>
    /// 物品被收集后的存活时间（秒）
    /// </summary>
    public float lifespan = 0.5f;

    /// <summary>
    /// 目标玩家状态组件，用于在被拾取后应用奖励
    /// </summary>
    protected PlayerStats target;

    /// <summary>
    /// 向目标移动的速度
    /// </summary>
    protected float speed;

    /// <summary>
    /// 初始位置，用于未被拾取时的浮动动画计算
    /// </summary>
    private Vector2 initialPosition;

    /// <summary>
    /// 初始偏移量，用于每个物品独立的浮动动画相位
    /// </summary>
    private float initialOffset;
    
    /// <summary>
    /// 浮动动画参数结构体
    /// </summary>
    [Serializable]
    public struct BobbingAnimation
    {
        /// <summary>
        /// 浮动动画频率
        /// </summary>
        public float frequency;

        /// <summary>
        /// 浮动方向和幅度
        /// </summary>
        public Vector2 direction;
    }
    
    /// <summary>
    /// 浮动动画配置参数
    /// </summary>
    public BobbingAnimation bobbingAnimation = new BobbingAnimation
    {
        frequency = 2f, 
        direction = new Vector2(0, 0.3f)
    };

    /// <summary>
    /// 奖励经验值
    /// </summary>
    [Header("Bonuses")]
    public int experience;

    /// <summary>
    /// 奖励生命值
    /// </summary>
    public int health;

    /// <summary>
    /// 初始化浮动动画的初始偏移量
    /// </summary>
    protected virtual void Start()
    {
        initialPosition = transform.position;
        initialOffset = Random.Range(0, bobbingAnimation.frequency);
    }

    /// <summary>
    /// 每帧更新逻辑：如果已被拾取则向目标移动，否则执行浮动动画
    /// </summary>
    protected virtual void Update()
    {
        if (target)
        {
            // 计算与目标之间的距离并朝其移动
            Vector2 distance = target.transform.position - transform.position;
            if (distance.sqrMagnitude > speed * speed * Time.deltaTime)
                transform.position += (Vector3)distance.normalized * speed * Time.deltaTime;
            else
                Destroy(gameObject); // 接近目标时销毁自身
        }
        else
        {
            // 执行原地浮动动画
            transform.position = initialPosition + bobbingAnimation.direction *
                Mathf.Sin((Time.time + initialOffset) * bobbingAnimation.frequency);
        }
    }

    /// <summary>
    /// 尝试收集该物品
    /// </summary>
    /// <param name="target">收集该物品的玩家状态组件</param>
    /// <param name="speed">向玩家移动的速度</param>
    /// <param name="lifespan">收集后该物品的销毁延迟时间（秒）</param>
    /// <returns>是否成功开始收集过程</returns>
    public virtual bool Collect(PlayerStats target, float speed, float lifespan = 0f)
    {
        if (!this.target)
        {
            this.target = target;
            this.speed = speed;
            if (lifespan > 0) this.lifespan = lifespan;
            Destroy(gameObject, Mathf.Max(0.01f, this.lifespan));
            return true;
        }
        return false;
    }

    /// <summary>
    /// 物品销毁时应用奖励给玩家
    /// </summary>
    protected virtual void OnDestroy()
    {
        if (!target) return;
        if (experience != 0) target.IncreaseExperience(experience);
        if (health != 0) target.RestoreHealth(health);
    }
}
