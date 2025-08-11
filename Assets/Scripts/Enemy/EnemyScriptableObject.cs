using UnityEngine;

[CreateAssetMenu(fileName = "EnemyScriptableObject", menuName = "ScriptableObjects/Enemy")]
public class EnemyScriptableObject : ScriptableObject
{
    [SerializeField]
    private float moveSpeed;
    public float MoveSpeed => moveSpeed;
    [SerializeField]
    private float maxHealth;
    public float MaxHealth => maxHealth;
    [SerializeField]
    private float damage;
    public float Damage => damage;
}
