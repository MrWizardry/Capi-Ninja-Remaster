using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player_Life : MonoBehaviour
{
    private EnemyLife enemyLife; // Referência ao script EnemyLife
    public int playerdamage = 10;
    public int maxHealth = 100; // Vida máxima do jogador
    private int currentHealth; // Vida atual do jogador
    private PlayerIntagibility playerIntangibility;

    void Start()
    {
        currentHealth = maxHealth; // Inicializa a vida atual com a vida máxima]
        playerIntangibility = GetComponent<PlayerIntagibility>();
    }

    public void TakeDamage(int damage)
    {
        if (playerIntangibility != null && playerIntangibility.IsIntangible)
        {
            Debug.Log("Jogador está intangível, não levou dano!");
            return;
        }

        currentHealth -= damage; // Reduz a vida atual pelo dano recebido
        Debug.Log("Jogador tomou dano: " + damage + " | Vida atual: " + currentHealth);
        
        if (currentHealth <= 0)
        {
            Die(); // Chama o método de morte se a vida atual for menor ou igual a zero
        }
    }

    void Die()
    {
        Debug.Log("Jogador Morreu"); // Exibe mensagem de morte no console
        // Aqui você pode adicionar lógica para reiniciar o jogo ou carregar uma cena de Game Over
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // Reinicia a cena atual
    }
}
