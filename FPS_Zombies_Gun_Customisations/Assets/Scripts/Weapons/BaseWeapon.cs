using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseWeapon : MonoBehaviour
{
    //Fire
    public abstract void Fire();

    //Aim Down Sights
    public abstract void ADS();

    //Stop Aim Down Sights
    public abstract void StopADS();
}
