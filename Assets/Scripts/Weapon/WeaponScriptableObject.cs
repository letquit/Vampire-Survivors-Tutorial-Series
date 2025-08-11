using UnityEngine;

[CreateAssetMenu(fileName = "WeaponScriptableObject", menuName = "ScriptableObjects/Weapon")]
public class WeaponScriptableObject : ScriptableObject
{
    [SerializeField]
    private GameObject prefab;
    public GameObject Prefab => prefab;

    [SerializeField]
    private float damage;
    public float Damage => damage;
    [SerializeField]
    private float speed;
    public float Speed => speed;
    [SerializeField]
    private float cooldownDuration;
    public float CooldownDuration => cooldownDuration;
    [SerializeField]
    private int pierce;
    public int Pierce => pierce;
}
