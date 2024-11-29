using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class playerattack : MonoBehaviour
{
    // Public variables for attack range, damage, and cooldowns
    public float attackRange = 0.5f;
    public int attackDamage1 = 20;
    public int attackDamage2 = 30;
    public int specialattack = 40; // Damage for special attack

    public float attackCooldown1 = 0.5f;
    public float attackCooldown2 = 1f;
    public float specialattackcooldown = 2f; // Cooldown for special attack

    public float Experience = 0; // Player's total experience
    public int skillPoints = 0; // Player's total skill points

    public Transform attackPoint; // Position of the attack origin
    public LayerMask enemyLayers; // Layers that represent enemies

    // Private variables to track the next attack time for each type
    private float nextAttackTime1 = 0f;
    private float nextAttackTime2 = 0f;
    private float nextAttackTime3 = 0f;

    public SkillButton doubleAttackButton;
    public SkillButton UltimateAttackButton;  // Reference to Ultimate Attack SkillButton

    [SerializeField]
    public TextMeshProUGUI XP;

    [SerializeField]
    public TextMeshProUGUI SkillPoints;



    void Update()
    {
        // Left-click attack (mouse button 0)
        if (Time.time >= nextAttackTime1)
        {
            if (Input.GetMouseButtonDown(0)) // Left click
            {
                Attack(1); // Regular attack 1
                nextAttackTime1 = Time.time + attackCooldown1; // Set next attack time
            }
        }

        // Right-click attack (mouse button 1)
        if (Time.time >= nextAttackTime2)
        {
            if (Input.GetMouseButtonDown(1)) // Right click
            {
                if(doubleAttackButton.IsUnlocked)
                {
                    Attack(2); // Regular attack 2
                    nextAttackTime2 = Time.time + attackCooldown2; // Set next attack time
                }
            }
        }

        // Special attack (keyboard key X)
        if (Time.time >= nextAttackTime3)
        {
            if (Input.GetKeyDown(KeyCode.X)) // X key for special attack
            {
                if (UltimateAttackButton.IsUnlocked)
                {
                    Attack(3); // Special attack
                    nextAttackTime3 = Time.time + specialattackcooldown; // Set special attack cooldown
                }  
            }
        }
    }

    void Attack(int attackType)
    {
        // Detect enemies in range of the attack using a circle check
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        // Iterate over all detected enemies and apply damage
        foreach (Collider2D enemy in hitEnemies)
        {
            int damage = 0;
            if (attackType == 1) // First regular attack (left click)
            {

                damage = attackDamage1;
            }
            else if (attackType == 2) // Second regular attack (right click)
            {
                damage = attackDamage2;
            }
            else if (attackType == 3) // Special attack (X key)
            {
                damage = specialattack;
            }

            // Apply the damage to the enemy
            enemy.GetComponent<Enemy>().TakeDamage(damage);
        }
    }

    // Visualize the attack range in the Unity editor for debugging
    void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;

        Gizmos.DrawWireSphere(attackPoint.position, attackRange); // Draw a sphere representing the attack range
    }

    public void AddExperience(int amount)
    {
        Experience += amount;
        XP.text = Experience.ToString();  // Convert Experience to a string
        Debug.Log($"Player gained {amount} experience. Total experience: {Experience}");

        int newSkillPoints = Mathf.FloorToInt(Experience / 10);

        if (newSkillPoints > skillPoints)
        {
            int pointsGained = newSkillPoints - skillPoints;
            skillPoints = newSkillPoints;
            SkillPoints.text = skillPoints.ToString();
            Debug.Log($"Player gained {pointsGained} skill point(s). Total skill points: {skillPoints}");

            // Ensure EconomyManager reflects the updated skill points
            if (EconomyManager.instance != null)
            {
                EconomyManager.instance.UpdateXPText(skillPoints);
            }
        }
    }

    public void ModifyExperience(float amount)
    {
        Experience += amount;

        // Update UI
        XP.text = Experience.ToString();

        // Calculate and update skill points
        int newSkillPoints = Mathf.FloorToInt(Experience / 10);

        if (newSkillPoints > skillPoints)
        {
            int pointsGained = newSkillPoints - skillPoints;
            skillPoints = newSkillPoints;

            //Update UI
            SkillPoints.text = skillPoints.ToString();

            Debug.Log($"Player gained {pointsGained} skill point(s). Total skill points: {skillPoints}");

            if (EconomyManager.instance != null)
            {
                EconomyManager.instance.UpdateXPText(skillPoints);
            }
        }
        Debug.Log($"Player's experience updated by {amount}. Total experience: {Experience}");
    }
}

