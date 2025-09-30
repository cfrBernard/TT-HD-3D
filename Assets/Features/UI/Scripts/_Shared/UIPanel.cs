using UnityEngine;
using System.Collections;

public class UIPanel : MonoBehaviour
{
    public string panelName;
    public CanvasGroup canvasGroup;
    public float animDuration = 0.5f;

    [Header("Slide Positions")]
    public Vector2 shownPosition = Vector2.zero;
    public Vector2 hiddenPosition = Vector2.zero;

    private RectTransform rect;

    private void Reset()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        rect = GetComponent<RectTransform>();
    }

    private void Awake()
    {
        rect = GetComponent<RectTransform>();

        UIManager.Instance.RegisterPanel(panelName, this);
    }

    public IEnumerator ShowRoutine()
    {
        gameObject.SetActive(true);
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        float t = 0f;
        while (t < animDuration)
        {
            t += Time.deltaTime;
            float normalized = t / animDuration;

            // Fade in
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, normalized);
            // Slide
            rect.anchoredPosition = Vector2.Lerp(hiddenPosition, shownPosition, normalized);

            yield return null;
        }

        canvasGroup.alpha = 1f;
        rect.anchoredPosition = shownPosition;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    public IEnumerator HideRoutine()
    {
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        float t = 0f;
        while (t < animDuration)
        {
            t += Time.deltaTime;
            float normalized = t / animDuration;

            // Fade out
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, normalized);
            // Slide
            rect.anchoredPosition = Vector2.Lerp(shownPosition, hiddenPosition, normalized);

            yield return null;
        }

        canvasGroup.alpha = 0f;
        rect.anchoredPosition = hiddenPosition;
        gameObject.SetActive(false);
    }

    // Helpers for UIManager
    public void Show(MonoBehaviour owner) => owner.StartCoroutine(ShowRoutine());
    public void Hide(MonoBehaviour owner) => owner.StartCoroutine(HideRoutine());
}
