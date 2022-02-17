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

    //Private data
    Animator animator;

    //Helper data
    int currentAmmoCount;
    int reserveAmmoCount;
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

        //Set fire direction
        if (bIsADSing)  //If aiming
        {
            //Fire straight ahead
            fireDirection = cameraTransform.forward;
        }
        else    //Otherwise
        {
            //(geometry and math, send a raycast straight from camera, see what it hits, fire towards that)---------------------------------------------------------

        }
        
        //INPUT MOUSE HELD - Mouse 0 - Fire
        if (Input.GetMouseButton(0) && bCanFire)
        {
            Fire();
        }

    }

    public override void Fire()
    {

    }

    public override void ADS()
    {
        animator.SetBool("bIsAiming", true);
    }

    public override void StopADS()
    {
        animator.SetBool("bIsAiming", false);
    }

    IEnumerator Reload()
    {
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
