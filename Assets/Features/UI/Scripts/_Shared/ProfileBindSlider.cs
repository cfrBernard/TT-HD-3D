using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class ProfileBindSlider : MonoBehaviour
{
    [Tooltip("JSON path in ProfileManager (ex: playerData.health)")]
    [SerializeField] private string path;

    [Tooltip("Enable XP mode (binds to XP/Level automatically)")]
    [SerializeField] private bool xpMode = false;

    private Slider slider;

    private void Awake()
    {
        slider = GetComponent<Slider>();
    }

    private void Start()
    {
        if (ProfileManager.Instance == null)
            return;

        if (xpMode)
            ProfileManager.Instance.BindSliderXp(slider);
        else if (!string.IsNullOrEmpty(path))
            ProfileManager.Instance.BindSlider(path, slider);
    }
}
