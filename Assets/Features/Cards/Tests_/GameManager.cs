using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public HandManager player1Hand;
    public HandManager player2Hand;

    // TESTING REFz2 (need to conect the real deck)
    public List<CardData> dummyDeck;

    private Player player1;
    private Player player2;

    //private IPlayerController controller1;
    //private IPlayerController controller2;

    void Start()
    {
        // Setup players
        player1 = new Player(CreateDeck(dummyDeck));
        player2 = new Player(CreateDeck(dummyDeck));

        // Setup controllers
        //controller1 = new HumanController();
        //controller2 = new AIController();

        // Init player hands
        player1Hand.Init(player1, true);  // true = flip
        player2Hand.Init(player2, true); // false = no flip

        player1Hand.DrawStartingHand();
        player2Hand.DrawStartingHand();

        // StartCoroutine(GameLoop()); REFz1 ----------------
    }

    // TESTING REFz2 (need to conect the real deck)
    List<Card> CreateDeck(List<CardData> data)
    {
        List<Card> deck = new List<Card>();
        foreach (var d in data)
            deck.Add(new Card(d));
        return deck;
    }

    // REFz1 -------------------------------------------------
    //IEnumerator GameLoop()
    //{
    //    while (!IsGameOver())
    //    {
    //        yield return controller1.PlayTurn(player1, this);
    //        if (IsGameOver()) break;
    //
    //        yield return controller2.PlayTurn(player2, this);
    //    }
    //
    //    EndMatch();
    //}
}
