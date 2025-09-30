using System;
using UnityEngine;

public enum GameState // PLACEHOLDER TODO : REFz3
{
    Boot,
    MainMenu,
    Playing,
    Paused
}

public class GameManager : MonoBehaviour // PLACEHOLDER TODO : REFz3
{
    public static GameManager Instance { get; private set; }
    public GameState CurrentState { get; private set; }

    public static event Action<GameState> OnGameStateChanged;

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

    public void SetGameState(GameState newState)
    {
        if (newState == CurrentState) return;

        CurrentState = newState;
        Debug.Log($"[GameManager] GameState changed to {newState}");

        switch (newState)
        {
            case GameState.Boot:
                // SceneManager.Instance.LoadScene(SceneNames.Boot);
                break;
            case GameState.MainMenu:
                SceneManager.Instance.LoadScene(SceneNames.MainMenu);
                break;
            case GameState.Playing:
                SceneManager.Instance.LoadSceneWithLoading(SceneNames.Game);
                break;
        }

        OnGameStateChanged?.Invoke(newState);
    }
}
