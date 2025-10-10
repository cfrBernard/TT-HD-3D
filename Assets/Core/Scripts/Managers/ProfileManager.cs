using System.IO;
using UnityEngine;
using Newtonsoft.Json.Linq;

public class ProfileManager : MonoBehaviour
{
    public static ProfileManager Instance { get; private set; }

    private JObject defaultProfile;
    private JObject userProfile;
    private JObject metadataProfile;

    private const string DefaultProfilePath = "DefaultProfile";
    private const string UserProfileFileName = "UserProfile.json";
    private const string MetadataProfilePath = "MetadataProfile";

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
        // Load default profile from Resources
        TextAsset defaultAsset = Resources.Load<TextAsset>(DefaultProfilePath);
        if (defaultAsset == null)
        {
            Debug.LogError("[ProfileManager] DefaultProfile.json not found in Resources/");
            return;
        }
        defaultProfile = JObject.Parse(defaultAsset.text);

        // Load metadata profile
        TextAsset metadataAsset = Resources.Load<TextAsset>(MetadataProfilePath);
        if (metadataAsset == null)
        {
            Debug.LogError("[ProfileManager] MetadataProfile.json not found in Resources/");
            return;
        }
        metadataProfile = JObject.Parse(metadataAsset.text);

        // Load/init user profile
        userProfile = SaveManager.LoadData(UserProfileFileName);
        if (userProfile.Count == 0)
        {
            Debug.Log("[ProfileManager] No user profile found, creating default copy.");
            userProfile = new JObject(defaultProfile); // copy default
            Save();
        }

        Debug.Log("[ProfileManager] Profile loaded.");
    }

    public void Save()
    {
        SaveManager.SaveData(UserProfileFileName, userProfile);
    }

    public void ResetProfile()
    {
        SaveManager.DeleteData(UserProfileFileName);
        LoadOrInitProfile();
    }

    #region Get/Set
    public JObject GetProfile() => userProfile;
    public JObject GetMetadata() => metadataProfile;

    public T GetField<T>(string path)
    {
        var token = userProfile.SelectToken(path);
        if (token != null)
            return token.Value<T>();

        var defaultToken = defaultProfile.SelectToken(path);
        if (defaultToken != null)
            return defaultToken.Value<T>();

        Debug.LogWarning($"[ProfileManager] Field not found: {path}");
        return default;
    }

    public void SetField<T>(string path, T value)
    {
        var parts = path.Split('.');
        JObject current = userProfile;

        foreach (var p in parts[..^1])
        {
            if (current[p] == null)
                current[p] = new JObject();
            current = (JObject)current[p];
        }

        current[parts[^1]] = JToken.FromObject(value);
    }
    #endregion
}
