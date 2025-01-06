using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WatchingEnemy : EnemyBase
{
    protected override void Update()
    {
        base.Update(); // Mantém a detecção de jogador
        EnemyBehavior();
    }

    protected override void EnemyBehavior()
    {
        if (isPlayerDetected)
        {
            // Faz o inimigo olhar para o jogador
            Vector3 direction = player.position - transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
        else
        {
            // Reseta a rotação quando o jogador não está visível (opcional)
            transform.rotation = Quaternion.identity;
        }
    }
}
