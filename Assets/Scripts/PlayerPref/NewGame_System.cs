using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NewGame_System : MonoBehaviour
{
    public GameObject continueButton;
    private int sceneToContinue;

    void Start()
    {
        // Se não há cena salva, desativa o botão
        if (PlayerPrefs.GetInt("SavedScene", 0) == 0)
        {
            continueButton.SetActive(false);
        }
        else
        {
            continueButton.SetActive(true);
        }
    }

    public void StartNewGame()
    {
        PlayerPrefs.DeleteKey("cp_x");
        PlayerPrefs.DeleteKey("cp_y");
        PlayerPrefs.DeleteKey("cp_z");
        PlayerPrefs.DeleteKey("SavedScene");
        SceneManager.LoadScene(1); // primeira cena do jogo
    }

    public void ContinueGame()
    {
        sceneToContinue = PlayerPrefs.GetInt("SavedScene", 0);
        if (sceneToContinue != 0)
        {
            SceneManager.LoadScene(sceneToContinue);
        }
    }

    public void DeleteKeys()
    {
        PlayerPrefs.DeleteAll(); // Deleta todas as chaves salvas
        continueButton.SetActive(false); // Desativa o botão de continuar
        Debug.Log("Todas as chaves foram deletadas.");
    }
    
}
