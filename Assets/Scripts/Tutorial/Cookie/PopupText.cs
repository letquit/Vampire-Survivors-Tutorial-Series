using System;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// 弹出文本组件，用于显示带有动画效果的弹出文本（如点击获得的数值显示）
/// </summary>
public class PopupText : MonoBehaviour
{
    [SerializeField] private float startingVelocity = 750f;
    [SerializeField] private float velocityDecayRate = 1500f;
    [SerializeField] private float timeBeforeFadeStarts = 0.6f;
    [SerializeField] private float fadeSpeed = 3f;

    private TextMeshProUGUI _clickAmountText;

    private Vector2 _currentVelocity;

    private Color _startColor;
    private float _timer;
    private float _textAlpha;

    /// <summary>
    /// 当对象启用时的初始化方法
    /// 重置文本颜色、计时器和透明度
    /// </summary>
    private void OnEnable()
    {
        _clickAmountText = GetComponent<TextMeshProUGUI>();
        
        Color newColor = _clickAmountText.color;
        newColor.a = 1f;
        _clickAmountText.color = newColor;

        _startColor = newColor;
        _timer = 0f;
        _textAlpha = 1f;
    }

    /// <summary>
    /// 创建弹出文本实例
    /// </summary>
    /// <param name="amount">要显示的数值</param>
    /// <returns>创建的PopupText实例</returns>
    public static PopupText Create(double amount)
    {
        GameObject popupObj = ObjectPoolManager.SpawnObject(CookieManager.Instance.cookieTextPopup,
            CookieManager.Instance.mainGameCanvas.transform);
        popupObj.transform.position = CookieManager.Instance.mainGameCanvas.transform.position;
        
        PopupText cookiePopup = popupObj.GetComponent<PopupText>();
        cookiePopup.Init(amount);
        
        return cookiePopup;
    }

    /// <summary>
    /// 初始化弹出文本
    /// </summary>
    /// <param name="amount">要显示的数值</param>
    public void Init(double amount)
    {
        _clickAmountText.text = "+" + amount.ToString("0");
        
        float randomX = Random.Range(-400f, 400f);
        _currentVelocity = new Vector2(randomX, startingVelocity);
    }

    /// <summary>
    /// 更新弹出文本的位置和透明度
    /// 实现向上移动并逐渐消失的动画效果
    /// </summary>
    private void Update()
    {
        // 应用重力效果，减少垂直速度
        _currentVelocity.y -= Time.deltaTime * velocityDecayRate;
        // 根据当前速度移动文本对象
        transform.Translate(_currentVelocity * Time.deltaTime);
        
        _timer += Time.deltaTime;
        if (_timer > timeBeforeFadeStarts)
        {
            // 开始淡出效果
            _textAlpha -= Time.deltaTime * fadeSpeed;
            _startColor.a = _textAlpha;
            _clickAmountText.color = _startColor;

            if (_textAlpha <= 0f)
            {
                // 透明度为0时，将对象返回对象池
                ObjectPoolManager.ReturnObjectToPool(gameObject);
            }
        }
    }
}