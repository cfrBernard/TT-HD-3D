using UnityEngine;
using Newtonsoft.Json.Linq;
using UnityEngine.UI;
using TMPro;

public class PlayMenuUI : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Transform contentPanel;

    [Header("Prefabs")]
    [SerializeField] private GameObject toggleParamPrefab;

    [Header("Info Panel TMP")]
    [SerializeField] private TMP_Text infoLabelTMP;
    [SerializeField] private TMP_Text infoDescriptionTMP;

    private bool initialized = false;
    
    private void OnEnable()
    {
        if (!initialized)
        {
            GenerateUI("rules");
            initialized = true;
        }
    }

    private void GenerateUI(string category)
    {
        JObject metadata = Resources.Load<TextAsset>("MetadataSettings") is TextAsset metaAsset
            ? JObject.Parse(metaAsset.text)?[category] as JObject
            : null;

        if (metadata == null)
        {
            Debug.LogError($"[RuleSettingsUI] Metadata not found or invalid for category '{category}'.");
            return;
        }

        JObject defaults = Resources.Load<TextAsset>("DefaultSettings") is TextAsset defaultAsset
            ? JObject.Parse(defaultAsset.text)?[category] as JObject
            : null;

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
                    bool currentValue = SettingsManager.Instance.GetSetting<bool>(category, key);
                    CreateToggleItem(category, key, label, description, currentValue);
                    break;

                default:
                    Debug.LogWarning($"[RuleSettingsUI] Unknown type '{type}' for rule '{key}'.");
                    break;
            }
        }
    }

    private void CreateToggleItem(string category, string key, string label, string description, bool currentValue)
    {
        var go = Instantiate(toggleParamPrefab, contentPanel);
        var itemUI = go.GetComponent<SettingItemUI>();
        var toggle = go.GetComponentInChildren<Toggle>();
        var labelTMP = go.GetComponentInChildren<TMP_Text>();

        if (itemUI == null || toggle == null || labelTMP == null)
        {
            Debug.LogError($"[RuleSettingsUI] Missing components on prefab '{toggleParamPrefab.name}'.");
            return;
        }

        // Label/Value Setup 
        labelTMP.text = label;
        toggle.isOn = currentValue;

        // Info panel hover hook
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

        // Toggle hook -> save
        toggle.onValueChanged.AddListener(value =>
        {
            SettingsManager.Instance.SetOverride(category, key, value);
            ApplyAndSave();
        });
    }

    private void ApplyAndSave()
    {
        SettingsManager.Instance.Save();
        SettingsManager.Instance.ApplySettings();
    }
}
