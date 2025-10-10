using System.IO;
using UnityEngine;
using Newtonsoft.Json.Linq;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    private static string BasePath;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            BasePath = Application.persistentDataPath;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public static void SaveData(string fileName, JObject data)
    {
        string path = Path.Combine(BasePath, fileName);
        File.WriteAllText(path, data.ToString());
        Debug.Log($"[SaveManager] Saved {fileName} to {path}");
    }

    public static JObject LoadData(string fileName)
    {
        string path = Path.Combine(BasePath, fileName);

        if (!File.Exists(path))
        {
            Debug.LogWarning($"[SaveManager] No file found: {fileName}");
            return new JObject();
        }

        string json = File.ReadAllText(path);
        return JObject.Parse(json);
    }

    public static void DeleteData(string fileName)
    {
        string path = Path.Combine(BasePath, fileName);
        if (File.Exists(path))
            File.Delete(path);
    }
}
