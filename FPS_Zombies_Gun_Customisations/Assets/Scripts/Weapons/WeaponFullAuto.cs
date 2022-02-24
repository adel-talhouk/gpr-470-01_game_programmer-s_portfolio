using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WeaponFullAuto : BaseWeapon
{
    [Header("Weapon Attachments")]
    public BaseWeaponBarrel barrel;
    public BaseWeaponGrip grip;
    public BaseWeaponMag mag;
    public BaseWeaponStock stock;

    [Header("Other References")]
    public Transform cameraTransform;
    public Transform firePointTransform;
    public GameObject tempFireHitPrefab;

    [Header("UI")]
    public TextMeshProUGUI ammoCountText;
    public TextMeshProUGUI warningsText;

    //Private data
    Animator animator;

    //---------------Mag data---------------
    int currentAmmoCount;
    int reserveAmmoCount;

    //Helper data
    bool bCanFire = true;
    bool bIsADSing = false;
    Vector3 fireDirection;

    void Start()
    {
        //Get components
        animator = GetComponent<Animator>();

        //Set starting ammo
        currentAmmoCount = mag.magSize;
        reserveAmmoCount = currentAmmoCount * mag.additionalMagCount;

        //Set UI
        ammoCountText.text = currentAmmoCount + "/" + reserveAmmoCount;
    }

    void Update()
    {
        //INPUT MOUSE HELD - Mouse 1 - Aim Down Sights
        if (Input.GetMouseButton(1))
        {
            ADS();
        }
        else
        {
            StopADS();
        }
        
        //INPUT MOUSE HELD - Mouse 0 - Fire
        if (Input.GetMouseButton(0) && bCanFire && currentAmmoCount > 0)
        {
            StartCoroutine(Fire());
        }

        //INPUT KEY DOWN - 'R' - Reload
        if (Input.GetKeyDown(KeyCode.R) && reserveAmmoCount > 0)
        {
            StartCoroutine(Reload());
        }
    }

    public override IEnumerator Fire()
    {
        //Prepare to save data
        RaycastHit rayHit;

        //Set fire direction
        if (bIsADSing)
        {
            //Fire straight ahead
            fireDirection = cameraTransform.forward;
        }
        else
        {
            //Fire a ray forward and fire direction goes towards the first target
            if (Physics.Raycast(firePointTransform.position, cameraTransform.forward, out rayHit, Mathf.Infinity))
            {
                firePointTransform.forward = (rayHit.point - firePointTransform.position).normalized;
                fireDirection = firePointTransform.forward;
            }
        }

        //Fire
        Physics.Raycast(firePointTransform.position, fireDirection, out rayHit, barrel.damageReductionRange * 4f);
        bCanFire = false;

        //TO-DO: Player SFX and particles


        //If it hit something
        if (rayHit.transform != null)
        {
            float damageToApply = damage;

            //TEMP - Spawn sphere there     TO-DO: REMOVE
            GameObject hitPointIndicator = Instantiate(tempFireHitPrefab, rayHit.point, Quaternion.identity);

            //Check distance, apply damage falloff
            if ((rayHit.transform.position - transform.position).sqrMagnitude >= barrel.damageReductionRange * barrel.damageReductionRange)
            {
                damageToApply *= barrel.damageFalloffMultiplier;
            }

            //TO-DO: Apply damage
            //if (rayHit.collider.gameObject.CompareTag("Enemy"))
            //{
            //    
            //}

            //Reduce ammo count and update UI
            currentAmmoCount--;
            ammoCountText.text = currentAmmoCount + "/" + reserveAmmoCount;
        }

        //Auto reload
        if (currentAmmoCount == 0 && reserveAmmoCount > 0)
        {
            StartCoroutine(Reload());
        }

        //Cooldown
        yield return new WaitForSeconds(rateOfFire);

        bCanFire = true;
    }

    public override void ADS()
    {
        animator.SetBool("bIsAiming", true);

        bIsADSing = true;
    }

    public override void StopADS()
    {
        animator.SetBool("bIsAiming", false);

        bIsADSing = false;
    }

    IEnumerator Reload()
    {
        //Account for any ammo that remains in the clip
        int remainingAmmoInClip = currentAmmoCount;
        int totalRemainingAmmo = reserveAmmoCount + remainingAmmoInClip;
        int ammoTopUp;

        //Add as much remaining ammo as possible
        if (totalRemainingAmmo >= mag.magSize)
        {
            ammoTopUp = mag.magSize;
        }
        else
        {
            ammoTopUp = totalRemainingAmmo;
        }

        //Update ammo count and reserve ammo count
        currentAmmoCount = ammoTopUp;
        reserveAmmoCount -= ammoTopUp - remainingAmmoInClip;

        //UI indicator
        warningsText.text = "RELOADING";

        bCanFire = false;

        yield return new WaitForSeconds(mag.reloadTime);

        //Update UI
        ammoCountText.text = currentAmmoCount + "/" + reserveAmmoCount;
        warningsText.text = "";

        bCanFire = true;
    }

    void SetNewBarrelType(BaseWeaponBarrel newBarrel)
    {
        barrel = newBarrel;

        //Set values

    }

    void SetNewStockType(BaseWeaponStock newStock)
    {
        stock = newStock;

        //Set values

    }

    void SetNewGripType(BaseWeaponGrip newGrip)
    {
        grip = newGrip;

        //Set values

    }

    void SetNewMagType(BaseWeaponMag newMag)
    {
        mag = newMag;

        //Set values
        currentAmmoCount = mag.magSize;
        reserveAmmoCount = currentAmmoCount * mag.additionalMagCount;
    }
}
