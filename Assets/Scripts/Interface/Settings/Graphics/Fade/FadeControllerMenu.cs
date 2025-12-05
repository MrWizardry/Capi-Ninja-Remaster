using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeControllerMenu : MonoBehaviour
{
    public FadeManager fade;
    public NewGame_System newGame;

    public void NewGame()
    {
        StartCoroutine(fade.FadeOut(() =>
        {
            newGame.StartNewGame();
        }));
    }

    public void ContinueGame()
    {
        StartCoroutine(fade.FadeOut(() =>
        {
            newGame.ContinueGame();
        }));
    }
}
