using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TimeStop : MonoBehaviour
{
    public GameObject pauseMenu;
    bool isPaused = false;



    void Start()
    {
        isPaused = false;
        // Ensure the pause menu is not active at the start
        if (pauseMenu != null)
        {
            pauseMenu.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {

        if (Custom_Input.GetKeyDown("Pause"))
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        Debug.Log("TogglePause chamado");

        if (pauseMenu != null)
        {
            isPaused = !isPaused;
            pauseMenu.SetActive(isPaused);
            Time.timeScale = isPaused ? 0f : 1f;
        }
        else
        {
            Debug.LogWarning("pauseMenu não está atribuído!");
        }
    }

    public void ResumeGame()
    {
        if (pauseMenu != null)
        {
            isPaused = false;
            pauseMenu.SetActive(false);
            Time.timeScale = 1f; // Resume time
        }
    }
}
