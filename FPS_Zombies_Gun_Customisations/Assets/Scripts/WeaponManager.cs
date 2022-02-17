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

    void Update()
    {
        //INPUT - Mouse 0 - Fire
        if (Input.GetMouseButtonDown(0))
        {
            Fire();
        }
    }

    void Fire()
    {

    }

    IEnumerator Reload()
    {

    }
}
