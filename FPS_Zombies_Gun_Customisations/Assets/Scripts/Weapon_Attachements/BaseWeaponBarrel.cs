using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "WeaponAttachments/Barrel")]
public class BaseWeaponBarrel : ScriptableObject
{
    //Damage falloff starts
    public float damageReductionRange;

    //Damage falloff percentage
    public float damageFalloffPercentage;
}
