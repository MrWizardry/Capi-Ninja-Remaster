using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Through_Plataform : MonoBehaviour
{
    public Dash dash;
    public Collider2D col; // Colisor
    public float timerToReset = 0.2f; // Tempo que o player pode passar por baixo da plataforma
    public GameObject palyer;

    private void Start()
    {
        col = GetComponent<Collider2D>(); // Pega o colisor da plataforma
    }

    private void Update()
    {
        if(dash.isDashing == true || Input.GetKeyDown(KeyCode.S)) // Se o player estiver dashing
        {
            col.IsTouching(palyer.GetComponent<Collider2D>()); // Checa se o player está tocando a plataforma
            col.enabled = false; // Desabilita o colisor da plataforma
            StartCoroutine(ResetTrigger()); // Inicia a coroutine pra resetar o trigger
        }
    }

    private IEnumerator ResetTrigger()
    {
        yield return new WaitForSeconds(timerToReset); // Espera o tempo definido
        col.enabled = true; // Habilita o colisor da plataforma
    }


}
