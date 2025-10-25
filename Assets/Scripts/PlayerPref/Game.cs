using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Game : MonoBehaviour
{
    public static Game Instance { get; private set; }

    private bool canReceiveInputs;
    public bool CanReceiveInputs => canReceiveInputs;
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Optional: if you want the manager to persist across scenes
        }

        Time.timeScale = 1f;
        canReceiveInputs = false;
    }
    public void ToMenu()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

        // Salva o índice da cena atual como "SavedScene"
        PlayerPrefs.SetInt("SavedScene", currentSceneIndex);
        PlayerPrefs.Save();

        SceneManager.LoadScene(0); // volta pro menu
    }
    public bool isReceivingInputs()
    {
        return canReceiveInputs;
    }
    public void CanReceiveInputsNow()
    {
        canReceiveInputs = true;
    }
    public void CanNotReceiveInputsNow()
    {
        canReceiveInputs = false;
    }
}
