using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WeaponFullAuto : BaseWeapon
{
    [Header("Weapon Attachments")]
    public BaseWeaponBarrel barrel;
    public BaseWeaponGrip grip;
    public BaseWeaponMag mag;
    public BaseWeaponStock stock;

    [Header("Other References")]
    public Transform cameraTransform;
    public Transform firePointTransform;
    public GameObject tempFireHitPrefab;
    public GameObject damageIndicatorPrefab;

    [Header("UI")]
    public TextMeshProUGUI ammoCountText;
    public TextMeshProUGUI warningsText;

    //Private data
    Animator animator;
    IEnumerator fire;
    IEnumerator reload;

    //---------------Mag data---------------
    int currentAmmoCount;
    int reserveAmmoCount;

    //Helper data
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

        //Set UI
        ammoCountText.text = currentAmmoCount + "/" + reserveAmmoCount;
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
        
        //INPUT MOUSE HELD - Mouse 0 - Fire
        if (Input.GetMouseButton(0) && bCanFire)
        {
            //If trying to shoot with no ammo left
            if (currentAmmoCount + reserveAmmoCount == 0)
            {
                //Reload - indicate there is no ammo left
                if (reload != null)
                {
                    StopCoroutine(reload);
                }

                reload = Reload();
                StartCoroutine(Reload());
            }
            else
            {
                if (fire != null)
                {
                    StopCoroutine(fire);
                }

                fire = Fire();
                StartCoroutine(fire);
            }
        }

        //INPUT KEY DOWN - 'R' - Reload
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (reload != null)
            {
                StopCoroutine(reload);
            }

            reload = Reload();
            StartCoroutine(Reload());
        }
    }

    public override IEnumerator Fire()
    {
        //Prepare to save data
        RaycastHit rayHit;

        //Set fire direction
        if (!bIsADSing)
        {
            //Fire a ray forward and fire direction goes towards the first target within max range
            if (Physics.Raycast(firePointTransform.position, cameraTransform.forward, out rayHit, barrel.damageReductionRange * 4f))
            {
                firePointTransform.forward = (rayHit.point - firePointTransform.position).normalized;
                fireDirection = firePointTransform.forward;
            }
        }
        else
        {
            //Fire straight ahead
            fireDirection = cameraTransform.forward;
        }

        //Fire
        Physics.Raycast(firePointTransform.position, fireDirection, out rayHit, barrel.damageReductionRange * 4f);
        bCanFire = false;

        //Reduce ammo count and update UI
        currentAmmoCount--;
        ammoCountText.text = currentAmmoCount + "/" + reserveAmmoCount;

        //Auto reload
        if (currentAmmoCount == 0)
        {
            if (reload != null)
            {
                StopCoroutine(reload);
            }

            reload = Reload();
            StartCoroutine(Reload());
        }

        //TO-DO: Player SFX and particles


        //If it hit something
        if (rayHit.transform != null)
        {
            float damageToApply = damage;

            //TEMP - Spawn sphere there     TO-DO: REMOVE
            GameObject hitPointIndicator = Instantiate(tempFireHitPrefab, rayHit.point, Quaternion.identity);

            //Check distance, apply damage falloff
            if ((rayHit.transform.position - transform.position).sqrMagnitude >= barrel.damageReductionRange * barrel.damageReductionRange)
            {
                damageToApply *= barrel.damageFalloffMultiplier;
            }

            //TO-DO: Apply damage
            //if (rayHit.collider.gameObject.CompareTag("Enemy"))
            //{
            //    

                //Indicate damage
                GameObject damageIndicator = Instantiate(damageIndicatorPrefab, rayHit.point + new Vector3(0f, 2f, -2f), Quaternion.identity);
                damageIndicator.GetComponentInChildren<TextMeshProUGUI>().text = damageToApply.ToString();
            //}
        }

        //Cooldown
        yield return new WaitForSeconds(rateOfFire);

        bCanFire = true;
    }

    public override void ADS()
    {
        animator.SetBool("bIsAiming", true);

        bIsADSing = true;
    }

    public override void StopADS()
    {
        animator.SetBool("bIsAiming", false);

        bIsADSing = false;
    }

    IEnumerator Reload()
    {
        //Account for any ammo that remains in the clip
        int remainingAmmoInClip = currentAmmoCount;
        int totalRemainingAmmo = reserveAmmoCount + remainingAmmoInClip;
        int ammoTopUp;

        //If no ammo left
        if (totalRemainingAmmo == 0)
        {
            warningsText.text = "OUT OF AMMO";

            //TO-DO: Play SFX


            //THIS CAN BE REMOVED, AND ALL CODE IN THIS if-statement afterwards if you want the OUT OF AMMO text to persist
            yield return new WaitForSeconds(mag.reloadTime);

            //Update UI
            ammoCountText.text = currentAmmoCount + "/" + reserveAmmoCount;
            warningsText.text = "";

            bCanFire = true;
        }
        else if (reserveAmmoCount == 0 || remainingAmmoInClip == mag.magSize)   //If trying to reload with a full mag or no reserve ammo
        {
            //Do nothing - keep allowing player to fire
        }
        //Add as much remaining ammo as possible
        else if (totalRemainingAmmo >= mag.magSize)
        {
            ammoTopUp = mag.magSize;

            //Update ammo count and reserve ammo count
            currentAmmoCount = ammoTopUp;
            reserveAmmoCount -= ammoTopUp - remainingAmmoInClip;

            //UI indicator
            warningsText.text = "RELOADING";

            //Do not fire
            if (fire != null)
            {
                StopCoroutine(fire);
            }

            bCanFire = false;

            yield return new WaitForSeconds(mag.reloadTime);

            //Update UI
            ammoCountText.text = currentAmmoCount + "/" + reserveAmmoCount;
            warningsText.text = "";

            bCanFire = true;
        }
        else
        {
            ammoTopUp = totalRemainingAmmo;

            //Update ammo count and reserve ammo count
            currentAmmoCount = ammoTopUp;
            reserveAmmoCount -= ammoTopUp - remainingAmmoInClip;

            //UI indicator
            warningsText.text = "RELOADING";

            //Do not fire
            if (fire != null)
            {
                StopCoroutine(fire);
            }

            bCanFire = false;

            yield return new WaitForSeconds(mag.reloadTime);

            //Update UI
            ammoCountText.text = currentAmmoCount + "/" + reserveAmmoCount;
            warningsText.text = "";

            bCanFire = true;
        }
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
