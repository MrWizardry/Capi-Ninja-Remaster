using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EnemyLife : MonoBehaviour
{
    public int maxHealth = 100; // Vida máxima do inimigo
    private int currentHealth; // Vida atual do inimigo
    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage ; // Dano fixo de 10, você pode modificar isso para ser variável
        Debug.Log("inimigo tomou dano: " + damage + " | Vida atual: " + currentHealth);
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Inimigo Morreu");
        Destroy(gameObject); // Destroi o objeto inimigo
    }
}
