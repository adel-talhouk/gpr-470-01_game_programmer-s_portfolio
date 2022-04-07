using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//THIS CLASS WILL ALLOW THE PLAYER TO SWAP BETWEEN AND PICK UP WEAPONS AND AMMO

public class WeaponManager : MonoBehaviour
{
    [SerializeField] [Range(0.05f, 0.5f)] float weaponSwapCooldownTime = 0.1f;

    //Weapons
    BaseWeapon[] weaponsArray = new BaseWeapon[2];
    //BaseWeapon primaryWeapon;
    //BaseWeapon secondaryWeapon;

    //Helper data
    bool bCanSwapWeapons = true;
    int currentWeaponIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        //Set weapons
        //primaryWeapon = transform.GetChild(0).GetComponent<BaseWeapon>();
        //secondaryWeapon = transform.GetChild(1).GetComponent<BaseWeapon>();
        weaponsArray[0] = transform.GetChild(0).GetComponent<BaseWeapon>();
        weaponsArray[1] = transform.GetChild(1).GetComponent<BaseWeapon>();

        //Set primary weapon as active by default
        //primaryWeapon.gameObject.SetActive(true);
        //secondaryWeapon.gameObject.SetActive(false);
        weaponsArray[0].gameObject.SetActive(true);
        weaponsArray[1].gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        //INPUT - KEY DOWN - TAB - Swap weapons
        if (Input.GetKeyDown(KeyCode.Tab) && bCanSwapWeapons)
        {
            //Swap active state
            //primaryWeapon.gameObject.SetActive(!primaryWeapon.gameObject.activeSelf);
            //secondaryWeapon.gameObject.SetActive(!secondaryWeapon.gameObject.activeSelf);

            weaponsArray[currentWeaponIndex].gameObject.SetActive(false);

            currentWeaponIndex = (currentWeaponIndex + 1) % 2;

            weaponsArray[currentWeaponIndex].gameObject.SetActive(true);

            StartCoroutine(WeaponSwapCooldown());
        }

        //INPUT - AXIS RAW - SCROLL WHEEL UP - Next weapon
        if (Input.GetAxisRaw("Mouse ScrollWheel") > 0f && bCanSwapWeapons)
        {
            weaponsArray[currentWeaponIndex].gameObject.SetActive(false);

            currentWeaponIndex = (currentWeaponIndex + 1) % 2;

            weaponsArray[currentWeaponIndex].gameObject.SetActive(true);
        
        
            StartCoroutine(WeaponSwapCooldown());
        }

        //INPUT - AXIS RAW - SCROLL WHEEL DOWN - Previous weapon
        if (Input.GetAxisRaw("Mouse ScrollWheel") < 0f && bCanSwapWeapons)
        {
            weaponsArray[currentWeaponIndex].gameObject.SetActive(false);

            currentWeaponIndex = Mathf.Abs(currentWeaponIndex - 1) % 2;

            weaponsArray[currentWeaponIndex].gameObject.SetActive(true);
        
        
            StartCoroutine(WeaponSwapCooldown());
        }
    }

    IEnumerator WeaponSwapCooldown()
    {
        bCanSwapWeapons = false;

        yield return new WaitForSeconds(weaponSwapCooldownTime);

        bCanSwapWeapons = true;
    }
}
