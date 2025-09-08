using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Attack : MonoBehaviour
{

    public Transform player;
    public float attackRange = 1f; 
    public float attackCD = 1f;
    public int damage = 10; 

    private bool isAttacking = false;
    private EnemyBase enemyBase; 
    

    private void Start()
    {
        enemyBase = GetComponent<EnemyBase>(); // Obtém a referência ao EnemyBase
    }

    void Update()
    {
        if(player == null || enemyBase == null) return; // Verifica se player ou enemyBase são nulos

        float distance = Vector2.Distance(transform.position, player.position);

        if (!isAttacking && distance <= attackRange)
        {
            isAttacking = true;
            StartCoroutine(Attack());
            enemyBase.PlayAttackAnim();
        }
    }

    IEnumerator Attack()
    {

        // Para o movimento e toca animação
        enemyBase.StopMovement();
    
        // Espera o tempo da animação ou um delay pequeno
        yield return new WaitForSeconds(0.3f);

        // Checa a distância antes de aplicar dano
        if(player != null && Vector2.Distance(transform.position, player.position) <= attackRange)
        {
            Player_Life player_Life = player.GetComponent<Player_Life>();
            if(player_Life != null)
                player_Life.TakeDamage(damage);
        }

        // Espera o cooldown antes de permitir novo ataque
        yield return new WaitForSeconds(attackCD);

        enemyBase.ResumeMovement();
        isAttacking = false;
        }

    void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, attackRange); // Desenha uma esfera para visualizar o alcance do ataque   
    }

}
