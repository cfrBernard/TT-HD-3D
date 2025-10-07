using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MatchManager : MonoBehaviour
{
    // TESTING REFz2 – placeholder
    public List<CardData> dummyDeck;

    // AIController
    public CardViewRegistry cardRegistry;
    public BoardViewManager boardView;

    public RuleEngine rules;
    public BoardManager board;
    public ScoreManager scoreManager;

    public Player player1;
    public Player player2;

    public HandManager player1Hand;
    public HandManager player2Hand;

    private IPlayerController controller1;
    private IPlayerController controller2;

    void Start()
    {
        // --- Init Rules ---
        rules = new RuleEngine();
        rules.LoadFromSettings(SettingsManager.Instance);

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

    private IEnumerator GameLoop()
    {
        // Lock all interactions at start
        CardDragHandler.DragLocked = true;
        CardHoverHandler.HoverLocked = true;

        // --- 1. Intro ---
        yield return StartCoroutine(IntroPhase());

        // --- 2. Draw ---
        yield return StartCoroutine(DrawPhase());

        // --- 3. Mulligan ---
        yield return StartCoroutine(MulliganPhase());

        // --- 4. Main Loop ---
        yield return StartCoroutine(MainLoop());

        // --- 5. End ---
        yield return StartCoroutine(EndPhase());
    }

    #region Intro
    // =====================================================
    // === 1. INTRO
    // =====================================================
    private IEnumerator IntroPhase()
    {
        UIManager.Instance.ShowPanel("GameIntro");
        yield return new WaitForSeconds(2f);
        UIManager.Instance.HidePanel("GameIntro");
        UIManager.Instance.HidePanel("BackG");
        UIManager.Instance.ShowPanel("PlayerUI_1");
        UIManager.Instance.ShowPanel("PlayerUI_2");
    }
    #endregion

    #region Draw
    // =====================================================
    // === 2. DRAW
    // =====================================================
    private IEnumerator DrawPhase()
    {
        player1Hand.DrawStartingHand();
        player2Hand.DrawStartingHand();
        yield return new WaitForSeconds(3f);
        // (int) = DrawStartingHand.Delay *5 + 0.5
    }
    #endregion

    #region Mulligan
    // =====================================================
    // === 3. MULLIGAN
    // =====================================================
    private IEnumerator MulliganPhase()
    {
        UIManager.Instance.ShowPanel("MulliganPhase");
        yield return new WaitForSeconds(2f);
        UIManager.Instance.ShowPanel("MulliganButton");
        UIManager.Instance.HidePanel("MulliganPhase");

        CardHoverHandler.HoverLocked = false;

        // Mulligan phase
        yield return RunMulliganPhase(player1, player2);

        UIManager.Instance.HidePanel("MulliganButton");
    }

    private bool skipRequested = false;

    public void RequestSkip()
    {
        skipRequested = true;
    }

    IEnumerator RunMulliganPhase(Player p1, Player p2)
    {
        bool p1Done = false;
        bool p2Done = false;
        skipRequested = false;

        // Players lancent leur mulligan
        p1.Controller.BeginMulligan(p1, () => p1Done = true);
        p2.Controller.BeginMulligan(p2, () => p2Done = true);

        Debug.Log("[MatchManager] Mulligan Phase started");

        while (!skipRequested && !(p1Done && p2Done))
        {
            yield return null;
        }

        if (!p1Done) p1.Controller.CancelMulligan(p1);
        if (!p2Done) p2.Controller.CancelMulligan(p2);

        Debug.Log("[MatchManager] Mulligan Phase ended");
    }
    #endregion

    #region Main Loop
    // =====================================================
    // === 4. MAIN LOOP
    // =====================================================
    private IEnumerator MainLoop()
    {
        Player current = DecideStartingPlayer();
        Player other = (current == player1) ? player2 : player1;

        UIManager.Instance.ShowPanel("TurnStart");
        yield return new WaitForSeconds(1.5f);
        UIManager.Instance.HidePanel("TurnStart");
        UIManager.Instance.ShowPanel("MenuButton");

        scoreManager.Init();

        CardDragHandler.DragLocked = false;

        // Boucle principale
        while (!board.IsFull())
        {
            CardDragHandler.CurrentPlayerTurn = current;
            yield return current.Controller.TakeTurn(current, board);

            // Switch player
            (current, other) = (other, current);
        }
    }
    
    Player DecideStartingPlayer()
    {
        bool coin = Random.value > 0.5f;
        Player starter = coin ? player1 : player2;
        Debug.Log($"[MatchManager] Toss result: {starter.Name} starts!");
        return starter;
    }
    #endregion

    #region End
    // =====================================================
    // === 5. END
    // =====================================================
    private IEnumerator EndPhase()
    {
        Debug.Log("[MatchManager] Game Over! Board is full.");
        yield return new WaitForSeconds(1f);
        UIManager.Instance.ShowPanel("EndMatch");
        UIManager.Instance.ShowPanel("BackG");
        UIManager.Instance.HidePanel("MenuButton");
        UIManager.Instance.HidePanel("PlayerUI_1");
        UIManager.Instance.HidePanel("PlayerUI_2");
    }
    #endregion

    #region Internal
    // =====================================================
    // === INTERNAL
    // =====================================================

    // TESTING REFz2 – placeholder
    List<Card> CreateDeck(List<CardData> data, Player owner)
    {
        List<Card> deck = new List<Card>();
        foreach (var d in data)
            deck.Add(new Card(d, owner));
        return deck;
    }
    #endregion
}
