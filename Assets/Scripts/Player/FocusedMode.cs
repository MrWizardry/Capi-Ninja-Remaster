using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FocusedMode : MonoBehaviour
{   
    //Referencias dos códigos para fazer as ações necessarias
    public Player_Damage_Ranged player_Damage_ranged;
    public PlayerMovement1 playerMovement;
    public Player_Damage_Direct player_Damage_Direct;
    //Nesse KeyCode você consegue alterar qual a letra que será utilizada 
    //para ativar o modo
    [SerializeField] private KeyCode focused;
    private bool isFocused = false;


    void Update()
    {
        //Aqui ele simplesmente ativa quando necessario e desativa
        if(Input.GetKeyDown(focused))
        {
            playerMovement.enabled = false;
            player_Damage_ranged.enabled = true;
            player_Damage_Direct.enabled = false;
            isFocused = true;
            Debug.Log("Focused");
        }
        else if(Input.GetKeyUp(focused))
        {
            playerMovement.enabled = true;
            player_Damage_ranged.enabled = false;
            player_Damage_Direct.enabled = true;
            isFocused = false;
            Debug.Log("Unfocused");
        }
    }

}
