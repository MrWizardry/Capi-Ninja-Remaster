using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRecievaDamage : MonoBehaviour
{
    private int actualLife = 100;

    public void EnemyDamage(int damage)
    {
        actualLife -= damage;

        if(actualLife <= 0)
        {
            Destroy(this.gameObject);
        }
    }
}
