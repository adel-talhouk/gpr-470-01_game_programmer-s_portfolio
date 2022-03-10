using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "WeaponAttachments/Barrel")]
public class BaseWeaponBarrel : ScriptableObject
{
    //Damage falloff starts
    [Range(1f, 100f)] public float damageReductionRange;

    //Damage falloff multiplier
    [Range(0.1f, 1.0f)] public float damageFalloffMultiplier;
}
