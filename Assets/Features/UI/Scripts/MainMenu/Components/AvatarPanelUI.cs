using UnityEngine;

public class AvatarPanelUI : MonoBehaviour
{
    [SerializeField] private AvatarDatabase avatarDatabase;
    [SerializeField] private GameObject avatarItemPrefab;
    [SerializeField] private Transform contentParent;
    [SerializeField] private ProfileMenuUI menuUI;

    private void Start()
    {
        if (avatarDatabase == null)
            avatarDatabase = Resources.Load<AvatarDatabase>("Avatars/AvatarDatabase");

        PopulateGrid();
    }

    private void PopulateGrid()
    {
        if (avatarDatabase == null || contentParent == null || avatarItemPrefab == null)
        {
            Debug.LogError("[AvatarPanelUI] Missing references.");
            return;
        }

        foreach (Transform child in contentParent)
            Destroy(child.gameObject); // Clean si déjà peuplé (PERF ???)

        foreach (var avatarData in avatarDatabase.avatars)
        {
            GameObject item = Instantiate(avatarItemPrefab, contentParent);
            AvatarItemUI itemUI = item.GetComponent<AvatarItemUI>();
            itemUI.Setup(avatarData, OnAvatarSelected);
        }
    }

    private void OnAvatarSelected(AvatarData selectedAvatar)
    {
        ProfileManager.Instance.SetField("playerData.avatarId", selectedAvatar.avatarId);
        Debug.Log($"[AvatarPanelUI] Avatar changed to {selectedAvatar.avatarId}");

        if (menuUI != null)
            menuUI.CloseAvatarPanel();
    }
}
