using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "WeaponScriptableObject", menuName = "ScriptableObjects/Weapon")]
public class WeaponScriptableObject : ScriptableObject
{
    [field:SerializeField]
    public GameObject prefab { get; private set; }

    [field:SerializeField]
    public float damage { get; private set; }
    [field:SerializeField]
    public float speed { get; private set; }
    [field:SerializeField]
    public float cooldownDuration { get; private set; }
    [field:SerializeField]
    public int pierce { get; private set; }
    [field:SerializeField]
    public int level { get; private set; }
    [field:SerializeField]
    public GameObject nextLevelPrefab { get; private set; }
    [field:SerializeField]
    public Sprite icon { get; private set; }
}
