using System.Collections;
using UnityEngine;
using System.Linq;

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
    // === AI CORE
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
    // === MULLIGAN
    // =====================================================
    public void BeginMulligan(Player player, System.Action onDone)
    {
        if (player.Hand.Count == 0)
        {
            onDone?.Invoke();
            return;
        }

        Card weakestCard = FindWeakestCardForMulligan(player);
        if (weakestCard != null)
        {
            int idx = player.Hand.IndexOf(weakestCard);
            Card newCard = player.DrawCard();

            if (newCard != null)
            {
                player.HandManager.MulliganCard(weakestCard, newCard, idx);
                Debug.Log($"[Mulligan][AI] {player.Name} exchanged '{weakestCard.Data.name}' (score {EvaluateCardForMulligan(weakestCard, player):0.0}) " +
                          $"for '{newCard.Data.name}' (score {EvaluateCardForMulligan(newCard, player):0.0})");
            }
            else
            {
                Debug.LogWarning($"[Mulligan][AI] {player.Name} tried to mulligan '{weakestCard.Data.name}' but deck was empty!");
            }
        }
        else
        {
            Debug.Log($"[Mulligan][AI] {player.Name} keeps his hand (no card under threshold {MulliganThreshold})");
        }

        onDone?.Invoke();
    }

    private Card FindWeakestCardForMulligan(Player player)
    {
        float lowestScore = float.MaxValue;
        Card weakest = null;

        foreach (var card in player.Hand)
        {
            // Cartes jugées "intouchables"
            if (card.Data.level >= 8)
            {
                Debug.Log($"[Mulligan][AI] Skipping '{card.Data.name}' (level {card.Data.level}, considered untouchable)");
                continue;
            }

            float score = EvaluateCardForMulligan(card, player);
            Debug.Log($"[Mulligan][AI] '{card.Data.name}' evaluated with score {score:0.0}");

            // Considère seulement les cartes en dessous du seuil
            if (score < lowestScore && score <= MulliganThreshold)
            {
                lowestScore = score;
                weakest = card;
            }
        }

        if (weakest != null)
            Debug.Log($"[Mulligan][AI] Candidate for mulligan: '{weakest.Data.name}' with score {lowestScore:0.0}");

        return weakest;
    }

    private const float MulliganThreshold = 12f; // Seuil de mulligan

    // Évalue une carte : plus le score est haut, plus la carte est "forte".
    private float EvaluateCardForMulligan(Card card, Player me)
    {
        float score = 0f;

        // Puissance brute
        score += card.Data.north + card.Data.south + card.Data.west + card.Data.east;

        // Bonus si la carte est équilibrée
        int min = Mathf.Min(card.Data.north, card.Data.south, card.Data.west, card.Data.east);
        int max = Mathf.Max(card.Data.north, card.Data.south, card.Data.west, card.Data.east);
        score += (min + max) * 0.1f;

        // Pénalité si doublons
        int duplicates = me.Hand.Count(c => c.Data.cardId == card.Data.cardId);
        if (duplicates > 1)
        {
            float penalty = 3f * (duplicates - 1);
            score -= penalty;
            Debug.Log($"[Mulligan][AI] '{card.Data.name}' has {duplicates} duplicates → penalty {penalty}");
        }

        return score;
    }  
    public void CancelMulligan(Player player) { }
}
