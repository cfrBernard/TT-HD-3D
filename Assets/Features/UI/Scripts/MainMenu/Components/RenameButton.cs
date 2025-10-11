using UnityEngine;
using TMPro;

public class RenameButton : MonoBehaviour
{
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private ProfileMenuUI menuUI;

    private void OnEnable()
    {
        inputField.text = string.Empty;
    }

    public void OnAcceptClicked()
    {
        if (inputField == null || string.IsNullOrEmpty(inputField.text))
        {
            Debug.LogWarning("[RenameButton] No name entered.");
            return;
        }

        string newName = inputField.text.Trim();
        ProfileManager.Instance.SetField("playerData.name", newName);

        Debug.Log($"[RenameButton] Player name changed to: {newName}");

        if (menuUI != null)
            menuUI.CloseRenamePanel();
    }
}
