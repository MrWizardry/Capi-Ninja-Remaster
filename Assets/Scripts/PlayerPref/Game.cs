using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Game : MonoBehaviour
{
    void Awake()
    {
        Time.timeScale = 1f;
    }
    public void ToMenu()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

        // Salva o índice da cena atual como "SavedScene"
        PlayerPrefs.SetInt("SavedScene", currentSceneIndex);
        PlayerPrefs.Save();

        SceneManager.LoadScene(0); // volta pro menu
    }
}
