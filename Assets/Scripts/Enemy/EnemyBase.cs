using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyBase : MonoBehaviour
{
    public Transform player; // Referência ao jogador
    public float detectionRange = 5f; // Distância máxima de detecção
    public LayerMask playerLayer; // Camada para o jogador

    protected bool isPlayerDetected = false; // Indica se o jogador foi detectado

    protected virtual void Update()
    {
        DetectPlayer();
    }

    // Método para detectar o jogador usando Raycast
    protected void DetectPlayer()
    {
        Vector2 directionToPlayer = player.position - transform.position;
        float distanceToPlayer = directionToPlayer.magnitude;

        if (distanceToPlayer <= detectionRange)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, directionToPlayer.normalized, detectionRange, playerLayer);
            isPlayerDetected = hit.collider != null && hit.collider.CompareTag("Player");
        }
        else
        {
            isPlayerDetected = false;
        }
    }

    // Método abstrato para comportamento específico
    protected abstract void EnemyBehavior();

    private void OnDrawGizmos()
    {
        // Visualiza o alcance no editor
        Gizmos.color = isPlayerDetected ? Color.green : Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}