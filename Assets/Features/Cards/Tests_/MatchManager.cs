using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MatchManager : MonoBehaviour
{
    public HandManager player1Hand;
    public HandManager player2Hand;

    // TESTING REFz2 (connect the real deck later)
    public List<CardData> dummyDeck;

    public BoardManager board;

    public RuleEngine rules;

    public Player player1;
    public Player player2;

    private IPlayerController controller1;
    private IPlayerController controller2;

    // AIController
    public CardViewRegistry cardRegistry;
    public BoardViewManager boardView;

    void Start()
    {
        // Init Rules
        rules = new RuleEngine(new BasicCaptureRule());
        board.OnCardPlaced += (x, y, card) => rules.Resolve(board, x, y, card);

        // Crée les joueurs
        player1 = new Player("Player1");
        player2 = new Player("Player2");

        // Crée les decks en passant le Player comme owner
        player1.SetDeck(CreateDeck(dummyDeck, player1));
        player2.SetDeck(CreateDeck(dummyDeck, player2));

        // Setup controllers
        controller1 = new HumanController();
        controller2 = new AIController(cardRegistry, boardView);
        player1.Controller = controller1;
        player2.Controller = controller2;

        // Init player hands
        player1Hand.Init(player1, true);  // true = flip
        player2Hand.Init(player2, false); // false = no flip

        player1Hand.DrawStartingHand();
        player2Hand.DrawStartingHand();

        StartCoroutine(GameLoop());
    }

    // TESTING REFz2 (connect the real deck later)
    List<Card> CreateDeck(List<CardData> data, Player owner)
    {
        List<Card> deck = new List<Card>();
        foreach (var d in data)
            deck.Add(new Card(d, owner));
        return deck;
    }

    IEnumerator GameLoop()
    {
        Player current = DecideStartingPlayer();
        Player other = (current == player1) ? player2 : player1;

        // Setup mulligan/draw
        // yield return DrawHand(current);
        // yield return MulliganPhase(current);
        // yield return DrawHand(other);
        // yield return MulliganPhase(other);

        // Tour normal
        while (!board.IsFull())
        {
            yield return current.Controller.TakeTurn(current, board);
            // switch player
            Player temp = current;
            current = other;
            other = temp;
        }

        // EndMatch();
        Debug.Log("[Match] Game Over! Board is full.");
    }

    Player DecideStartingPlayer()
    {
        bool coin = Random.value > 0.5f;
        Player starter = coin ? player1 : player2;
        Debug.Log($"[Match] Toss result: {starter.Name} starts!");
        return starter;
    }

}
