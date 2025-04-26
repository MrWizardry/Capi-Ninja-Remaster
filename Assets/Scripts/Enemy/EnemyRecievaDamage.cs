using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRecievaDamage : MonoBehaviour
{
    [SerializeField] private GameObject vfxDeath;
    private int actualLife = 100;

    public void EnemyDamage(int damage)
    {
        actualLife -= damage;

        if(actualLife <= 0)
        {
            Instantiate(vfxDeath, transform.position, Quaternion.identity);
            Destroy(this.gameObject);
        }
    }
}
