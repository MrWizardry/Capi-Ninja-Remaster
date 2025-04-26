using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDamageSystemV2 : MonoBehaviour
{
    private bool attacking;
    public Transform damagePointer;

    public float damageRange = 0.5f;
    public LayerMask enemyLayer;

    AnimationManager animationManager;
    void Start()
    {
        animationManager = GetComponent<AnimationManager>();
    }
    void Update()
    {
        attacking = Input.GetButtonDown("Fire2");
        
        if(attacking == true)
        {
            StartCoroutine(AttackAction());
            Debug.Log("Ataque");
        }

    }


    IEnumerator AttackAction()
    {
        attacking = false;
        animationManager.PlayActionAnimation("Attack");
        yield return new WaitForSeconds(0.25f);
        AttackCheck();
        attacking = true;

    }
    void AttackCheck()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(damagePointer.position, damageRange, enemyLayer);

        foreach (Collider2D enemies in hitEnemies)
        {
            enemies.GetComponent<EnemyRecievaDamage>().EnemyDamage(100);
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(damagePointer.position, damageRange); 
    }
}

