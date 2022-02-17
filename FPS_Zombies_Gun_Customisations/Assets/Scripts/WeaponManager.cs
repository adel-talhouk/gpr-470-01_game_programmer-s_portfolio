using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    //All the weapon extensions
    public BaseWeapon weapon;
    public BaseWeaponBarrel barrel;
    public BaseWeaponStock stock;
    public BaseWeaponGrip grip;
    public BaseWeaponMag mag;

    //Helper data
    int currentAmmoCount;
    int reserveAmmoCount;
    bool bCanFire = true;

    void Start()
    {
        //Set starting ammo
        currentAmmoCount = mag.magSize;
        reserveAmmoCount = currentAmmoCount * mag.additionalMagCount;
    }

    void Update()
    {
        //INPUT - Mouse 0 - Fire
        if (Input.GetMouseButtonDown(0) && bCanFire)
        {
            Fire();
        }
    }

    void Fire()
    {

    }

    IEnumerator Reload()
    {
        bCanFire = false;

        yield return new WaitForSeconds(mag.reloadTime);

        bCanFire = true;
    }

    void SetNewWeaponType(BaseWeapon newWeapon)
    {
        weapon = newWeapon;

        //Set values

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
