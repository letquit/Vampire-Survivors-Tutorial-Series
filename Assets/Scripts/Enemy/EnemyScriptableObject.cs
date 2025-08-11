using UnityEngine;

[CreateAssetMenu(fileName = "EnemyScriptableObject", menuName = "ScriptableObjects/Enemy")]
public class EnemyScriptableObject : ScriptableObject
{
    [field:SerializeField]
    public float moveSpeed { get; private set; }
    [field:SerializeField]
    public float maxHealth { get; private set; }
    [field:SerializeField]
    public float damage { get; private set; }
}
