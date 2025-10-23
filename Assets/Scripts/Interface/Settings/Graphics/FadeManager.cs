using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class FadeManager : MonoBehaviour
{
    [Header("Escolha o Tipo de Fade")]
    public FadeType fadeType;

    [Header("Configuração Geral")]
    public Image fadeImage;
    public float fadeSpeed = 1f;

    [Header("Referências")]
    public Scene_Chnager scene_Chnager;
    public NewGame_System newGameSystem;
    public Movement playerMovement; // 👈 adiciona referência ao script de movimento

    public enum FadeType
    {
        InGame,
        NewGame,
        Continue
    }

    void Awake()
    {
        StartCoroutine(FadeIn());
        playerMovement.enabled = false; 
    }

    public void StartFadeOut()
    {
        StartCoroutine(FadeOutByType());
        Time.timeScale = 1f;
    }

    public void FadeToNew()
    {
        fadeType = FadeType.NewGame;
        StartFadeOut();
    }

    public void FadeToContinue()
    {
        fadeType = FadeType.Continue;
        StartFadeOut();
    }

    #region Fade In
    private IEnumerator FadeIn()
    {
        fadeImage.gameObject.SetActive(true);
        float duration = 1f / fadeSpeed;
        float elapsed = 0f;
        Color color = fadeImage.color;

        for (elapsed = 0f; elapsed < duration; elapsed += Time.deltaTime)
        {
            float alpha = Mathf.Lerp(1f, 0f, elapsed / duration);
            fadeImage.color = new Color(color.r, color.g, color.b, alpha);
            yield return null;
        }

        fadeImage.color = new Color(color.r, color.g, color.b, 0f);
        fadeImage.gameObject.SetActive(false);

        // ✅ Libera a movimentação do player após o fade
        if (playerMovement != null)
        {
            playerMovement.enabled = true;
        }
    }
    #endregion

    #region FadeOut By Type
    private IEnumerator FadeOutByType()
    {
        fadeImage.gameObject.SetActive(true);
        float duration = 1f / fadeSpeed;
        float elapsed = 0f;
        Color color = fadeImage.color;

        for (elapsed = 0f; elapsed < duration; elapsed += Time.deltaTime)
        {
            float alpha = Mathf.Lerp(0f, 1f, elapsed / duration);
            fadeImage.color = new Color(color.r, color.g, color.b, alpha);
            yield return null;
        }

        fadeImage.color = new Color(color.r, color.g, color.b, 1f);

        switch (fadeType)
        {
            case FadeType.InGame:
                if (scene_Chnager != null)
                    scene_Chnager.LoadScene(0);
                else
                    Debug.LogWarning("Scene_Chnager não foi atribuído!");
                break;

            case FadeType.NewGame:
                if (newGameSystem != null)
                    newGameSystem.StartNewGame();
                else
                    Debug.LogWarning("NewGame_System não foi atribuído!");
                break;

            case FadeType.Continue:
                if (newGameSystem != null)
                    newGameSystem.ContinueGame();
                else
                    Debug.LogWarning("NewGame_System não foi atribuído!");
                break;
        }
    }
    #endregion
}
