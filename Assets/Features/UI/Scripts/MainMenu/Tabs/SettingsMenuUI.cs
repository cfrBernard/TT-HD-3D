using UnityEngine;
using Newtonsoft.Json.Linq;
using UnityEngine.UI;
using System;
using TMPro;

public class SettingsMenuUI : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Transform contentPanel;

    [Header("Prefabs")]
    [SerializeField] private GameObject settingItemTogglePrefab;
    [SerializeField] private GameObject settingItemSliderPrefab;
    [SerializeField] private GameObject settingItemValuePickerPrefab;
    [SerializeField] private GameObject separatorPrefab; // TESTING

    [Header("Info Panel TMP")]
    [SerializeField] private TMP_Text infoLabelTMP;
    [SerializeField] private TMP_Text infoDescriptionTMP;

    private bool initialized = false;

    private void Start()
    {
        if (!initialized)
        {
            GenerateUI("video");
            // Instantiate(separatorPrefab, contentPanel); // TESTING
            GenerateUI("audio");
            initialized = true;
        }
    }

    #region UI Generation
    private void GenerateUI(string category)
    {
        JObject metadata = Resources.Load<TextAsset>("MetadataSettings") is TextAsset metaAsset
            ? JObject.Parse(metaAsset.text)?[category] as JObject
            : null;

        JObject defaults = Resources.Load<TextAsset>("DefaultSettings") is TextAsset defaultAsset
            ? JObject.Parse(defaultAsset.text)?[category] as JObject
            : null;

        if (metadata == null)
        {
            Debug.LogError($"[SettingsMenuUI] Metadata not found or invalid for category '{category}'.");
            return;
        }

        foreach (var pair in metadata)
        {
            string key = pair.Key;
            JObject param = pair.Value as JObject;
            if (param == null) continue;

            string type = param["type"]?.ToString();
            string label = param["label"]?.ToString() ?? key;
            string description = param["description"]?.ToString() ?? "";

            switch (type)
            {
                case "toggle":
                    bool toggleValue = SettingsManager.Instance.GetSetting<bool>(category, key);
                    CreateToggleItem(category, key, label, description, toggleValue);
                    break;

                case "slider":
                    float sliderValue = SettingsManager.Instance.GetSetting<float>(category, key);
                    float min = param["min"]?.Value<float>() ?? 0;
                    float max = param["max"]?.Value<float>() ?? 1;
                    CreateSliderItem(category, key, label, description, sliderValue, min, max);
                    break;

                case "dropdown":
                    string[] options = param["options"]?.ToObject<string[]>() ?? Array.Empty<string>();
                    string currentValue = SettingsManager.Instance.GetSetting<string>(category, key);
                    int index = Array.IndexOf(options, currentValue);
                    CreateValuePickerItem(category, key, label, description, options, index);
                    break;

                default:
                    Debug.LogWarning($"[SettingsMenuUI] Unknown type '{type}' for '{key}'.");
                    break;
            }
        }
    }
    #endregion

    #region Create Items
    private void CreateToggleItem(string category, string key, string label, string description, bool currentValue)
    {
        var go = Instantiate(settingItemTogglePrefab, contentPanel);
        var itemUI = go.GetComponent<SettingItemUI>();
        var toggle = go.GetComponentInChildren<Toggle>();
        var labelTMP = go.GetComponentInChildren<TMP_Text>();

        labelTMP.text = label;
        toggle.isOn = currentValue;

        itemUI.OnHoverEnter += () =>
        {
            if (infoLabelTMP) infoLabelTMP.text = label;
            if (infoDescriptionTMP) infoDescriptionTMP.text = description;
        };
        itemUI.OnHoverExit += () =>
        {
            if (infoLabelTMP) infoLabelTMP.text = "";
            if (infoDescriptionTMP) infoDescriptionTMP.text = "";
        };

        toggle.onValueChanged.AddListener(value =>
        {
            SettingsManager.Instance.SetOverride(category, key, value);
            ApplyAndSave();
        });
    }

    private void CreateSliderItem(string category, string key, string label, string description, float value, float min, float max)
    {
        var go = Instantiate(settingItemSliderPrefab, contentPanel);
        var itemUI = go.GetComponent<SettingItemUI>();
        var slider = go.GetComponentInChildren<Slider>();
        var labelTMP = go.GetComponentInChildren<TMP_Text>();

        labelTMP.text = label;
        slider.minValue = min;
        slider.maxValue = max;
        slider.value = value;

        itemUI.OnHoverEnter += () =>
        {
            if (infoLabelTMP) infoLabelTMP.text = label;
            if (infoDescriptionTMP) infoDescriptionTMP.text = description;
        };
        itemUI.OnHoverExit += () =>
        {
            if (infoLabelTMP) infoLabelTMP.text = "";
            if (infoDescriptionTMP) infoDescriptionTMP.text = "";
        };

        slider.onValueChanged.AddListener(v =>
        {
            SettingsManager.Instance.SetOverride(category, key, v);
            ApplyAndSave();
        });
    }

    private void CreateValuePickerItem(string category, string key, string label, string description, string[] options, int startIndex)
    {
        var go = Instantiate(settingItemValuePickerPrefab, contentPanel);
        var itemUI = go.GetComponent<SettingItemUI>();
        var picker = go.GetComponentInChildren<ParamValuePicker>();
        var labelTMP = go.GetComponentInChildren<TMP_Text>();

        labelTMP.text = label;

        itemUI.OnHoverEnter += () =>
        {
            if (infoLabelTMP) infoLabelTMP.text = label;
            if (infoDescriptionTMP) infoDescriptionTMP.text = description;
        };
        itemUI.OnHoverExit += () =>
        {
            if (infoLabelTMP) infoLabelTMP.text = "";
            if (infoDescriptionTMP) infoDescriptionTMP.text = "";
        };

        picker.Setup(options, Mathf.Max(startIndex, 0), index =>
        {
            string selected = options[index];
            SettingsManager.Instance.SetOverride(category, key, selected);
            ApplyAndSave();
        });
    }
    #endregion

    #region Save / Apply
    private void ApplyAndSave()
    {
        SettingsManager.Instance.Save();
        SettingsManager.Instance.ApplySettings();
    }
    #endregion
}
