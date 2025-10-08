using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Linq;

public class CreditsController : MonoBehaviour
{
    [SerializeField] private Canvas canvas;
    [SerializeField] private CanvasGroup skipButton;
    [SerializeField] private float fadeDuration = 0.5f;
    [SerializeField] private float mouseIdleTime = 2f;

    private float lastMouseMoveTime;
    private bool cursorHidden;

    private void Start()
    {
        StartCoroutine(FadeOut());
        Application.runInBackground = true;
        lastMouseMoveTime = Time.unscaledTime;
        Cursor.visible = false;
        cursorHidden = true;
        StartCoroutine(PlayCredits());
    }

    private void Update()
    {
        if (Mouse.current != null && Mouse.current.delta.ReadValue() != Vector2.zero)
        {
            lastMouseMoveTime = Time.unscaledTime;

            if (skipButton.alpha < 1f)
            {
                StopCoroutine("FadeSkipButton");
                StartCoroutine(FadeSkipButton(1f));
            }

            if (cursorHidden)
            {
                Cursor.visible = true;
                cursorHidden = false;
            }
        }
        else if (Time.unscaledTime - lastMouseMoveTime > mouseIdleTime)
        {
            if (skipButton.alpha > 0f)
            {
                StopCoroutine("FadeSkipButton");
                StartCoroutine(FadeSkipButton(0f));
            }

            if (!cursorHidden)
            {
                Cursor.visible = false;
                cursorHidden = true;
            }
        }
    }

    private IEnumerator FadeSkipButton(float targetAlpha)
    {
        float startAlpha = skipButton.alpha;
        float time = 0f;

        while (time < fadeDuration)
        {
            time += Time.unscaledDeltaTime;
            skipButton.alpha = Mathf.Lerp(startAlpha, targetAlpha, time / fadeDuration);
            yield return null;
        }

        skipButton.alpha = targetAlpha;
    }

    private IEnumerator PlayCredits()
    {
        var credits = canvas.GetComponentsInChildren<CreditsComponent>(true)
                            .OrderBy(c => c.index)
                            .ToArray();

        foreach (var credit in credits)
        {
            CanvasGroup group = credit.GetComponent<CanvasGroup>();
            if (group == null)
                group = credit.gameObject.AddComponent<CanvasGroup>();

            group.alpha = 0f;
            group.gameObject.SetActive(true);

            // Fade in
            yield return StartCoroutine(FadeCanvasGroup(group, 0f, 1f, fadeDuration));

            // Temps affiché
            yield return new WaitForSecondsRealtime(credit.Duration);

            // Fade out
            yield return StartCoroutine(FadeCanvasGroup(group, 1f, 0f, fadeDuration));

            group.gameObject.SetActive(false);
        }

        yield return new WaitForSecondsRealtime(5f);
        ExitCredits();
    }

    private IEnumerator FadeCanvasGroup(CanvasGroup group, float start, float end, float duration)
    {
        float time = 0f;
        while (time < duration)
        {
            time += Time.unscaledDeltaTime;
            group.alpha = Mathf.Lerp(start, end, time / duration);
            yield return null;
        }
        group.alpha = end;
    }

    // ================= Enter/Exit Transition =================
    public CanvasGroup fadeCanvasGroup;

    private IEnumerator FadeIn()
    {
        fadeCanvasGroup.gameObject.SetActive(true);

        float elapsedTime = 0f;
        while (elapsedTime < 1f)
        {
            fadeCanvasGroup.alpha = Mathf.Lerp(0, 1, elapsedTime / 1f);
            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }
        fadeCanvasGroup.alpha = 1;
    }

    private IEnumerator FadeOut()
    {
        float elapsedTime = 0f;
        while (elapsedTime < 1f)
        {
            fadeCanvasGroup.alpha = Mathf.Lerp(1, 0, elapsedTime / 1f);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        fadeCanvasGroup.alpha = 0;
        fadeCanvasGroup.gameObject.SetActive(false);
    }

    private IEnumerator TransitionToLoadingScene()
    {
        yield return new WaitForSecondsRealtime(0.5f);

        fadeCanvasGroup.gameObject.SetActive(true);
        yield return FadeIn();
        yield return new WaitForSecondsRealtime(1f);

        Application.runInBackground = false;
        GameManager.Instance.SetGameState(GameState.MainMenu);
    }

    public void ExitCredits()
    {
        StartCoroutine(TransitionToLoadingScene());
    }
}
