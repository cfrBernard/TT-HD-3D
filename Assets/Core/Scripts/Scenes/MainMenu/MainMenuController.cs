using UnityEngine;
using System.Collections;

public class MainMenuController : MonoBehaviour // PLACEHOLDER TODO : REFz3
{
    public CanvasGroup fadeCanvasGroup;
    public float fadeDuration = 1f;

    private void Start()
    {
        fadeCanvasGroup.gameObject.SetActive(true);
        StartCoroutine(FadeOut());

        UIManager.Instance.ShowPanel("MainPanel");
        UIManager.Instance.ShowPanel("SidePanel");
        UIManager.Instance.ShowPanel("Header");
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
        UIManager.Instance.HidePanel("MenusPanel");
        UIManager.Instance.HidePanel("Header");
        UIManager.Instance.UnregisterPanel("MenusPanel");
        UIManager.Instance.UnregisterPanel("MainPanel");
        UIManager.Instance.UnregisterPanel("SidePanel");
        UIManager.Instance.UnregisterPanel("Header");
        yield return new WaitForSeconds(0.5f);

        fadeCanvasGroup.gameObject.SetActive(true);
        yield return FadeIn();
        yield return new WaitForSeconds(1f);

        GameManager.Instance.SetGameState(GameState.Playing);
    }

    // ================= Buttons =================
    public void OnPlayClicked()
    {
        UIManager.Instance.ShowPanel("MenusPanel");
        UIManager.Instance.HidePanel("MainPanel");
        UIManager.Instance.HidePanel("SidePanel");
        UIManager.Instance.HidePanel("Header");
    }

    public void OnReturnClicked()
    {
        UIManager.Instance.HidePanel("MenusPanel");
        UIManager.Instance.ShowPanel("MainPanel");
        UIManager.Instance.ShowPanel("SidePanel");
        UIManager.Instance.ShowPanel("Header");
    }

    public void OpenGithub()
    {
        Application.OpenURL("https://github.com/cfrBernard/TT-HD-3D");
    }

    public void PlayGame()
    {
        StartCoroutine(TransitionToLoadingScene());
    }

    public void ExitGame()
    {
        Debug.Log("EXIT");
        Application.Quit();
    }
}
