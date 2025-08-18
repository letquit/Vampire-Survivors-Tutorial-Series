using System;
using UnityEngine;

[Obsolete]
[CreateAssetMenu(fileName = "PassiveItemScriptableObject", menuName = "ScriptableObjects/Passive Item")]
public class PassiveItemScriptableObject : ScriptableObject
{
    [field:SerializeField]
    public float multipler { get; private set; }
    [field:SerializeField]
    public int level { get; private set; }
    [field:SerializeField]
    public GameObject nextLevelPrefab { get; private set; }
    [field:SerializeField]
    public new string name { get; private set; }
    [field:SerializeField]
    public string description { get; private set; }
    [field:SerializeField]
    public Sprite icon { get; private set; }
    [field:SerializeField]
    public PassiveItemScriptableObject basePassiveItemData { get; private set; }
}
