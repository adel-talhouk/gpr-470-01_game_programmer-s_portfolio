using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "WeaponAttachments/Stock")]
public class BaseWeaponStock : ScriptableObject
{
    //Amount of weapon sway
    [Range(0.01f, 0.2f)] public float weaponSwayRadius;

    //ADS sway multiplier
    [Range(0.1f, 1.0f)] public float weaponSwayADSMultiplier;

    //Sway move duration
    [Range(0.1f, 1.0f)] public float weaponSwayMoveDuration;

    //Aim Down Sights speed
    [Range(1.0f, 2.0f)] public float adsSpeedMultiplier;
}
