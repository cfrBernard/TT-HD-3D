using System.Collections;
using UnityEngine;

public class HumanController : IPlayerController
{
    private bool cardPlacedThisTurn = false;

    private System.Action mulliganDone;
    private bool mulliganUsed = false;

    public IEnumerator TakeTurn(Player player, BoardManager board)
    {
        Debug.Log($"[Turn][Human] {player.Name}'s turn");

        cardPlacedThisTurn = false;

        // événement de pose d'une carte
        CardDragHandler.OnCardPlaced += OnCardPlaced;

        // joueur place une carte
        while (!cardPlacedThisTurn)
        {
            yield return null;
        }

        CardDragHandler.OnCardPlaced -= OnCardPlaced;

        // captures, effets, etc.
        Debug.Log($"[Turn][Human] {player.Name} finished turn");
    }

    private void OnCardPlaced(Card card)
    {
        cardPlacedThisTurn = true;
    }

    // =====================================================
    // === MULLIGAN PHASE
    // =====================================================
    public void BeginMulligan(Player player, System.Action onDone)
    {
        mulliganDone = onDone;
        mulliganUsed = false;

        Debug.Log($"[Mulligan][Human] {player.Name} can replace a card");

        // Active le clic
        CardClickHandler.MulliganActive = true;
        CardClickHandler.CurrentPlayer = player;

        // Abonne à l’event
        CardClickHandler.OnCardClicked += HandleMulliganClick;
    }

    private void HandleMulliganClick(Card card)
    {
        if (mulliganUsed) return;
        mulliganUsed = true;

        Player player = card.Owner;
        int slotIndex = player.Hand.IndexOf(card);

        Card newCard = player.DrawCard();
        player.HandManager.MulliganCard(card, newCard, slotIndex);

        Debug.Log($"[Mulligan][Human] {player.Name} exchanged '{card.Data.name}' for '{newCard?.Data.name}'");

        // Cleanup
        CardClickHandler.OnCardClicked -= HandleMulliganClick;
        CardClickHandler.MulliganActive = false;

        mulliganDone?.Invoke();
    }

    public void CancelMulligan(Player player)
    {
        CardClickHandler.OnCardClicked -= HandleMulliganClick;
        CardClickHandler.MulliganActive = false;

        mulliganDone?.Invoke();
        mulliganDone = null;
    }
}
