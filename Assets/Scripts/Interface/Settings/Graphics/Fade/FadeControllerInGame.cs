using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeControllerInGame : MonoBehaviour
{
    public FadeManager fade;
    public Scene_Chnager sceneChanger;
    public Movement movement;

    public void Awake()
    {
        movement.enabled = false;
        StartCoroutine(fade.FadeIn(() =>
        {
            movement.enabled = true;
        }));
    }

    public void ReturnToMenu()
    {
        Time.timeScale = 1f;
        movement.enabled = false;
        StartCoroutine(fade.FadeOut(() =>
        {
            sceneChanger.LoadScene(0);
        }));
    }
}
