using UnityEngine;
using System.Collections;

public class MainMenuController : MonoBehaviour // PLACEHOLDER TODO : REFz3
{
    [SerializeField] private TabManager tabManager;

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

    // =====================================================
    // === INTERNAL
    // =====================================================
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

    private IEnumerator TransitionToCredits()
    {
        UIManager.Instance.HidePanel("SidePanel");
        UIManager.Instance.HidePanel("MainPanel");
        UIManager.Instance.HidePanel("Header");
        UIManager.Instance.UnregisterPanel("MenusPanel");
        UIManager.Instance.UnregisterPanel("MainPanel");
        UIManager.Instance.UnregisterPanel("SidePanel");
        UIManager.Instance.UnregisterPanel("Header");
        yield return new WaitForSeconds(0.5f);

        fadeCanvasGroup.gameObject.SetActive(true);
        yield return FadeIn();
        yield return new WaitForSeconds(1f);

        GameManager.Instance.SetGameState(GameState.Credits);
    }

    // =====================================================
    // === BUTTONS
    // =====================================================
    public void ShowPlayMenu()
    {
        UIManager.Instance.ShowPanel("MenusPanel");
        UIManager.Instance.HidePanel("MainPanel");
        UIManager.Instance.HidePanel("SidePanel");
        UIManager.Instance.HidePanel("Header");

        tabManager.OpenWithTab(0);
    }

    public void ShowStoreMenu()
    {
        UIManager.Instance.ShowPanel("MenusPanel");
        UIManager.Instance.HidePanel("MainPanel");
        UIManager.Instance.HidePanel("SidePanel");
        UIManager.Instance.HidePanel("Header");

        tabManager.OpenWithTab(1);
    }

    public void ShowCollectionMenu()
    {
        UIManager.Instance.ShowPanel("MenusPanel");
        UIManager.Instance.HidePanel("MainPanel");
        UIManager.Instance.HidePanel("SidePanel");
        UIManager.Instance.HidePanel("Header");

        tabManager.OpenWithTab(2);
    }

    public void ShowMissionsMenu()
    {
        UIManager.Instance.ShowPanel("MenusPanel");
        UIManager.Instance.HidePanel("MainPanel");
        UIManager.Instance.HidePanel("SidePanel");
        UIManager.Instance.HidePanel("Header");

        tabManager.OpenWithTab(3);
    }

    public void ShowStatsMenu()
    {
        UIManager.Instance.ShowPanel("MenusPanel");
        UIManager.Instance.HidePanel("MainPanel");
        UIManager.Instance.HidePanel("SidePanel");
        UIManager.Instance.HidePanel("Header");

        tabManager.OpenWithTab(4);
    }

    public void ShowSettingsMenu()
    {
        UIManager.Instance.ShowPanel("MenusPanel");
        UIManager.Instance.HidePanel("MainPanel");
        UIManager.Instance.HidePanel("SidePanel");
        UIManager.Instance.HidePanel("Header");

        tabManager.OpenWithTab(5);
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

    public void PlayCredits()
    {
        StartCoroutine(TransitionToCredits());
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
