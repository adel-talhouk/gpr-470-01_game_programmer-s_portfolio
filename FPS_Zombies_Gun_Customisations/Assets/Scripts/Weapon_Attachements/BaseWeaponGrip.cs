using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "WeaponAttachments/Grip")]
public class BaseWeaponGrip : ScriptableObject
{
    //Amount of recoil
    [Range(0.1f, 1.0f)] public float recoilStrengthX;
    [Range(0.1f, 1.0f)] public float recoilStrengthY;
    [Range(0.1f, 1.0f)] public float recoilReturnSnappiness;
    [Range(0.1f, 2.0f)] public float recoilReturnStrength;
}
