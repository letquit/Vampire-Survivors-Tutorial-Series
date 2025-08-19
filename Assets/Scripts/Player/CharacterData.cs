using UnityEngine;

/// <summary>
/// 角色数据类，用于存储和管理角色的基本属性和配置信息
/// 继承自ScriptableObject，可以在Unity编辑器中创建资产文件
/// </summary>
[CreateAssetMenu(fileName = "Character Data", menuName = "2D Top-down Rogue-like/Character Data")]
public class CharacterData : ScriptableObject
{
    /// <summary>
    /// 角色图标精灵
    /// </summary>
    [SerializeField]
    private Sprite icon;
    /// <summary>
    /// 获取或设置角色图标
    /// </summary>
    public Sprite Icon { get => icon; private set => icon = value; }

    /// <summary>
    /// 运行时动画控制器
    /// </summary>
    public RuntimeAnimatorController controller;
    
    /// <summary>
    /// 角色名称
    /// </summary>
    [SerializeField]
    new string name;
    /// <summary>
    /// 获取或设置角色名称
    /// </summary>
    public string Name { get => name; private set => name = value; }

    /// <summary>
    /// 起始武器数据
    /// </summary>
    [SerializeField]
    private WeaponData startingWeapon;
    /// <summary>
    /// 获取或设置起始武器数据
    /// </summary>
    public WeaponData StartingWeapon { get => startingWeapon; private set => startingWeapon = value; }

    /// <summary>
    /// 角色属性结构体，包含角色的各项基础和增益属性
    /// </summary>
    [System.Serializable]
    public struct Stats
    {
        /// <summary>
        /// 最大生命值
        /// </summary>
        public float maxHealth;
        /// <summary>
        /// 生命恢复速度
        /// </summary>
        public float recovery;
        /// <summary>
        /// 移动速度
        /// </summary>
        public float moveSpeed;
        /// <summary>
        /// 力量属性，影响攻击伤害
        /// </summary>
        public float might;
        /// <summary>
        /// 速度属性，影响攻击速度等
        /// </summary>
        public float speed;
        /// <summary>
        /// 磁铁属性，影响物品拾取范围
        /// </summary>
        public float magnet;

        /// <summary>
        /// 构造函数，初始化角色属性
        /// </summary>
        /// <param name="maxHealth">最大生命值，默认1000</param>
        /// <param name="recovery">生命恢复速度，默认0</param>
        /// <param name="moveSpeed">移动速度，默认1f</param>
        /// <param name="might">力量属性，默认1f</param>
        /// <param name="speed">速度属性，默认1f</param>
        /// <param name="magnet">磁铁属性，默认30f</param>
        public Stats(float maxHealth = 1000, float recovery = 0, float moveSpeed = 1f, float might = 1f, float speed = 1f, float magnet = 30f)
        {
            this.maxHealth = maxHealth;
            this.recovery = recovery;
            this.moveSpeed = moveSpeed;
            this.might = might;
            this.speed = speed;
            this.magnet = magnet;
        }
        
        /// <summary>
        /// 重载加法运算符，用于属性叠加计算
        /// </summary>
        /// <param name="s1">第一个属性结构体</param>
        /// <param name="s2">第二个属性结构体</param>
        /// <returns>叠加后的属性结构体</returns>
        public static Stats operator +(Stats s1, Stats s2)
        {
            s1.maxHealth += s2.maxHealth;
            s1.recovery += s2.recovery;
            s1.moveSpeed += s2.moveSpeed;
            s1.might += s2.might;
            s1.speed += s2.speed;
            s1.magnet += s2.magnet;
            return s1;
        }
    }

    /// <summary>
    /// 角色基础属性，初始化为默认值
    /// </summary>
    public Stats stats = new Stats(1000);
}
