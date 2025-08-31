using System.Collections;
using UnityEngine;

public class HumanController : IPlayerController
{
    private bool cardPlacedThisTurn = false;

    public IEnumerator TakeTurn(Player player, BoardManager board)
    {
        Debug.Log($"[Turn] {player.Name}'s turn (Human)");

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
        Debug.Log($"[Turn] {player.Name} finished turn");
    }

    private void OnCardPlaced(Card card)
    {
        cardPlacedThisTurn = true;
    }
}
