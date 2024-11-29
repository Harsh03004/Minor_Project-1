using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider slider;
    public Image fill;

    // Reference to the Playerhealth script to access current health
    public Playerhealth playerHealth;

    void Start()
    {
        if (playerHealth != null)
        {
            SetMaxHealth(playerHealth.maxHealth);
        }
    }

    // Set the max health of the player and update the health bar
    public void SetMaxHealth(int health)
    {
        slider.maxValue = health;
        slider.value = health;

    }

    // Update the health bar based on the player's current health
    public void SetHealth(int health)
    {
        slider.value = health;

    }

    // Method to call from Playerhealth to update the health bar
    public void UpdateHealthBar()
    {
        if (playerHealth != null)
        {
            SetHealth(playerHealth.currentHealth);
        }
    }
}
