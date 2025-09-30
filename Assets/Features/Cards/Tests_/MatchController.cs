using UnityEngine;
using System.Collections;

public class MatchController : MonoBehaviour // PLACEHOLDER TODO : REFz3
{
    public CanvasGroup fadeCanvasGroup;
    public float fadeDuration = 1f;

    private void Start()
    {
        fadeCanvasGroup.gameObject.SetActive(true);
        StartCoroutine(FadeOut());
    }

    private IEnumerator FadeIn()
    {
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            fadeCanvasGroup.alpha = Mathf.Lerp(0, 1, elapsedTime / fadeDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        fadeCanvasGroup.alpha = 1;
    }

    private IEnumerator FadeOut()
    {
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            fadeCanvasGroup.alpha = Mathf.Lerp(1, 0, elapsedTime / fadeDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        fadeCanvasGroup.alpha = 0;
        fadeCanvasGroup.gameObject.SetActive(false);
    }

    private IEnumerator TransitionToLoadingScene()
    {
        yield return new WaitForSeconds(0.5f);

        fadeCanvasGroup.gameObject.SetActive(true);
        yield return FadeIn();
        yield return new WaitForSeconds(1f);

        GameManager.Instance.SetGameState(GameState.MainMenu);
    }

    // ================= Buttons =================
    public void ExitMatch()
    {
        StartCoroutine(TransitionToLoadingScene());
    }
}
