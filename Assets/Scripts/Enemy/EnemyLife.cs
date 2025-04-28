using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EnemyLife : MonoBehaviour
{
    public int maxHealth = 100;
    private int currentHealth;
    public bool isDead = false; // Nova variável para saber se o inimigo morreu

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        Debug.Log("inimigo tomou dano: " + damage + " | Vida atual: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        if (isDead) return;

        isDead = true;
        Debug.Log("Inimigo Morreu");
        Destroy(gameObject);
    }

    public bool IsDead()
    {
        return isDead;
    }
}
