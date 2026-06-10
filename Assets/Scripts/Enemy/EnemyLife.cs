using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EnemyLife : MonoBehaviour
{
    public int maxHealth = 100; // Vida máxima do inimigo
    private int currentHealth; // Vida atual do inimigo

    [SerializeField] private GameObject vfxDeath;

    private Vector3 SpawnPosition;
    void Start()
    {
        currentHealth = maxHealth;
        SpawnPosition = transform.position;
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
        //Instantiate(vfxDeath, transform.position, Quaternion.identity);
        //Destroy(this.gameObject);
        //Debug.Log("Inimigo Morreu");

        if(vfxDeath != null)
        {
            Instantiate(vfxDeath, transform.position, Quaternion.identity);
        }

        Respawn();
    }

    void Respawn()
    {
        currentHealth = maxHealth; // Restaura a vida do inimigo
        transform.position = SpawnPosition; 
        Debug.Log("Inimigo Respawnou");
    }

}
