using System;
using UnityEngine;
using UnityEngine.U2D.Animation;
using Random = UnityEngine.Random;

public class ChangeWeapons : MonoBehaviour
{
    public SpriteLibrary spriteLibrary;
    public SpriteLibraryAsset[] weaponRefs;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            spriteLibrary.spriteLibraryAsset = weaponRefs[Random.Range(0, weaponRefs.Length)];
        }
    }
}
