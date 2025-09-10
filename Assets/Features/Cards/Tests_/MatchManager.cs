using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MatchManager : MonoBehaviour
{
    // TESTING REFz2 – placeholder
    public List<CardData> dummyDeck;

    public RuleEngine rules;
    public BoardManager board;
    public ScoreManager scoreManager;

    public Player player1;
    public Player player2;

    public HandManager player1Hand;
    public HandManager player2Hand;

    private IPlayerController controller1;
    private IPlayerController controller2;

    // AIController
    public CardViewRegistry cardRegistry;
    public BoardViewManager boardView;

    void Start()
    {
        // --- Init Rules ---
        rules = new RuleEngine();
        rules.enableSame = true;
        rules.enablePlus = true;
        rules.enableCombo = true;
        rules.enableBasic = true;

        board.OnCardPlaced += (x, y, card) => rules.Resolve(board, x, y, card);

        // --- Init player ---
        player1 = new Player("Player1");
        player2 = new Player("Player2");

        // --- Creates decks – TESTING REFz2 ---
        player1.SetDeck(CreateDeck(dummyDeck, player1));
        player2.SetDeck(CreateDeck(dummyDeck, player2));

        // --- Setup controllers ---
        controller1 = new HumanController();
        controller2 = new AIController(cardRegistry, boardView);
        player1.Controller = controller1;
        player2.Controller = controller2;

        // --- Init player hands ---
        player1Hand.Init(player1, true);    // true = flip
        player2Hand.Init(player2, false);   // false = no flip
        player1.HandManager = player1Hand;
        player2.HandManager = player2Hand;

        StartCoroutine(GameLoop());
    }

    IEnumerator GameLoop()
    {
        // block drag ???
        CardDragHandler.DragLocked = true;

        // --- 1. Draw hands ---
        player1Hand.DrawStartingHand();
        player2Hand.DrawStartingHand();
        yield return new WaitForSeconds(3f); // (int) = HandManager.DrawStartingHand.Delay *5 + 0.5

        // --- 2. Mulligan phase ---
        yield return RunMulliganPhase(player1, player2, 10f);

        // --- 3. Decide starter ---
        Player current = DecideStartingPlayer();
        Player other = (current == player1) ? player2 : player1;

        scoreManager.Init();

        // active drag ???
        CardDragHandler.DragLocked = false;

        // --- 4. Main loop ---
        while (!board.IsFull())
        {
            // block drag
            CardDragHandler.CurrentPlayerTurn = current;

            yield return current.Controller.TakeTurn(current, board);

            // switch player
            Player temp = current;
            current = other;
            other = temp;
        }

        // --- 5. End of match ---
        Debug.Log("[MatchManager] Game Over! Board is full.");
    }

    // TESTING REFz2 – placeholder
    List<Card> CreateDeck(List<CardData> data, Player owner)
    {
        List<Card> deck = new List<Card>();
        foreach (var d in data)
            deck.Add(new Card(d, owner));
        return deck;
    }

    Player DecideStartingPlayer()
    {
        bool coin = Random.value > 0.5f;
        Player starter = coin ? player1 : player2;
        Debug.Log($"[MatchManager] Toss result: {starter.Name} starts!");
        return starter;
    }

    IEnumerator RunMulliganPhase(Player p1, Player p2, float duration)
    {
        bool p1Done = false;
        bool p2Done = false;
    
        float timer = duration;
    
        // IA peut choisir direct (sinon elle attend le timer)
        p1.Controller.BeginMulligan(p1, () => p1Done = true);
        p2.Controller.BeginMulligan(p2, () => p2Done = true);
    
        Debug.Log("[MatchManager] Mulligan Phase started");
    
        while (timer > 0f && !(p1Done && p2Done))
        {
            // TODO : update UI "Game start {timer:F1} sec"
            timer -= Time.deltaTime;
            yield return null;
        }

        if (!p1Done) p1.Controller.CancelMulligan(p1);
        if (!p2Done) p2.Controller.CancelMulligan(p2);
    
        Debug.Log("[MatchManager] Mulligan Phase ended");
    }
}
