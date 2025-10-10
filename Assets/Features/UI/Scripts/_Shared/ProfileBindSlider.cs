using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class ProfileBindSlider : MonoBehaviour
{
    [Tooltip("JSON path in ProfileManager (ex: playerData.xp)")]
    [SerializeField] private string path;

    private Slider slider;

    private void Awake()
    {
        slider = GetComponent<Slider>();
    }

    private void Start()
    {
        if (ProfileManager.Instance == null || string.IsNullOrEmpty(path)) return;

        ProfileManager.Instance.Bind<float>(path, value => slider.value = value);
    }
}
