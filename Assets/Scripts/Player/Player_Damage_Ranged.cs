using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Damage_Ranged : MonoBehaviour
{
    public GameObject moveableRange;
    public Transform damagePointer;
    public float damageRange = 0.5f;
    public LayerMask enemyLayer;
    public float maxRangeDistance = 5f;  
    private bool attacking;

    private Vector2 screenPosition;
    private Vector2 worldPosition;

    void Update()
    {
        screenPosition = Input.mousePosition;
        worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);

        Vector2 direction = worldPosition - (Vector2)transform.position;
        
        if (direction.magnitude > maxRangeDistance)
        {
            direction = direction.normalized * maxRangeDistance;
        }

        moveableRange.transform.position = (Vector2)transform.position + direction;

        damagePointer.position = moveableRange.transform.position;

        attacking = Input.GetButtonDown("Fire1");
        if (attacking)
        {
            Attack();
            Debug.Log("Ataque");
        }
    }

    void Attack()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(damagePointer.position, damageRange, enemyLayer);

        foreach (Collider2D enemy in hitEnemies)
        {
            enemy.GetComponent<EnemyRecievaDamage>().EnemyDamage(100);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(damagePointer.position, damageRange);
    }
}
