using UnityEngine;

public class ProfileMenuUI : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject renamePanel;
    [SerializeField] private GameObject avatarPanel;

    private void OnEnable()
    {
        if (renamePanel != null) renamePanel.SetActive(false);
        if (avatarPanel != null) avatarPanel.SetActive(false);
    }

    // Called by Button "Change Name"
    public void OnChangeNameClicked()
    {
        if (renamePanel != null)
            renamePanel.SetActive(true);
            avatarPanel.SetActive(false);
    }

    // Called by Button "Change Avatar"
    public void OnChangeAvatarClicked()
    {
        if (avatarPanel != null)
            avatarPanel.SetActive(true);
            renamePanel.SetActive(false);
    }

    // Called by buttons inside panels
    public void CloseRenamePanel()
    {
        if (renamePanel != null)
            renamePanel.SetActive(false);
    }

    public void CloseAvatarPanel()
    {
        if (avatarPanel != null)
            avatarPanel.SetActive(false);
    }
}
