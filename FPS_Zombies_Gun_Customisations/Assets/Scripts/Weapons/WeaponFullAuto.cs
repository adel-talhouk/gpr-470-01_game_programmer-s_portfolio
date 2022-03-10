using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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
    public Transform adsPointTransform;
    public GameObject tempFireHitPrefab;
    public GameObject damageIndicatorPrefab;
    public FirstPersonCameraController fpCameraController;

    [Header("UI")]
    public TextMeshProUGUI ammoCountText;
    public TextMeshProUGUI warningsText;
    public Slider rangeSlider;
    public Slider recoilSlider;
    public Slider adsSpeedSlider;
    public Slider handlingSlider;

    //Private data
    IEnumerator fire;
    IEnumerator reload;

    //---------------Mag data---------------
    int currentAmmoCount;
    int reserveAmmoCount;

    //---------------Stock data---------------
    Vector3 weaponSwayTargetPos;
    Vector3 originalWeaponPos;
    float swayTime;
    float adsTime;

    //---------------Camera data---------------
    Vector3 cameraCurrentRotation;
    Vector3 cameraTargetRotation;
    Quaternion cameraReturnToRotation;

    //Helper data
    bool bCanFire = true;
    bool bIsADSing = false;
    Vector3 fireDirection;

    void Start()
    {
        //Set starting ammo
        currentAmmoCount = mag.magSize;
        reserveAmmoCount = currentAmmoCount * mag.additionalMagCount;

        //Weapon data
        originalWeaponPos = transform.localPosition;
        swayTime = stock.weaponSwayMoveDuration;
        adsTime = 1f / stock.adsSpeedInverse;

        //Set UI --> slider values are modified so values are between 0 and 1!
        ammoCountText.text = currentAmmoCount + "/" + reserveAmmoCount;
        rangeSlider.value = barrel.damageReductionRange * 0.01f;
        recoilSlider.value = grip.recoilStrengthX + grip.recoilStrengthY - grip.recoilReturnStrength + 1f;
        adsSpeedSlider.value = adsTime;
        handlingSlider.value = stock.weaponSwayRadius * stock.weaponSwayMoveDuration / 6f;
    }

    void Update()
    {
        if (fpCameraController.BMenuIsOpen)
            return;

        //INPUT MOUSE HELD - Mouse 1 - Aim Down Sights
        if (Input.GetMouseButton(1))
        {
            ADS();
        }
        else
        {
            StopADS();
        }

        //Weapon Sway
        WeaponSway();

        //Lerp camera back to original position --> recoil doesn't work, so this is unnecessary. This also blocks player from being able to aim up or down
        LerpCameraBack();

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

        //Save camera transform to return to for recoil
        //cameraReturnToRotation = cameraTransform.localRotation;
        cameraReturnToRotation = firePointTransform.rotation;

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


        //Recoil
        cameraTargetRotation -= new Vector3(grip.recoilStrengthY, grip.recoilStrengthX, 0f);

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

            //Indicate damage
            GameObject damageIndicator = Instantiate(damageIndicatorPrefab, rayHit.point + new Vector3(0f, 2f, -2f), Quaternion.identity);
            damageIndicator.GetComponentInChildren<TextMeshProUGUI>().text = damageToApply.ToString();
        }

        //Cooldown
        yield return new WaitForSeconds(rateOfFire);

        bCanFire = true;
    }

    public override void ADS()
    {
        //Slerp position to it
        transform.localPosition = Vector3.Lerp(transform.localPosition, adsPointTransform.localPosition, adsTime * Time.deltaTime);
        bIsADSing = true;
    }

    public override void StopADS()
    {
        //Slerp position to it
        transform.localPosition = Vector3.Lerp(transform.localPosition, originalWeaponPos, adsTime * Time.deltaTime);
        bIsADSing = false;
    }

    void WeaponSway()
    {
        //Learned how to get point in random radius from: https://answers.unity.com/questions/1719092/calculate-a-random-point-inside-a-circlehow-to-ran.html

        //Random sway - point in the radius
        if (bIsADSing)
        {
            //Apply multiplier
            Vector2 swayPoint = Random.insideUnitCircle * stock.weaponSwayRadius * stock.weaponSwayADSMultiplier;

            weaponSwayTargetPos = originalWeaponPos + new Vector3(swayPoint.x, swayPoint.y, 0f);
        }
        else
        {
            Vector2 swayPoint = Random.insideUnitCircle * stock.weaponSwayRadius;

            weaponSwayTargetPos = originalWeaponPos + new Vector3(swayPoint.x, swayPoint.y, 0f);
        }

        //Lerp position to it
        transform.localPosition = Vector3.Lerp(transform.localPosition, weaponSwayTargetPos, swayTime * Time.deltaTime);
    }

    void LerpCameraBack()   //TO-DO: Let it allow me to fire up or down
    {
        //cameraTargetRotation = Vector3.Lerp(cameraTargetRotation, cameraReturnToRotation * Vector3.forward, grip.recoilReturnStrength * Time.deltaTime);
        cameraTargetRotation = Vector3.Lerp(cameraTargetRotation, cameraReturnToRotation * cameraTransform.forward, grip.recoilReturnStrength * Time.deltaTime);
        cameraCurrentRotation = Vector3.Slerp(cameraCurrentRotation, cameraTargetRotation, grip.recoilReturnSnappiness * Time.fixedDeltaTime);
        cameraTransform.localRotation = Quaternion.Euler(cameraCurrentRotation);
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

    public void SetNewBarrelType(BaseWeaponBarrel newBarrel)
    {
        barrel = newBarrel;

        //UI
        rangeSlider.value = barrel.damageReductionRange * 0.01f;
    }

    public void SetNewStockType(BaseWeaponStock newStock)
    {
        stock = newStock;

        //Set values
        swayTime = stock.weaponSwayMoveDuration;
        adsTime = 1f / stock.adsSpeedInverse;

        //UI
        adsSpeedSlider.value = adsTime;
        handlingSlider.value = stock.weaponSwayRadius * stock.weaponSwayMoveDuration / 6f;
    }

    public void SetNewGripType(BaseWeaponGrip newGrip)
    {
        grip = newGrip;

        //UI
        recoilSlider.value = grip.recoilStrengthX + grip.recoilStrengthY - grip.recoilReturnStrength + 1f;
    }

    public void SetNewMagType(BaseWeaponMag newMag)
    {
        mag = newMag;

        //Set values
        currentAmmoCount = mag.magSize;
        reserveAmmoCount = currentAmmoCount * mag.additionalMagCount;

        //Set UI
        ammoCountText.text = currentAmmoCount + "/" + reserveAmmoCount;
    }
}
