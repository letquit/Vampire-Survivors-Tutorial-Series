using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

/// <summary>
/// 健康条组件，用于显示和管理角色的生命值
/// </summary>
public class HealthBar : MonoBehaviour
{
    /// <summary>
    /// 健康值调整速率，控制健康条变化的平滑速度
    /// </summary>
    public float adjustRate = 120f;
    
    /// <summary>
    /// 最大健康值
    /// </summary>
    public float maxHealth;
    /// <summary>
    /// 当前健康值
    /// </summary>
    public float currentHealth;

    /// <summary>
    /// 健康条滑动条组件
    /// </summary>
    public Slider healthSlider;
    /// <summary>
    /// 健康条填充图片组件
    /// </summary>
    public Image fillImage;

    /// <summary>
    /// 当前健康值文本显示组件
    /// </summary>
    public TextMeshProUGUI txtCurrentHealth;
    /// <summary>
    /// 最大健康值文本显示组件
    /// </summary>
    public TextMeshProUGUI txtMaxHealth;
    /// <summary>
    /// 颜色渐变，用于根据健康值显示不同颜色
    /// </summary>
    public Gradient colorGradient;

    /// <summary>
    /// 初始化健康条组件，设置初始健康值和UI显示
    /// </summary>
    private void Awake()
    {
        currentHealth = maxHealth;
        healthSlider.value = NormalizedHealth();
        fillImage.color = colorGradient.Evaluate(NormalizedHealth());
        txtCurrentHealth.text = currentHealth.ToString("F0");
        txtMaxHealth.text = maxHealth.ToString("F0");
    }

    /// <summary>
    /// 每帧更新，检测按键输入并触发伤害效果
    /// </summary>
    private void Update()
    {
        // 检测G键按下，触发伤害效果
        if (Keyboard.current.gKey.wasPressedThisFrame)
        {
            StartCoroutine(TakeDamage(50f));
        }
    }

    /// <summary>
    /// 执行伤害效果，平滑地减少健康值并更新UI
    /// </summary>
    /// <param name="damage">造成的伤害值</param>
    /// <returns>协程迭代器</returns>
    private IEnumerator TakeDamage(float damage)
    {
        float originHealth = currentHealth;
        float targetHealth = currentHealth - damage;
        
        float timeToAdjust = damage / adjustRate;
        float timeElapsed = 0f;
        
        // 平滑过渡健康值变化
        while (timeElapsed < timeToAdjust)
        {
            currentHealth = Mathf.SmoothStep(originHealth, targetHealth, timeElapsed / timeToAdjust);
            healthSlider.value = NormalizedHealth();
            fillImage.color = colorGradient.Evaluate(NormalizedHealth());

            // 检查健康值是否归零
            if (currentHealth <= 0)
            {
                currentHealth = 0;
                txtCurrentHealth.text = currentHealth.ToString("F0");
                yield break;
            }
            txtCurrentHealth.text = currentHealth.ToString("F0");
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        currentHealth = targetHealth;
        fillImage.color = colorGradient.Evaluate(NormalizedHealth());
        txtCurrentHealth.text = currentHealth.ToString("F0");
    }

    /// <summary>
    /// 计算标准化的健康值（0-1之间的浮点数）
    /// </summary>
    /// <returns>标准化的健康值比例</returns>
    private float NormalizedHealth()
    {
        return currentHealth / maxHealth;
    }
}