using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerXpUI : MonoBehaviour
{
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private Slider xpSlider;

    private void OnEnable() => GameEventBus.Subscribe<PlayerXpChangedEvent>(OnXPChanged);
    private void OnDisable() => GameEventBus.Unsubscribe<PlayerXpChangedEvent>(OnXPChanged);

    private void OnXPChanged(PlayerXpChangedEvent e)
    {
        levelText.text = $"Level {e.NewLevel}";
        xpSlider.value = e.NewXp; // normaliser avec le max XP du niveau ?
    }
}
