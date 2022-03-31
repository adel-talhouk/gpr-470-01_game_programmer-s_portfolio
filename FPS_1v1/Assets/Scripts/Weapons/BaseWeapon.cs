using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseWeapon : MonoBehaviour
{
    [Header("Damage")]
    [SerializeField] [Range(10, 100)] protected int base_damageValue = 35;
    [SerializeField] [Range(1f, 2f)] protected float base_headshotDamageMultiplier = 2f;
    [Range(0.05f, 2.0f)] public float base_rateOfFire;

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
    [SerializeField] [Range(0.1f, 1.0f)] protected float base_adsSpeed;

    [Header("Misc.")]
    [SerializeField] protected Transform base_cameraTransform;
    [SerializeField] protected GameObject base_damageIndicatorPrefab;
    protected Transform base_firePointTransform;
    protected Transform base_adsPointTransform;

    //Setup weapon stuff here
    void Awake()
    {
        base_firePointTransform = transform.Find("FirePoint");
        base_adsPointTransform = transform.Find("ADSPoint");
    }

    //Derived methods
    protected abstract IEnumerator Fire();
    protected abstract IEnumerator Reload();
    protected abstract void ADS();
    protected abstract void StopADS();
}
