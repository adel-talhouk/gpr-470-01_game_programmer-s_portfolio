using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "WeaponAttachments/Stock")]
public class BaseWeaponStock : ScriptableObject
{
    //Amount of weapon sway
    [Range(0.1f, 1.0f)] public float weaponSway;
}
