using System;
using UnityEngine;

/// <summary>
/// 玩家动画控制器类，负责控制玩家角色的动画播放和精灵翻转
/// </summary>
public class PlayerAnimator : MonoBehaviour
{
    //References
    private Animator am;
    private PlayerMovement pm;
    private SpriteRenderer sr;

    /// <summary>
    /// 初始化组件引用，在游戏对象启动时调用
    /// </summary>
    private void Start()
    {
        am = GetComponent<Animator>();
        pm = GetComponent<PlayerMovement>();
        sr = GetComponent<SpriteRenderer>();
    }

    /// <summary>
    /// 每帧更新动画状态，根据玩家移动方向设置动画参数和精灵翻转
    /// </summary>
    private void Update()
    {
        // 检查玩家是否有移动输入
        if (pm.moveDir.x != 0 || pm.moveDir.y != 0)
        {
            am.SetBool("Move", true);
            
            SpriteDirectionChecker();
        }
        else
        {
            am.SetBool("Move", false);
        }
    }

    /// <summary>
    /// 检查并设置精灵的朝向，根据玩家最后的水平移动方向进行翻转
    /// </summary>
    private void SpriteDirectionChecker()
    {
        // 根据水平移动方向翻转精灵
        if (pm.lastHorizontalVector < 0)
        {
            sr.flipX = true;
        }
        else
        {
            sr.flipX = false;
        }
    }
    
    /// <summary>
    /// 设置动画控制器
    /// </summary>
    /// <param name="c">要设置的运行时动画控制器</param>
    public void SetAnimationController(RuntimeAnimatorController c)
    {
        if (!am) am = GetComponent<Animator>();
        am.runtimeAnimatorController = c;
    }
}

