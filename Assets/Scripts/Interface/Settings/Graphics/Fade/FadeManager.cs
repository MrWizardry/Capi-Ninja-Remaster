using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class FadeManager : MonoBehaviour
{
    public Image fadeImage;
    public float fadeSpeed = 1f;

    public IEnumerator FadeIn(System.Action onComplete = null)
    {
        fadeImage.gameObject.SetActive(true);

        float duration = 1f / fadeSpeed;
        Color c = fadeImage.color;

        for (float t = 0f; t < duration; t += Time.deltaTime)
        {
            float alpha = Mathf.Lerp(1f, 0f, t / duration);
            fadeImage.color = new Color(c.r, c.g, c.b, alpha);
            yield return null;
        }

        fadeImage.raycastTarget= false;
        onComplete?.Invoke();
    }

    public IEnumerator FadeOut(System.Action onComplete = null)
    {
        fadeImage.gameObject.SetActive(true);

        float duration = 1f / fadeSpeed;
        Color c = fadeImage.color;

        for (float t = 0f; t < duration; t += Time.deltaTime)
        {
            float alpha = Mathf.Lerp(0f, 1f, t / duration);
            fadeImage.color = new Color(c.r, c.g, c.b, alpha);
            yield return null;
        }

        onComplete?.Invoke();
    }


}
