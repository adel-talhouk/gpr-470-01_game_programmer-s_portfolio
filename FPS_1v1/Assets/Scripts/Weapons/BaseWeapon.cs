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
    [SerializeField] protected Transform base_adsPointTransform;
    protected Transform base_firePointTransform;

    //Helper data
    protected bool base_bIsADSing = false;
    Vector3 originalWeaponPos;

    //Setup weapon stuff here
    void Awake()
    {
        base_firePointTransform = transform.Find("FirePoint");

        originalWeaponPos = transform.localPosition;
    }

    //Public shared methods
    public abstract void DisableWeapon();

    //Derived methods
    public abstract void UpdateCurrentWeaponUI();
    protected abstract IEnumerator Fire();
    protected abstract IEnumerator Reload();

    protected void ADS()
    {
        //Slerp position to it
        transform.localPosition = Vector3.Lerp(transform.localPosition, base_adsPointTransform.localPosition, base_adsSpeed * Time.deltaTime * 10f);
        base_bIsADSing = true;
    }

    protected void StopADS()
    {
        //Slerp position to it
        transform.localPosition = Vector3.Lerp(transform.localPosition, originalWeaponPos, base_adsSpeed * Time.deltaTime * 10f);
        base_bIsADSing = false;
    }
}
