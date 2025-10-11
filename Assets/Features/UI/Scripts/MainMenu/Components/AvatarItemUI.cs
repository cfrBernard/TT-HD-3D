using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class AvatarItemUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] private Image avatarImage;
    [SerializeField] private GameObject hoverOverlay;

    private AvatarData avatarData;
    private System.Action<AvatarData> onClick;

    public void Setup(AvatarData data, System.Action<AvatarData> onClickCallback)
    {
        avatarData = data;
        onClick = onClickCallback;

        if (avatarImage != null) avatarImage.sprite = data.avatarSprite;
        if (hoverOverlay != null) hoverOverlay.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (hoverOverlay != null) hoverOverlay.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (hoverOverlay != null) hoverOverlay.SetActive(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!this || avatarImage == null)
            return;
            
        onClick?.Invoke(avatarData);
    }
    
    private void OnDestroy()
    {
        onClick = null;
    }
}
