using UnityEngine;

public class ToolsManager : MonoBehaviour // PLACEHOLDER
{
    public static ToolsManager Instance { get; private set; }

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

    // abstract all the tools ? 
}
