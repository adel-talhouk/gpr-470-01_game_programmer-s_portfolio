using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Weapons/Full Auto")]
public class WeaponFullAuto : BaseWeapon
{
    //Private data
    int currentAmmoCount;
    int reserveAmmoCount;

    // Start is called before the first frame update
    void Start()
    {
        //Set ammo count
        currentAmmoCount = base_magSize;
        reserveAmmoCount = base_magSize * base_additionalMagCount;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected override IEnumerator Fire()
    {
        throw new System.NotImplementedException();
    }

    protected override IEnumerator Reload()
    {
        throw new System.NotImplementedException();
    }

    protected override void ADS()
    {
        throw new System.NotImplementedException();
    }

    protected override void StopADS()
    {
        throw new System.NotImplementedException();
    }
}
