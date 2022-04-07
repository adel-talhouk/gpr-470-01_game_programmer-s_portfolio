using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WeaponPistol : BaseWeapon
{
    [Header("UI Indicators")]
    public TextMeshProUGUI ammoCountText;
    public TextMeshProUGUI weaponNameText;
    public TextMeshProUGUI warningsText;

    //Component(s)
    Health healthScript;

    //Private data
    int currentAmmoCount;
    int reserveAmmoCount;

    //Firing and ADSing
    bool bCanFire = true;
    Vector3 fireDirection;

    //Coroutines
    IEnumerator fire;
    IEnumerator reload;

    // Start is called before the first frame update
    void Start()
    {
        //Set ammo count
        currentAmmoCount = base_magSize;
        reserveAmmoCount = base_magSize * base_additionalMagCount;

        healthScript = transform.root.GetComponent<Health>();

        //UpdateCurrentWeaponUI();
        ammoCountText.text = currentAmmoCount + "/" + reserveAmmoCount;
        weaponNameText.text = gameObject.name;
    }

    // Update is called once per frame
    void Update()
    {
        if (healthScript.BIsAlive)
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

            //INPUT MOUSE DOWN - Mouse 0 - Fire
            if (Input.GetMouseButtonDown(0) && bCanFire)
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
    }

    protected override IEnumerator Fire()
    {
        //Prepare to save data
        RaycastHit rayHit;

        //Set fire direction
        if (!base_bIsADSing)
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

        //If it hit a player
        if (rayHit.transform != null && rayHit.transform.CompareTag("Player"))
        {
            float damageToApply = base_damageValue;

            //Check distance, apply damage falloff
            if ((rayHit.transform.position - transform.position).sqrMagnitude >= base_damageReductionRange * base_damageReductionRange)
            {
                damageToApply *= base_damageFalloffMultiplier;
            }

            //Check headshot
            if (rayHit.transform.CompareTag("Hitbox_Head"))
            {
                damageToApply *= base_headshotDamageMultiplier;
            }

            //Indicate damage
            GameObject damageIndicator = Instantiate(base_damageIndicatorPrefab, rayHit.point, Quaternion.identity);
            damageIndicator.GetComponentInChildren<TextMeshProUGUI>().text = damageToApply.ToString();

            //Deal damage - round it
            rayHit.transform.GetComponent<Health>().TakeDamage((int)damageToApply);
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
            warningsText.text = "OUT OF AMMO. PRESS TAB TO SWAP WEAPONS";

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

            ////Update ammo count and reserve ammo count
            //currentAmmoCount = ammoTopUp;
            //reserveAmmoCount -= ammoTopUp - remainingAmmoInClip;

            //UI indicator
            warningsText.text = "RELOADING";

            //Do not fire
            if (fire != null)
            {
                StopCoroutine(fire);
            }

            bCanFire = false;

            yield return new WaitForSeconds(base_reloadTime);

            //Update ammo count and reserve ammo count
            currentAmmoCount = ammoTopUp;
            reserveAmmoCount -= ammoTopUp - remainingAmmoInClip;

            //Update UI
            ammoCountText.text = currentAmmoCount + "/" + reserveAmmoCount;
            warningsText.text = "";

            bCanFire = true;
        }
        else
        {
            ammoTopUp = totalRemainingAmmo;

            ////Update ammo count and reserve ammo count
            //currentAmmoCount = ammoTopUp;
            //reserveAmmoCount -= ammoTopUp - remainingAmmoInClip;

            //UI indicator
            warningsText.text = "RELOADING";

            //Do not fire
            if (fire != null)
            {
                StopCoroutine(fire);
            }

            bCanFire = false;

            yield return new WaitForSeconds(base_reloadTime);

            //Update ammo count and reserve ammo count
            currentAmmoCount = ammoTopUp;
            reserveAmmoCount -= ammoTopUp - remainingAmmoInClip;

            //Update UI
            ammoCountText.text = currentAmmoCount + "/" + reserveAmmoCount;
            warningsText.text = "";

            bCanFire = true;
        }
    }

    //public override void UpdateCurrentWeaponUI()
    //{
    //    ammoCountText.text = currentAmmoCount + "/" + reserveAmmoCount;
    //    weaponNameText.text = gameObject.name;
    //}

    public override void EnableWeapon()
    {
        //Reload if empty
        if (currentAmmoCount == 0)
            StartCoroutine(Reload());

        //Update UI
        ammoCountText.text = currentAmmoCount + "/" + reserveAmmoCount;
        weaponNameText.text = gameObject.name;
    }

    public override void DisableWeapon()
    {
        StopAllCoroutines();
        bCanFire = true;
        warningsText.text = "";
    }
}
