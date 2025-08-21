using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Defense : MonoBehaviour
{
    private bool isDefending = false;
    //public float defenseDuration = 1f;
    private float defenseTimer = 0f;
    public GameObject defenceBarrier;
    private Movement playerMovement;
    private Dash dashScript;
    private WallSlide wsScript;

    void Start()
    {
        playerMovement = GetComponent<Movement>();
        dashScript = GetComponent<Dash>();
        wsScript = GetComponent<WallSlide>();
    }

    void Update()
    {
        if(wsScript.isWallSliding == true || wsScript.isTouchingWall == true)
        {
            StopDefense();           
        }
        else
        {
            // Exemplo: apertando botão "Fire2" (botão direito do mouse por padrão)
            if (Input.GetButtonDown("Fire2"))
            {
                StartDefense();
            }

            /*if (isDefending)
            {
                defenseTimer -= Time.deltaTime;
                if (defenseTimer <= 0f)
                {
                    StopDefense();
                }
            }*/

            else if(Input.GetButtonUp("Fire2"))
            {
                StopDefense();
            }
        }
    }

    void StartDefense()
    {
        isDefending = true;
        defenceBarrier.SetActive(true);
        //defenseTimer = defenseDuration;
        // Aqui você pode ativar animação de defesa, mudar cor, etc
        Debug.Log("Defendendo!");
        playerMovement.enabled = false; // Desabilita o movimento do jogador enquanto defende
        dashScript.enabled = false; // Desabilita o dash enquanto defende
    }

    void StopDefense()
    {
        isDefending = false;
        defenceBarrier.SetActive(false);
        // Aqui você pode voltar à animação normal, cor normal, etc
        Debug.Log("Parou de defender");
        playerMovement.enabled = true;
        dashScript.enabled = true; // Reabilita o dash quando para de defender
    }
}
