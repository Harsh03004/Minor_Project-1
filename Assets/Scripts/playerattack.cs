using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerattack : MonoBehaviour
{
    // Start is called before the first frame update
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
        if (Time.time >= nextAttackTime1)
        {
            if (Input.GetMouseButtonDown(0)) // Left click
            {
                Attack(1);
                nextAttackTime1 = Time.time + attackCooldown1;
            }
        }

        if (Time.time >= nextAttackTime2)
        {
            if (Input.GetMouseButtonDown(1)) // Right click
            {
                Attack(2);
                nextAttackTime2 = Time.time + attackCooldown2;
            }
        }
    }

    void Attack(int attackType)
    {
        // Play attack animation
        // animator.SetTrigger(attackType == 1 ? "Attack1" : "Attack2");

        // Detect enemies in range of attack
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        // Damage them
        foreach (Collider2D enemy in hitEnemies)
        {
            int damage = (attackType == 1) ? attackDamage1 : attackDamage2;
            enemy.GetComponent<Enemy>().TakeDamage(damage);
        }

        // You can add different visual or sound effects for each attack type here
    }

    // Visualize the attack range in the editor
    void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;

        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
