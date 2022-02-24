using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponFullAuto : BaseWeapon
{
    [Header("Base Weapon Stats")]
    public float damage;
    public float rateOfFire;

    [Header("Weapon Attachments")]
    public BaseWeaponBarrel barrel;
    public BaseWeaponStock stock;
    public BaseWeaponGrip grip;
    public BaseWeaponMag mag;

    [Header("Other References")]
    public Transform cameraTransform;
    public Transform firePointTransform;

    //Private data
    Animator animator;

    //---------------Mag data---------------
    int currentAmmoCount;
    int reserveAmmoCount;

    //Helper data
    bool bCanFire = true;
    //bool bIsADSing = false;
    Vector3 fireDirection;

    void Start()
    {
        //Get components
        animator = GetComponent<Animator>();

        //Set starting ammo
        currentAmmoCount = mag.magSize;
        reserveAmmoCount = currentAmmoCount * mag.additionalMagCount;
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
        if (Input.GetMouseButton(0) && bCanFire)
        {
            Fire();
        }

        //INPUT KEY DOWN - 'R' - Reload
        if (Input.GetKeyDown(KeyCode.R) && reserveAmmoCount > 0)
        {
            Reload();
        }
    }

    public override void Fire()
    {
        if (bCanFire)
        {
            //Auto reload
            if (currentAmmoCount == 0 && reserveAmmoCount > 0)
            {
                Reload();
            }
        }
    }

    public override void ADS()
    {
        animator.SetBool("bIsAiming", true);

        //Fire straight ahead
        fireDirection = cameraTransform.forward;
    }

    public override void StopADS()
    {
        animator.SetBool("bIsAiming", false);

        RaycastHit rayHit;

        //TO-DO: Implement (geometry and math, send a raycast straight from camera, see what it hits, fire towards that)
        if (Physics.Raycast(firePointTransform.position, cameraTransform.forward, out rayHit, Mathf.Infinity))
        {
            firePointTransform.LookAt(rayHit.transform);
            fireDirection = firePointTransform.forward;
        }
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

        //TO-DO: Update UI


        bCanFire = false;

        yield return new WaitForSeconds(mag.reloadTime);

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
