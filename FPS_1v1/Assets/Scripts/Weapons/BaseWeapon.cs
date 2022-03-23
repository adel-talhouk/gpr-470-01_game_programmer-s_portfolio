using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseWeapon : ScriptableObject
{
    [Header("Damage")]
    [SerializeField] [Range(10, 100)] protected int base_damageValue = 35;
    [SerializeField] [Range(1f, 2f)] protected float base_headshotDamageMultiplier = 2f;

    [Space(15)]
    [Header("Attackment Data")]
    [Header("Mag Data")]
    [SerializeField] protected int base_magSize = 30;
    [SerializeField] protected int base_additionalMagCount = 5;
    [SerializeField] [Range(0.25f, 2f)] protected float base_reloadTime = 0.75f;

    [Header("Grip Data")]
    [SerializeField] [Range(0.1f, 1.0f)] protected float base_recoilStrengthX;
    [SerializeField] [Range(0.1f, 1.0f)] protected float base_recoilStrengthY;
    [SerializeField] [Range(0.1f, 1.0f)] protected float base_recoilReturnSnappiness;
    [SerializeField] [Range(0.1f, 2.0f)] protected float base_recoilReturnStrength;

    [Header("Barrel Data")]
    [SerializeField] [Range(1f, 100f)] protected float base_damageReductionRange;
    [SerializeField] [Range(0.1f, 1.0f)] protected float base_damageFalloffMultiplier;

    [Header("Stock Data")]
    [SerializeField] [Range(0.1f, 2f)] protected float base_weaponSwayRadius;
    [SerializeField] [Range(0.1f, 1.0f)] protected float base_weaponSwayADSMultiplier;
    [SerializeField] [Range(0.5f, 3f)] protected float base_weaponSwayMoveDuration;
    [SerializeField] [Range(0.1f, 1.0f)] protected float base_adsSpeedInverse;

    //Derived methods
    protected abstract IEnumerator Fire();
    protected abstract IEnumerator Reload();
    protected abstract void ADS();
    protected abstract void StopADS();
}
