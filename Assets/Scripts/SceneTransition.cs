using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class SceneTransition : MonoBehaviour
{
    public CanvasGroup fadeCanvas;
    public float fadeDuration = 1f;

    private void Start()
    {
        StartCoroutine(FadeIn());
    }

    public void FadeToScene(string sceneName)
    {
        StartCoroutine(FadeOutAndLoad(sceneName));
    }

    private IEnumerator FadeIn()
    {
        float t = fadeDuration;
        while (t > 0)
        {
            t -= Time.deltaTime;
            fadeCanvas.alpha = t / fadeDuration;
            yield return null;
        }
        fadeCanvas.alpha = 0;
    }

    private IEnumerator FadeOutAndLoad(string sceneName)
    {
        float t = 0;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            fadeCanvas.alpha = t / fadeDuration;
            yield return null;
        }

        SceneManager.LoadScene(sceneName);
    }
}
