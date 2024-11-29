using System.Collections;
using UnityEngine;

public class Attack : MonoBehaviour
{
    public float attackRange = 0.5f;
    public int attackDamage1 = 20;
    public int attackDamage2 = 30;
    public float attackCooldown1 = 0.5f;
    public float attackCooldown2 = 1f;
    public Transform attackPoint;
    public LayerMask enemyLayers;

    private float nextAttackTime1 = 0f;
    private float nextAttackTime2 = 0f;

    void Update()
    {
        // Check if the attack cooldown has elapsed before allowing another attack
        if (Time.time >= nextAttackTime1 && Input.GetMouseButtonDown(0)) // Left click
        {
            Attackk(1);
            nextAttackTime1 = Time.time + attackCooldown1;
        }

        if (Time.time >= nextAttackTime2 && Input.GetMouseButtonDown(1)) // Right click
        {
            Attackk(2);
            nextAttackTime2 = Time.time + attackCooldown2;
        }
    }

    void Attackk(int attackType)
    {
        // Detect enemies in range of the attack
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        // Apply damage to each enemy detected
        foreach (Collider2D enemy in hitEnemies)
        {
            int damage = (attackType == 1) ? attackDamage1 : attackDamage2;
            Enemy enemyScript = enemy.GetComponent<Enemy>();

            if (enemyScript != null)
            {
                enemyScript.TakeDamage(damage);
                Debug.Log("Enemy hit with " + damage + " damage from attack " + attackType);
            }
            else
            {
                Debug.LogWarning("Enemy object missing Enemy script!");
            }
        }
    }

    // Visualize the attack range in the editor
    void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
