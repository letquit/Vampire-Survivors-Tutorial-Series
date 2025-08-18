using System;
using UnityEngine;

[Obsolete("This has been replaced by CharacterData.")]
[CreateAssetMenu(fileName = "CharacterScriptableObject", menuName = "ScriptableObjects/Character")]
public class CharacterScriptableObject : ScriptableObject
{
    [field:SerializeField]
    public Sprite icon { get; private set; }
    [field:SerializeField]
    public new string name { get; private set; }
    [field:SerializeField]
    public GameObject startingWeapon { get; private set; }
    [field:SerializeField]
    public float maxHealth { get; private set; }
    [field:SerializeField]
    public float recovery { get; private set; }
    [field:SerializeField]
    public float moveSpeed { get; private set; }
    [field:SerializeField]
    public float might { get; private set; }
    [field:SerializeField]
    public float projectileSpeed { get; private set; }
    [field:SerializeField]
    public float magnet { get; private set; }
}
