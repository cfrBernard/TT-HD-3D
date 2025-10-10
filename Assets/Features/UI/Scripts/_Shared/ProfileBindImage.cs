using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class ProfileBindImage : MonoBehaviour
{
    [Tooltip("JSON path in ProfileManager (e.g.: playerData.avatarId)")]
    [SerializeField] private string path = "playerData.avatarId";

    [Tooltip("REF to Avatar Database (Resources or Serialized)")]
    [SerializeField] private AvatarDatabase avatarDatabase;

    private Image image;

    private void Awake()
    {
        image = GetComponent<Image>();
    }

    private void Start()
    {
        if (ProfileManager.Instance == null) return;

        if (avatarDatabase == null)
            avatarDatabase = Resources.Load<AvatarDatabase>("Avatars/AvatarDatabase");

        ProfileManager.Instance.Bind<string>(path, UpdateImage);
    }

    private void UpdateImage(string avatarId)
    {
        if (avatarDatabase == null)
        {
            Debug.LogWarning("[ProfileBindImage] AvatarDatabase not found.");
            return;
        }

        var data = avatarDatabase.GetAvatarById(avatarId);
        if (data != null && data.avatarSprite != null)
            image.sprite = data.avatarSprite;
        else
            Debug.LogWarning($"[ProfileBindImage] Avatar '{avatarId}' not found in database.");
    }
}
