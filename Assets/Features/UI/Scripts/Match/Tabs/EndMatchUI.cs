using TMPro;
using UnityEngine;

public class EndMatchUI : MonoBehaviour
{
    [SerializeField] private TMP_Text resultText;
    [SerializeField] private TMP_Text rewardsText;

    private void OnEnable()
    {
        GameEventBus.Subscribe<MatchEndEvent>(OnMatchEnd);
    }

    private void OnDisable()
    {
        GameEventBus.Unsubscribe<MatchEndEvent>(OnMatchEnd);
    }

    private void OnMatchEnd(MatchEndEvent e)
    {
        resultText.text = e.Result switch
        {
            "win" => "VICTORY!",
            "draw" => "DRAW",
            _ => "DEFEAT"
        };

        rewardsText.text = $"+{e.Coins} Coins | +{e.Xp} XP";
    }
}
