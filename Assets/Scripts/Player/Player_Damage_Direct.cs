using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Player_Damage_Direct : MonoBehaviour
{
    private bool attacking;
    public Transform damagePointer;

    public float damageRange = 0.5f;
    //public LayerMask enemy;
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
        Collider2D[] hitObjects = Physics2D.OverlapCircleAll(damagePointer.position, damageRange);

        foreach(Collider2D obj in hitObjects)
        {
            if(obj.CompareTag("enemy") || obj.CompareTag("boss"))
            {
                Controller_Jaré jare = obj.GetComponent<Controller_Jaré>();
                if(jare != null)
                {
                    jare.TakeDamage(damage);
                    Debug.Log("Dano causado: " + damage);
                }
                EnemyLife enemy = obj.GetComponent<EnemyLife>();
                if(enemy != null)
                {
                    enemy.TakeDamage(damage);
                    Debug.Log("Dano causado: " + damage);
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(damagePointer.position, damageRange); 
    }
}

