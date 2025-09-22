using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : IPlayerController
{
    private CardViewRegistry viewRegistry;
    private BoardViewManager boardView;

    public AIController(CardViewRegistry registry, BoardViewManager viewManager)
    {
        viewRegistry = registry;
        boardView = viewManager;
    }

    public IEnumerator TakeTurn(Player player, BoardManager board)
    {
        // Block drag
        CardDragHandler.DragLocked = true;
        // CardHoverHandler.HoverLocked = true; --- TESTING ---

        Debug.Log($"[Turn][AI] {player.Name}'s turn");
        yield return new WaitForSeconds(Random.Range(2.5f, 4.5f));

        if (player.Hand.Count == 0)
        {
            Debug.LogWarning("[Turn][AI] No cards in hand!");
            yield break;
        }

        // --- 1. Choisir la carte à jouer ---
        Card cardToPlay = ChooseCard(player, board);

        // --- 2. Choisir le slot où la poser ---
        (int x, int y)? slot = ChooseSlot(cardToPlay, player, board);

        if (slot.HasValue)
        {
            // --- 3. Jouer la carte ---
            PlayCard(cardToPlay, player, board, slot.Value.x, slot.Value.y);
        }
        else
        {
            Debug.LogWarning("[Turn][AI] No valid slot to play this turn.");
        }

        // Active drag
        CardDragHandler.DragLocked = false;
        // CardHoverHandler.HoverLocked = false; --- TESTING ---
    }

    // Placeholder: pour l'instant choisit une carte random
    private Card ChooseCard(Player player, BoardManager board)
    {
        return player.Hand[Random.Range(0, player.Hand.Count)];
    }

    // Placeholder: pour l'instant choisit un slot libre random
    private (int x, int y)? ChooseSlot(Card card, Player player, BoardManager board)
    {
        var freeSlots = new List<(int x, int y)>();
        for (int x = 0; x < BoardManager.SIZE; x++)
        {
            for (int y = 0; y < BoardManager.SIZE; y++)
            {
                if (board.GetSlot(x, y).IsEmpty)
                    freeSlots.Add((x, y));
            }
        }

        if (freeSlots.Count == 0) return null;

        return freeSlots[Random.Range(0, freeSlots.Count)];
    }

    private void PlayCard(Card card, Player player, BoardManager board, int x, int y)
    {
        if (board.TryPlaceCard(x, y, card))
        {
            player.RemoveFromHand(card);

            Debug.Log($"[Turn][AI] {player.Name} placed {card.Data.name} at {x},{y}");

            // Animation
            var cardView = viewRegistry.GetView(card);
            var slotView = boardView.GetView(x, y);

            if (cardView != null && slotView != null)
            {
                var animator = cardView.GetComponent<CardAnimator>();
                if (animator != null)
                    animator.AnimatePlay(slotView.transform);
            }
        }
    }

    // --- MULLIGAN PHASE --- AI
    public void BeginMulligan(Player player, System.Action onDone)
    {
        if (player.Hand.Count > 0 && Random.value > 0.5f)
        {
            int idx = Random.Range(0, player.Hand.Count);
            Card oldCard = player.Hand[idx];
            Card newCard = player.DrawCard();

            player.HandManager.MulliganCard(oldCard, newCard, idx);

            Debug.Log($"[Mulligan][AI] {player.Name} exchanged '{oldCard.Data.name}' for '{newCard?.Data.name}'");
        }
        else
        {
            Debug.Log($"[Mulligan][AI] {player.Name} keep his hand");
        }

        onDone?.Invoke();
    }

    public void CancelMulligan(Player player)
    {
        // AI n’a rien à nettoyer
    }
}
