using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;
using UnityEngine.UI;
using TMPro;

public class ProfileManager : MonoBehaviour
{
    public static ProfileManager Instance { get; private set; }

    private JObject defaultProfile;
    private JObject userProfile;
    private JObject metadataProfile;

    private const string DefaultProfilePath = "DefaultProfile";
    private const string UserProfileFileName = "UserProfile.json";
    private const string MetadataProfilePath = "MetadataProfile";

    private Dictionary<string, List<Action<JToken>>> bindings = new();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadOrInitProfile();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void LoadOrInitProfile()
    {
        TextAsset defaultAsset = Resources.Load<TextAsset>(DefaultProfilePath);
        if (defaultAsset == null) { Debug.LogError("[ProfileManager] DefaultProfile.json missing"); return; }
        defaultProfile = JObject.Parse(defaultAsset.text);

        TextAsset metadataAsset = Resources.Load<TextAsset>(MetadataProfilePath);
        if (metadataAsset != null) metadataProfile = JObject.Parse(metadataAsset.text);

        userProfile = SaveManager.LoadData(UserProfileFileName);
        if (userProfile.Count == 0)
        {
            userProfile = new JObject(defaultProfile);
            Save();
        }
    }

    #region Get/Set
    public T GetField<T>(string path)
    {
        var token = userProfile.SelectToken(path) ?? defaultProfile.SelectToken(path);
        return token != null ? token.Value<T>() : default;
    }

    public void SetField<T>(string path, T value)
    {
        var parts = path.Split('.');
        JObject current = userProfile;

        foreach (var p in parts[..^1])
        {
            if (current[p] == null) current[p] = new JObject();
            current = (JObject)current[p];
        }

        current[parts[^1]] = JToken.FromObject(value);
        Save();
    }
    #endregion

    #region Save
    public void Save()
    {
        SaveManager.SaveData(UserProfileFileName, userProfile);

        // Trigger all bindings (UI)
        foreach (var kv in bindings)
        {
            var value = userProfile.SelectToken(kv.Key);
            foreach (var callback in kv.Value)
                callback.Invoke(value);
        }
    }
    #endregion

    #region Binding helpers
    // --- Bind any JSON path to a callback ---
    public void Bind<T>(string path, Action<T> callback)
    {
        void wrapper(JToken token)
        {
            if (token != null) callback.Invoke(token.Value<T>());
        }

        if (!bindings.ContainsKey(path)) bindings[path] = new List<Action<JToken>>();
        bindings[path].Add(wrapper);

        wrapper(userProfile.SelectToken(path));
    }

    // --- Bind Text/TMP_Text/Slider/Image directly ---
    public void BindText(string path, Text uiText)
        => Bind<string>(path, value => uiText.text = value);
    
    public void BindText(string path, TMP_Text tmpText)
        => Bind<string>(path, value => tmpText.text = value);

    public void BindSlider(string path, Slider slider)
        => Bind<float>(path, value => slider.value = value);

    public void BindFill(string path, Image img)
        => Bind<float>(path, value => img.fillAmount = Mathf.Clamp01(value));
    #endregion
}
