using TMPro;
using UnityEngine;
// RENAME CA PLZ
public class PlayerStatusUI : MonoBehaviour
{
    [SerializeField] private TMP_Text statusText;
    [SerializeField] private bool isHuman;

    private void OnEnable()
    {
        GameEventBus.Subscribe<InitialDrawEvent>(OnDrawStart);
        GameEventBus.Subscribe<MulliganPhaseEvent>(OnMulliganStart);
        GameEventBus.Subscribe<TurnStartEvent>(OnTurnStart);

    }

    private void OnDisable()
    {
        GameEventBus.Subscribe<InitialDrawEvent>(OnDrawStart);
        GameEventBus.Subscribe<MulliganPhaseEvent>(OnMulliganStart);
        GameEventBus.Unsubscribe<TurnStartEvent>(OnTurnStart);
    }

    private void OnDrawStart(InitialDrawEvent e)
    {
        statusText.text = "Draw Hand...";
    }
    
    private void OnMulliganStart(MulliganPhaseEvent e)
    {
        statusText.text = "Mulligan Phase...";
    }

    private void OnTurnStart(TurnStartEvent e)
    {
        if (isHuman)
            statusText.text = e.CurrentPlayer.IsHuman ? "Your turn..." : "Enemy turn...";
        else
            statusText.text = e.CurrentPlayer.IsHuman ? "Enemy turn..." : "Your turn...";
    }


}
