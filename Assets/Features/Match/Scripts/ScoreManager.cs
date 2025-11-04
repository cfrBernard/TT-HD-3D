using TMPro;
using UnityEngine;
using System.Collections.Generic;

public class ScoreManager : MonoBehaviour
{
    private int player1Score;
    private int player2Score;

    public TextMeshProUGUI player1Ui;
    public TextMeshProUGUI player2Ui;

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

    private void OnCardOwnerChanged(CardOwnerChanged evt)
    {
        RecalculateScore();
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

        if (player1Ui != null)
            player1Ui.text = player1Score.ToString();

        if (player2Ui != null)
            player2Ui.text = player2Score.ToString();
    }

    public (int player1, int player2) GetScores() => (player1Score, player2Score);
}
