using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[CreateAssetMenu(menuName = "Weapons/Full Auto")]
public class WeaponFullAuto : BaseWeapon
{
    [Header("UI Indicators")]
    public TextMeshProUGUI ammoCountText;
    public TextMeshProUGUI warningsText;

    //Private data
    int currentAmmoCount;
    int reserveAmmoCount;

    //Firing and ADSing
    bool bCanFire = true;
    bool bIsADSing = false;
    Vector3 fireDirection;
    Vector3 originalWeaponPos;

    //Coroutines
    IEnumerator fire;
    IEnumerator reload;

    // Start is called before the first frame update
    void Start()
    {
        //Set ammo count
        currentAmmoCount = base_magSize;
        reserveAmmoCount = base_magSize * base_additionalMagCount;

        ammoCountText.text = currentAmmoCount + "/" + reserveAmmoCount;

        originalWeaponPos = transform.localPosition;
    }

    // Update is called once per frame
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

    protected override IEnumerator Fire()
    {
        //Prepare to save data
        RaycastHit rayHit;

        //Set fire direction
        if (!bIsADSing)
        {
            //Fire a ray forward and fire direction goes towards the first target within max range
            if (Physics.Raycast(base_firePointTransform.position, base_cameraTransform.forward, out rayHit, base_damageReductionRange * 4f))
            {
                base_firePointTransform.forward = (rayHit.point - base_firePointTransform.position).normalized;
                fireDirection = base_firePointTransform.forward;
            }
        }
        else
        {
            //Fire straight ahead
            fireDirection = base_cameraTransform.forward;
        }

        //Fire
        Physics.Raycast(base_firePointTransform.position, fireDirection, out rayHit, base_damageReductionRange * 4f);
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
            float damageToApply = base_damageValue;

            //TEMP - Spawn sphere there     TO-DO: REMOVE
            GameObject hitPointIndicator = Instantiate(base_tempFireHitPrefab, rayHit.point, Quaternion.identity);

            //Check distance, apply damage falloff
            if ((rayHit.transform.position - transform.position).sqrMagnitude >= base_damageReductionRange * base_damageReductionRange)
            {
                damageToApply *= base_damageFalloffMultiplier;
            }

            //Indicate damage
            GameObject damageIndicator = Instantiate(base_damageIndicatorPrefab, rayHit.point + new Vector3(0f, 2f, -2f), Quaternion.identity);
            damageIndicator.GetComponentInChildren<TextMeshProUGUI>().text = damageToApply.ToString();
        }

        //Cooldown
        yield return new WaitForSeconds(base_rateOfFire);

        bCanFire = true;
    }

    protected override IEnumerator Reload()
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
            yield return new WaitForSeconds(base_reloadTime);

            //Update UI
            ammoCountText.text = currentAmmoCount + "/" + reserveAmmoCount;
            warningsText.text = "";

            bCanFire = true;
        }
        else if (reserveAmmoCount == 0 || remainingAmmoInClip == base_magSize)   //If trying to reload with a full mag or no reserve ammo
        {
            //Do nothing - keep allowing player to fire
        }
        //Add as much remaining ammo as possible
        else if (totalRemainingAmmo >= base_magSize)
        {
            ammoTopUp = base_magSize;

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

            yield return new WaitForSeconds(base_reloadTime);

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

            yield return new WaitForSeconds(base_reloadTime);

            //Update UI
            ammoCountText.text = currentAmmoCount + "/" + reserveAmmoCount;
            warningsText.text = "";

            bCanFire = true;
        }
    }

    protected override void ADS()
    {
        //Slerp position to it
        transform.localPosition = Vector3.Lerp(transform.localPosition, base_adsPointTransform.localPosition, base_adsSpeed * Time.deltaTime);
        bIsADSing = true;
    }

    protected override void StopADS()
    {
        //Slerp position to it
        transform.localPosition = Vector3.Lerp(transform.localPosition, originalWeaponPos, base_adsSpeed * Time.deltaTime);
        bIsADSing = false;
    }
}
