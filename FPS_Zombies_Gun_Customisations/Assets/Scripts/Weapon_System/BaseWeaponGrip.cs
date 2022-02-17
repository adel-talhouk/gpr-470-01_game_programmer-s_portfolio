using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseWeaponGrip
{
    //Amount of recoil
    [Range(0.1f, 1.0f)] public float recoil;
}
