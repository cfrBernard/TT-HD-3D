using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour // PLACEHOLDER TODO : REFz3
{
    public static UIManager Instance { get; private set; }

    private Dictionary<string, UIPanel> panels = new();

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
    private void OnEnable()
    {
        SceneManager.OnSceneLoaded += HandleSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.OnSceneLoaded -= HandleSceneLoaded;
    }

    private void HandleSceneLoaded(string sceneName)
    {
        switch (sceneName)
        {
            case SceneNames.MainMenu:
                Debug.Log("[UIManager] MainMenu Scene loaded.");
                // UI
                break;
            case SceneNames.Game:
                Debug.Log("[UIManager] Game Scene loaded.");
                // UI
                break;
            case SceneNames.Credits:
                Debug.Log("[UIManager] Credits Scene loaded.");
                // UI
                break;
        }
    }

    // Helper to show panel: Panel prefab are pre-registered (via UIPanel).
    public void RegisterPanel(string name, UIPanel panel) => panels[name] = panel;
    public void UnregisterPanel(string name) => panels.Remove(name);

    public void ShowPanel(string name)
    {
        if (!panels.TryGetValue(name, out var panel)) { Debug.LogWarning($"Panel {name} not found."); return; }
        StartCoroutine(panel.ShowRoutine());
    }

    public void HidePanel(string name)
    {
        if (!panels.TryGetValue(name, out var panel)) { Debug.LogWarning($"Panel {name} not found."); return; }
        StartCoroutine(panel.HideRoutine());
    }
}
