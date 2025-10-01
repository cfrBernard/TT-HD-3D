using UnityEngine;
using UnityEngine.UI;

public class TabButton : MonoBehaviour
{
    public Image selectedBackground;
    public GameObject tabContent;

    public void SetActive(bool selected)
    {
        if (selectedBackground != null)
            selectedBackground.enabled = selected;

        if (tabContent != null)
            tabContent.SetActive(selected);
    }
}
