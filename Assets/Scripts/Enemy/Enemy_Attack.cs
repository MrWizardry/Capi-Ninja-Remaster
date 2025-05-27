using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Attack : MonoBehaviour
{

    #region Variables
    public Transform player;
    public float attackRange = 1f; // Distância do ataque
    public float attackCD = 1f;
    public int damage = 10; // Dano fixo de 10, você pode modificar isso para ser variável

    private bool isAttacking = false;
    private EnemyBase enemyBase; // Referência ao EnemyBase 
    #endregion

    private void Start()
    {
        enemyBase = GetComponent<EnemyBase>(); // Obtém a referência ao EnemyBase
    }

    void Update()
    {
        if(player == null || enemyBase == null) return; // Verifica se player ou enemyBase são nulos

        float distance = Vector2.Distance(transform.position, player.position); // Distância entre o inimigo e o jogador

        if(!isAttacking && distance <= attackRange) // Se não está atacando e o jogador está dentro do alcance de ataque
        {
            isAttacking = true; // Define que o inimigo está atacando
            enemyBase.StopMovement(); // Para o movimento do inimigo (método a ser implementado no EnemyBase)

            // Aqui você pode adicionar a lógica de ataque, como animações ou danos ao jogador
            enemyBase.PlayAttackAnim();
        }
    }

    IEnumerator Attack()
    {

        Player_Life player_Life = player.GetComponent<Player_Life>(); // Obtém a referência ao Player_Life
        if(player_Life != null) // Verifica se o Player_Life não é nulo
        {
            player_Life.TakeDamage(damage); // Aplica dano ao jogador (dano fixo de 10, você pode modificar isso para ser variável)
        }

        yield return new WaitForSeconds(attackCD); // Espera o cooldown do ataque

        enemyBase.ResumeMovement(); // Retoma o movimento do inimigo (método a ser implementado no EnemyBase)
        isAttacking = false; // Permite que o inimigo ataque novamente
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, attackRange); // Desenha uma esfera para visualizar o alcance do ataque   
    }

}
