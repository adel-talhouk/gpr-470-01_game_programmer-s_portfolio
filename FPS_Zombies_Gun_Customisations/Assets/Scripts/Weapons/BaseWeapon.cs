using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//DERIVE FROM THIS CLASS for each weapon type. This allows the player to hold multiple weapons and to have different firign modes (full-auto, semi-auto)
public abstract class BaseWeapon : MonoBehaviour
{
    //Fire
    public abstract void Fire();

    //Aim Down Sights
    public abstract void ADS();

    //Stop Aim Down Sights
    public abstract void StopADS();
}
