using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class ParamValuePicker : MonoBehaviour
{
    [SerializeField] private TMP_Text valueLabel;
    [SerializeField] private Button prevButton;
    [SerializeField] private Button nextButton;

    private string[] options;
    private int currentIndex;

    public event Action<int> OnValueChanged;

    public void Setup(string[] options, int startIndex, Action<int> callback)
    {
        this.options = options ?? Array.Empty<string>();
        currentIndex = Mathf.Clamp(startIndex, 0, options.Length - 1);
        OnValueChanged = callback;

        UpdateLabel();

        prevButton.onClick.AddListener(() => ChangeValue(-1));
        nextButton.onClick.AddListener(() => ChangeValue(1));
    }

    private void ChangeValue(int delta)
    {
        if (options.Length == 0) return;
        currentIndex = (currentIndex + delta + options.Length) % options.Length;
        UpdateLabel();
        OnValueChanged?.Invoke(currentIndex);
    }

    private void UpdateLabel()
    {
        valueLabel.text = options.Length > 0 ? options[currentIndex] : "-";
    }
}
