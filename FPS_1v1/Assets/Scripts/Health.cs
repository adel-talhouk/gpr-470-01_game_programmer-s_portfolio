using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    //Public/Serialized data
    [SerializeField] int maxHealth;
    public Slider healthSlider;

    //Helper data
    int currentHealth;

    // Start is called before the first frame update
    void Start()
    {
        //Setup health and UI
        currentHealth = maxHealth;
        healthSlider.maxValue = maxHealth;
        healthSlider.value = currentHealth;
    }

    public void TakeDamage(int damageValue)
    {
        //Take damage
        currentHealth -= damageValue;

        //Check death
        if (currentHealth <= 0)
        {
            Die();
        }

        //Update slider
        healthSlider.value = currentHealth;
    }

    void Die()
    {
        //TO-DO: Implement


        //Update slider
        healthSlider.value = currentHealth;
    }
}
