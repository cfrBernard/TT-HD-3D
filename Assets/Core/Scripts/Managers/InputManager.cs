using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour // PLACEHOLDER
{
    public static InputManager Instance { get; private set; }

    private InputActionAsset inputActions;

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

        inputActions = GlobalConfigs.Input.inputActions;
    }
}

