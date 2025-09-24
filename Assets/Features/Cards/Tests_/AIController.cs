using System.Collections;
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
        yield return new WaitForSeconds(Random.Range(1.5f, 2.5f));

        if (player.Hand.Count == 0)
        {
            Debug.LogWarning("[Turn][AI] No cards in hand!");
            yield break;
        }

        // --- 1. Trouver le meilleur coup ---
        var bestMove = FindBestMove(player, board);

        if (bestMove.HasValue)
        {
            var (card, x, y) = bestMove.Value;
            PlayCard(card, player, board, x, y);
        }
        else
        {
            Debug.LogWarning("[Turn][AI] No valid move found.");
        }

        // Unlock drag
        CardDragHandler.DragLocked = false;
        // CardHoverHandler.HoverLocked = false; --- TESTING ---
    }

    // =====================================================
    // === AI
    // =====================================================
    private (Card, int, int)? FindBestMove(Player player, BoardManager board)
    {
        float bestScore = float.NegativeInfinity;
        (Card, int, int)? bestMove = null;

        // Convertir une fois le board courant en état
        var baseState = BoardConverter.ToState(board);

        foreach (var card in player.Hand)
        {
            foreach (var (x, y) in baseState.GetFreeSlots())
            {
                // --- Simulation ---
                var simState = baseState.Clone();
                var simManager = new SimBoardManager(simState);

                var simCard = card.CloneForSim();
                simCard.Owner = player;

                if (!simManager.TryPlaceCard(x, y, simCard))
                    continue;

                var rules = new RuleEngine();
                rules.Resolve(simManager, x, y, simCard);

                float score = EvaluateBoard(simState, player);

                if (score > bestScore)
                {
                    bestScore = score;
                    bestMove = (card, x, y);
                }
            }
        }

        return bestMove;
    }

    private float EvaluateBoard(BoardState state, Player me)
    {
        int myCards = 0, enemyCards = 0;

        foreach (var c in state.GetAllCards())
        {
            if (c.Owner == me) myCards++;
            else enemyCards++;
        }

        float score = myCards - enemyCards;

        // Exemple d’heuristique :
        foreach (var (x, y) in state.GetFreeSlots())
        {
            // Si une case libre est un coin -> valeur stratégique
            if ((x == 0 || x == BoardState.SIZE - 1) && (y == 0 || y == BoardState.SIZE - 1))
                score += 0.2f;
        }

        return score;
    }

    // =====================================================
    // === ACTIONS
    // =====================================================
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

    // =====================================================
    // === MULLIGAN PHASE
    // =====================================================
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
            Debug.Log($"[Mulligan][AI] {player.Name} keeps his hand");
        }

        onDone?.Invoke();
    }

    public void CancelMulligan(Player player)
    {
        // AI n’a rien à nettoyer
    }
}
