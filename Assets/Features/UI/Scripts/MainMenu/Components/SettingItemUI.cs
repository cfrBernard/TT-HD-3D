using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using System;

[RequireComponent(typeof(CanvasGroup))]
public class SettingItemUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("Refs")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TMP_Text label;
    [SerializeField] private Selectable control; // Toggle only

    [Header("Style")]
    [Range(0, 1)] public float normalAlpha = 0.6f;
    [Range(0, 1)] public float hoverAlpha = 1f;

    public Action OnHoverEnter;
    public Action OnHoverExit;

    void Awake()
    {
        if (!canvasGroup) canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = normalAlpha;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        canvasGroup.alpha = hoverAlpha;
        OnHoverEnter?.Invoke();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        canvasGroup.alpha = normalAlpha;
        OnHoverExit?.Invoke();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // GO Click = toggle
        if (control is Toggle toggle && !eventData.used)
        {
            toggle.isOn = !toggle.isOn;
        }
    }
}
