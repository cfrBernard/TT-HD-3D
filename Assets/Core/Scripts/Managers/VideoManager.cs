using UnityEngine;

public class VideoManager : MonoBehaviour
{
    public static VideoManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        ApplyVideoSettings();
    }

    public void ApplyVideoSettings()
    {
        string resolution = SettingsManager.Instance.GetSetting<string>("video", "resolution");
        bool fullscreen = SettingsManager.Instance.GetSetting<bool>("video", "fullscreen");
        string framerateStr = SettingsManager.Instance.GetSetting<string>("video", "targetFramerate");
        bool vsync = SettingsManager.Instance.GetSetting<bool>("video", "vsync");

        // --- Resolution ---
        string[] resParts = resolution.Split('x');
        if (resParts.Length == 2 &&
            int.TryParse(resParts[0], out int width) &&
            int.TryParse(resParts[1], out int height))
        {
            Screen.SetResolution(width, height, fullscreen);
        }
        else
        {
            Debug.LogWarning("Résolution invalide : " + resolution);
        }

        // --- Framerate ---
        if (int.TryParse(framerateStr.Replace("FPS", ""), out int targetFPS))
        {
            Application.targetFrameRate = targetFPS;
        }
        else
        {
            Debug.LogWarning("Framerate invalide : " + framerateStr);
        }

        // --- VSync ---
        QualitySettings.vSyncCount = vsync ? 1 : 0;
    }
}
