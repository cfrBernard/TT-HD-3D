using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;
using UnityEngine.UI;
using TMPro;

public class ProfileManager : MonoBehaviour
{
    public static ProfileManager Instance { get; private set; }

    public JObject MetadataProfile => metadataProfile;

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

    public void AddToField(string path, float amount)
    {
        var token = userProfile.SelectToken(path);
        float current = 0;

        if (token != null && token.Type == JTokenType.Integer)
            current = token.Value<int>();
        else if (token != null && token.Type == JTokenType.Float)
            current = token.Value<float>();

        float newValue = current + amount;
        SetField(path, newValue);
    }

    // AddXp Placeholder
    public void AddXp(int amount)
    {
        int currentXp = GetField<int>("playerData.xp");
        int currentLevel = GetField<int>("playerData.level");

        int newXp = currentXp + amount;
        int newLevel = currentLevel;

        // Récup steps depuis metadata
        var steps = metadataProfile["levels"]?["steps"] as JArray;
        if (steps == null)
        {
            Debug.LogWarning("[ProfileManager] No XP steps found in metadata.");
            return;
        }

        // Trouver le niveau en fonction de l'Xp
        foreach (var step in steps)
        {
            int level = step["level"].Value<int>();
            int xpRequired = step["xpRequired"].Value<int>();

            if (newXp >= xpRequired)
                newLevel = level;
            else
                break;
        }

        // Met à jour le profil
        SetField("playerData.xp", newXp);

        // Si on a monté de niveau
        if (newLevel > currentLevel)
        {
            SetField("playerData.level", newLevel);
            ApplyLevelRewards(newLevel);

            // Publier un event pour l'UI
            GameEventBus.Publish(new PlayerXpChangedEvent(newXp, newLevel));
        }
        else
        {
            // Toujours notifier si l'Xp change
            GameEventBus.Publish(new PlayerXpChangedEvent(newXp, currentLevel));
        }
    }

    private void ApplyLevelRewards(int level)
    {
        var rewardsArray = metadataProfile["levels"]?["Rewards"] as JArray;
        if (rewardsArray == null) return;

        foreach (var entry in rewardsArray)
        {
            if (entry["level"].Value<int>() != level) continue;

            var rewards = entry["reward"] as JArray;
            foreach (var reward in rewards)
            {
                if (reward["coins"] != null)
                    AddToField("playerData.coins", reward["coins"].Value<int>());

                if (reward["pack"] != null)
                {
                    // e.g. placeholder : +1 pack
                    Debug.Log($"[ProfileManager] Player received {reward["pack"]} pack(s) for level {level}");
                }
            }
        }
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
            var dead = new List<Action<JToken>>();

            foreach (var callback in kv.Value)
            {
                try
                {
                    callback?.Invoke(value);
                }
                catch (MissingReferenceException)
                {
                    // Callback cible un objet Unity détruit -> on le supprime
                    dead.Add(callback);
                }
                catch (Exception e)
                {
                    Debug.LogWarning($"[ProfileManager] Bind callback error ({kv.Key}): {e.Message}");
                }
            }

            // Cleaning death callbacks
            foreach (var d in dead)
                kv.Value.Remove(d);
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

    // --- Bind Slider XP (auto updates with playerData.xp / level) ---
    public void BindSliderXp(Slider slider)
    {
        void UpdateXpUI()
        {
            int xp = GetField<int>("playerData.xp");
            int level = GetField<int>("playerData.level");

            var steps = metadataProfile["levels"]?["steps"] as JArray;
            if (steps == null || steps.Count == 0) return;

            int prevXp = 0;
            int nextXp = 0;

            for (int i = 0; i < steps.Count; i++)
            {
                if (steps[i]["level"].Value<int>() == level)
                {
                    prevXp = steps[i]["xpRequired"].Value<int>();
                    nextXp = (i + 1 < steps.Count) ? steps[i + 1]["xpRequired"].Value<int>() : prevXp;
                    break;
                }
            }

            slider.minValue = prevXp;
            slider.maxValue = nextXp;
            slider.value = xp;

        }

        Bind<int>("playerData.xp", _ => UpdateXpUI());
        Bind<int>("playerData.level", _ => UpdateXpUI());
        UpdateXpUI();
    }
}
