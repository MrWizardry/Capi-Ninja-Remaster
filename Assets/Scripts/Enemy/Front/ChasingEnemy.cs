using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChasingEnemy : EnemyBase
{
    public float moveSpeed = 2f;

    protected override void Update()
    {
        base.Update();
        EnemyBehavior();
    }

    protected override void EnemyBehavior()
    {
        if(isPlayerDetected)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            transform.position = Vector2.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);
        }
    }
}
