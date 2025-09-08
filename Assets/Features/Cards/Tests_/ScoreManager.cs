using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    private int player1Score;
    private int player2Score;

    [SerializeField] private BoardManager board;
    [SerializeField] private MatchManager matchManager;

    private List<Card> allCards = new List<Card>();

    private void OnEnable()
    {
        GameEventBus.Subscribe<CardOwnerChanged>(OnCardOwnerChanged);
    }

    private void OnDisable()
    {
        GameEventBus.Unsubscribe<CardOwnerChanged>(OnCardOwnerChanged);
    }

    public void Init()
    {
        allCards.Clear();

        // Board cards
        foreach (var slot in board.GetAllSlots())
            if (!slot.IsEmpty) allCards.Add(slot.Occupant);

        // Hand cards
        allCards.AddRange(matchManager.player1.Hand);
        allCards.AddRange(matchManager.player2.Hand);

        RecalculateScore();
    }

    private void OnCardOwnerChanged(CardOwnerChanged evt)
    {
        UpdateScoreForCard(evt.OldOwner, evt.NewOwner, evt.Card);
    }

    private void UpdateScoreForCard(Player oldOwner, Player newOwner, Card card)
    {
        if (oldOwner == matchManager.player1) player1Score--;
        else if (oldOwner == matchManager.player2) player2Score--;

        if (newOwner == matchManager.player1) player1Score++;
        else if (newOwner == matchManager.player2) player2Score++;

        UpdateScoreDisplay();
    }

    private void RecalculateScore()
    {
        player1Score = 0;
        player2Score = 0;

        foreach (var card in allCards)
        {
            if (card.Owner == matchManager.player1) player1Score++;
            else if (card.Owner == matchManager.player2) player2Score++;
        }

        UpdateScoreDisplay();
    }

    private void UpdateScoreDisplay()
    {
        Debug.Log($"Score => Player1: {player1Score} | Player2: {player2Score}");
    }

    public (int player1, int player2) GetScores() => (player1Score, player2Score);
}
