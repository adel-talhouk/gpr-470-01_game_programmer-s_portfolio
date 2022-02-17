using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseWeaponMag
{
    //How many bullets in each magazine
    public int magSize;

    //How many additional mags you get
    public int additionalMagCount;

    //Time to reload
    public float reloadTime;
}
