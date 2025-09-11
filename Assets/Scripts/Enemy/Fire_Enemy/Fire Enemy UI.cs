using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireEnemyUI : MonoBehaviour
{
    [SerializeField] private GameObject ui; // UI que aparece quando entra na área
    [SerializeField] private FireEnemy fireEnemy; // referência ao fogo

    private bool playerInArea = false;

    private void Start()
    {
        if (ui != null)
            ui.SetActive(false); // começa desativado
    }

    private void Update()
    {
        if (playerInArea && Input.GetKeyDown(KeyCode.E))
        {
            if (fireEnemy != null)
            {
                fireEnemy.DisableFireParent();
                ui.SetActive(false); // esconde UI depois de desligar fogo
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && ui != null)
        {
            ui.SetActive(true);
            playerInArea = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && ui != null)
        {
            ui.SetActive(false);
            playerInArea = false;
        }
    }
}
