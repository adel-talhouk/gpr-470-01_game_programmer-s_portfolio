using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "WeaponAttachments/Barrel")]
public abstract class BaseWeaponBarrel
{
    //Damage falloff starts
    public float damageReductionRange;

    //Damage falloff percentage
    public float damageFalloffPercentage;
}
