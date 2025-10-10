using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AIManager : MonoBehaviour
{
    [Header("Database Reference")]
    [SerializeField] private AIIdentityDatabase aiDatabase;

    [Header("UI Bindings")]
    [SerializeField] private TMP_Text introAiNameText;
    [SerializeField] private TMP_Text gameAiNameText;
    [SerializeField] private Image AiAvatarImage;

    private string aiName;
    private string aiAvatarId;

    private void Start()
    {
        GenerateIdentity();
        UpdateUI();
    }

    private void GenerateIdentity()
    {
        if (aiDatabase == null)
        {
            Debug.LogError("[AIManager] Missing AIIdentityDatabaseSO!");
            return;
        }

        (aiName, aiAvatarId) = aiDatabase.GetRandomIdentity();

        Debug.Log($"AI Generated → Name: {aiName}, Avatar: {aiAvatarId}");
    }

    private void UpdateUI()
    {
        if (introAiNameText != null && gameAiNameText != null)
            introAiNameText.text = aiName;
            gameAiNameText.text = aiName;

        if (AiAvatarImage != null && aiDatabase.avatarDatabase != null )
        {
            var data = aiDatabase.avatarDatabase.GetAvatarById(aiAvatarId);
            if (data != null && data.avatarSprite != null)
                AiAvatarImage.sprite = data.avatarSprite;
        }
    }
}
