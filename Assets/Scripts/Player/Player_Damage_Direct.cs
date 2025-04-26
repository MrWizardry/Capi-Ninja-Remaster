using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Damage_Direct : MonoBehaviour
{
    private bool attacking;
    public Transform damagePointer;

    public float damageRange = 0.5f;
    public LayerMask enemy;
    public int damage = 20;

    void Start()
    {

    }
    void Update()
    {
        attacking = Input.GetButtonDown("Fire1");
        
        if(attacking == true)
        {
            Attack();
            Debug.Log("Ataque direto");
        }

    }


    void Attack()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(damagePointer.position, damageRange, enemy);

        foreach(Collider2D enemy in hitEnemies)
        {
            enemy.GetComponent<EnemyLife>().TakeDamage(damage);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(damagePointer.position, damageRange); 
    }
}

