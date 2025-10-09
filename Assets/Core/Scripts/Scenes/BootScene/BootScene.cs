using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BootScene : MonoBehaviour
{
    [Header("Singletons")]
    private GameManager gameManager;
    private SceneManager sceneManager;
    private SaveManager saveManager; 
    private SettingsManager settingsManager;
    private ProfileManager profileManager;
    private InputManager inputManager;
    private UIManager _UIManager;
    private EventManager eventManager;
    private AudioManager audioManager;
    private VideoManager videoManager;
    private ToolsManager toolsManager;
    private FPSDisplay _FPSDisplay;

    #region BootSequence
    private void Start()
    {
        StartCoroutine(BootSequence());
    }

    private IEnumerator BootSequence()
    {
        yield return StartCoroutine(FadeOut());
        yield return new WaitForSeconds(1f);

        InitSingletons();
        UpdateProgressBar();
        // yield return StartCoroutine(LoadPlayerPrefs());
        // UpdateProgressBar();
        // yield return StartCoroutine(LoadStaticData());
        // UpdateProgressBar();
        // yield return StartCoroutine(InitBackendServices());
        // UpdateProgressBar();
        // yield return StartCoroutine(LoadUserProfile());
        // UpdateProgressBar();
        // yield return StartCoroutine(PrepareAssets());
        
        yield return new WaitForSeconds(1f);
        UpdateProgressBar();
        
        yield return StartCoroutine(FadeIn());
        yield return StartCoroutine(LoadMainMenuAsync());
    }

    private void InitSingletons()
    {
        if (GameManager.Instance == null)
        {
            gameManager = new GameObject("GameManager").AddComponent<GameManager>();
            DontDestroyOnLoad(gameManager.gameObject);
        }
        else
            gameManager = GameManager.Instance;
    
        if (SceneManager.Instance == null)
        {
            sceneManager = new GameObject("SceneManager").AddComponent<SceneManager>();
            DontDestroyOnLoad(sceneManager.gameObject);
        }
        else
            sceneManager = SceneManager.Instance;

        if (SaveManager.Instance == null)
        {
            saveManager = new GameObject("SaveManager").AddComponent<SaveManager>();
            DontDestroyOnLoad(saveManager.gameObject);
        }
        else
            saveManager = SaveManager.Instance;

        if (SettingsManager.Instance == null)
        {
            settingsManager = new GameObject("SettingsManager").AddComponent<SettingsManager>();
            DontDestroyOnLoad(settingsManager.gameObject);
        }
        else
            settingsManager = SettingsManager.Instance;
            
        if (ProfileManager.Instance == null)
        {
            profileManager = new GameObject("ProfileManager").AddComponent<ProfileManager>();
            DontDestroyOnLoad(profileManager.gameObject);
        }
        else
            profileManager = ProfileManager.Instance;
    
        if (InputManager.Instance == null)
        {
            inputManager = new GameObject("InputManager").AddComponent<InputManager>();
            DontDestroyOnLoad(inputManager.gameObject);
        }
        else
            inputManager = InputManager.Instance;

        if (UIManager.Instance == null)
        {
            _UIManager = new GameObject("UIManager").AddComponent<UIManager>();
            DontDestroyOnLoad(_UIManager.gameObject);
        }
        else
            _UIManager = UIManager.Instance;

        if (EventManager.Instance == null)
        {
            eventManager = new GameObject("EventManager").AddComponent<EventManager>();
            DontDestroyOnLoad(eventManager.gameObject);
        }
        else
            eventManager = EventManager.Instance;

        if (AudioManager.Instance == null)
        {
            audioManager = new GameObject("AudioManager").AddComponent<AudioManager>();
            DontDestroyOnLoad(audioManager.gameObject);
        }
        else
            audioManager = AudioManager.Instance;

        if (VideoManager.Instance == null)
        {
            videoManager = new GameObject("VideoManager").AddComponent<VideoManager>();
            DontDestroyOnLoad(videoManager.gameObject);
        }
        else
            videoManager = VideoManager.Instance;

        if (ToolsManager.Instance == null)
        { 
            toolsManager = new GameObject("toolsManager").AddComponent<ToolsManager>();
            DontDestroyOnLoad(toolsManager.gameObject);
        }
        else 
            toolsManager = ToolsManager.Instance;

        if (FPSDisplay.Instance == null)
        {
            _FPSDisplay = new GameObject("_FPSDisplay").AddComponent<FPSDisplay>();
            DontDestroyOnLoad(_FPSDisplay.gameObject);
        }
        else
            _FPSDisplay = FPSDisplay.Instance;
    }

    // IEnumerator LoadPlayerPrefs() {}
    // IEnumerator LoadStaticData() {}
    // IEnumerator InitBackendServices() {}
    // IEnumerator LoadUserProfile() {}
    // IEnumerator PrepareAssets() {}
    
    private IEnumerator LoadMainMenuAsync()
    {
        yield return null;
        GameManager.Instance.SetGameState(GameState.MainMenu);
    }
    #endregion

    #region UI
    [Header("UI Elements")]
    public Image progressBar;
    public CanvasGroup fadeCanvasGroup;
    public float fadeDuration = 1f;

    // Progress Bar
    private float totalSteps = 2f;
    private float currentStep = 0f;
    private float progressSpeed = 0.3f;

    private IEnumerator FadeIn()
    {
        fadeCanvasGroup.gameObject.SetActive(true);
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            fadeCanvasGroup.alpha = Mathf.Lerp(0, 1, elapsedTime / fadeDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        fadeCanvasGroup.alpha = 1;
    }

    private IEnumerator FadeOut()
    {
        fadeCanvasGroup.gameObject.SetActive(true);
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            fadeCanvasGroup.alpha = Mathf.Lerp(1, 0, elapsedTime / fadeDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        fadeCanvasGroup.alpha = 0;
        fadeCanvasGroup.gameObject.SetActive(false);
    }

    private void UpdateProgressBar()
    {
        currentStep += 1f;
        StartCoroutine(SmoothProgressBarUpdate(progressBar.fillAmount, currentStep / totalSteps));
    }
    
    private IEnumerator SmoothProgressBarUpdate(float startValue, float targetValue)
    {
        float elapsedTime = 0f;
        while (elapsedTime < progressSpeed)
        {
            elapsedTime += Time.deltaTime;
            progressBar.fillAmount = Mathf.Lerp(startValue, targetValue, elapsedTime / progressSpeed);
            yield return null;
        }
        progressBar.fillAmount = targetValue;
    }
    #endregion
}
