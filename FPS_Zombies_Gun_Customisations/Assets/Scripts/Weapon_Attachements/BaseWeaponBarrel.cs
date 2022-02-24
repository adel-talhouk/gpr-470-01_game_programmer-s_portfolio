using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "WeaponAttachments/Barrel")]
public class BaseWeaponBarrel : ScriptableObject
{
    //Damage falloff starts
    public float damageReductionRange;

    //Damage falloff multiplier
    [Range(0.1f, 1.0f)] public float damageFalloffMultiplier;
}
