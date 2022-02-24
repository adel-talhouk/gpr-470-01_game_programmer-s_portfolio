using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//DERIVE FROM THIS CLASS for each weapon type. This allows the player to hold multiple weapons and to have different firing modes (full-auto, semi-auto)
//This will also allow for a weapon manager to have references to the BaseWeapon and have access to derived classes for easy switching.
public abstract class BaseWeapon : MonoBehaviour
{
    [Header("Base Weapon Stats")]
    [Range(1, 100)] public int damage;
    [Range(0.05f, 2.0f)] public float rateOfFire;

    //Fire
    public abstract IEnumerator Fire();

    //Aim Down Sights
    public abstract void ADS();

    //Stop Aim Down Sights
    public abstract void StopADS();
}
