using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Attack : MonoBehaviour
{

    #region Variables
    public Transform player;
    public float attackRange = 1f; // Distância do ataque
    public float attackCD = 1f;

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
            StartCoroutine(Attack()); // Inicia o ataque
        }
    }

    IEnumerator Attack()
    {
        isAttacking = true; // Define que o inimigo está atacando
        enemyBase.StopMovement(); // Para o movimento do inimigo (método a ser implementado no EnemyBase)

        // Aqui você pode adicionar a lógica de ataque, como animações ou danos ao jogador
        Debug.Log("Ataque!");

        yield return new WaitForSeconds(attackCD); // Espera o cooldown do ataque

        enemyBase.ResumeMovement(); // Retoma o movimento do inimigo (método a ser implementado no EnemyBase)
        isAttacking = false; // Permite que o inimigo ataque novamente
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, attackRange); // Desenha uma esfera para visualizar o alcance do ataque   
    }

}
