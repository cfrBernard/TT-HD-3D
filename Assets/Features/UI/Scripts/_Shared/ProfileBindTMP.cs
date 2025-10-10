using UnityEngine;
using TMPro;

[RequireComponent(typeof(TMP_Text))]
public class ProfileBindTMP : MonoBehaviour
{
    [Tooltip("JSON path in ProfileManager (e.g.: playerData.name)")]
    [SerializeField] private string path = "playerData.name";

    private TMP_Text textComponent;

    private void Awake()
    {
        textComponent = GetComponent<TMP_Text>();
    }

    private void Start()
    {
        if (ProfileManager.Instance == null)
        {
            Debug.LogWarning("[ProfileBindTMP] ProfileManager not initialized yet.");
            return;
        }

        if (string.IsNullOrEmpty(path))
        {
            Debug.LogWarning($"[ProfileBindTMP] No JSON path set on {gameObject.name}");
            return;
        }

        ProfileManager.Instance.BindText(path, textComponent);
    }
}
