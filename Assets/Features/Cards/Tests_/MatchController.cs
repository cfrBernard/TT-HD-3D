using UnityEngine;
using System.Collections;
using SM = UnityEngine.SceneManagement.SceneManager;

public class MatchController : MonoBehaviour
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

    // =====================================================
    // === INTERNAL
    // =====================================================
    private IEnumerator TransitionToMenu()
    {
        UIManager.Instance.UnregisterPanel("GameMenu");
        UIManager.Instance.UnregisterPanel("PlayerUI_2");
        UIManager.Instance.UnregisterPanel("PlayerUI_1");
        UIManager.Instance.UnregisterPanel("BackG");
        UIManager.Instance.UnregisterPanel("MenuButton");
        UIManager.Instance.UnregisterPanel("TurnStart");
        UIManager.Instance.UnregisterPanel("MulliganPhase");
        UIManager.Instance.UnregisterPanel("GameIntro");
        UIManager.Instance.UnregisterPanel("MulliganButton");
        yield return new WaitForSeconds(0.5f);

        fadeCanvasGroup.gameObject.SetActive(true);
        yield return FadeIn();
        yield return new WaitForSeconds(1f);

        GameManager.Instance.SetGameState(GameState.MainMenu);
    }

    private IEnumerator RestartTransition()
    {
        UIManager.Instance.UnregisterPanel("GameMenu");
        UIManager.Instance.UnregisterPanel("PlayerUI_2");
        UIManager.Instance.UnregisterPanel("PlayerUI_1");
        UIManager.Instance.UnregisterPanel("BackG");
        UIManager.Instance.UnregisterPanel("MenuButton");
        UIManager.Instance.UnregisterPanel("TurnStart");
        UIManager.Instance.UnregisterPanel("MulliganPhase");
        UIManager.Instance.UnregisterPanel("GameIntro");
        UIManager.Instance.UnregisterPanel("MulliganButton");
        yield return new WaitForSeconds(0.5f);

        fadeCanvasGroup.gameObject.SetActive(true);
        yield return FadeIn();
        yield return new WaitForSeconds(1f);

        // SceneReload
        SM.LoadScene(SM.GetActiveScene().buildIndex);
    }

    // =====================================================
    // === BUTTONS
    // =====================================================
    public void ExitMatch()
    {
        StartCoroutine(TransitionToMenu());
    }

    public void RestartGame()
    {
        StartCoroutine(RestartTransition());
    }

    public void OpenGameMenu()
    {
        UIManager.Instance.ShowPanel("GameMenu");
        UIManager.Instance.ShowPanel("BackG");
        UIManager.Instance.HidePanel("MenuButton");
        UIManager.Instance.HidePanel("PlayerUI_1");
        UIManager.Instance.HidePanel("PlayerUI_2");
    }

    public void ResumeGame()
    {
        UIManager.Instance.HidePanel("GameMenu");
        UIManager.Instance.HidePanel("BackG");
        UIManager.Instance.ShowPanel("MenuButton");
        UIManager.Instance.ShowPanel("PlayerUI_1");
        UIManager.Instance.ShowPanel("PlayerUI_2");
    }
}
