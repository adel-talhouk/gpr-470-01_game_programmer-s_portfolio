using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//THIS CLASS WILL ALLOW THE PLAYER TO SWAP BETWEEN AND PICK UP WEAPONS AND AMMO

public class WeaponManager : MonoBehaviour
{
    BaseWeapon primaryWeapon;
    BaseWeapon secondaryWeapon;

    // Start is called before the first frame update
    void Start()
    {
        //Set weapons
        primaryWeapon = transform.GetChild(0).GetComponent<BaseWeapon>();
        secondaryWeapon = transform.GetChild(1).GetComponent<BaseWeapon>();

        //Set primary weapon as active by default
        primaryWeapon.gameObject.SetActive(true);
        secondaryWeapon.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        //INPUT - KEY DOWN - TAB - Swap weapons
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            //Swap active state
            primaryWeapon.gameObject.SetActive(!primaryWeapon.gameObject.activeSelf);
            secondaryWeapon.gameObject.SetActive(!secondaryWeapon.gameObject.activeSelf);
        }
    }
}
