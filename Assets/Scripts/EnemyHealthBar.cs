using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    public Slider slide;

    public Enemy enemy;
    // Start is called before the first frame update
    void Start()
    {
        if (enemy != null)
        {
            SetMaxHealth(enemy.maxHealth);
        }
    }

    public void SetMaxHealth(int health)
    {
        slide.maxValue = health;
        slide.value = health;
    }

    public void SetHealth(int health)
    {
        slide.value = health;
    }
    // Update is called once per frame
    public void UpdateHealthBar()
    {
        if(enemy != null)
        {
            SetHealth(enemy.currentHealth);
        }
    }
}
