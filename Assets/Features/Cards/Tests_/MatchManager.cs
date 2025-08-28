using UnityEngine;
using System.Collections.Generic;

public class MatchManager : MonoBehaviour
{
    public HandManager player1Hand;
    public HandManager player2Hand;

    // TESTING REFz2 (connect the real deck later)
    public List<CardData> dummyDeck;

    public Player player1;
    public Player player2;

    //private IPlayerController controller1;
    //private IPlayerController controller2;

    void Start()
    {
        // Crée les joueurs
        player1 = new Player("Player1");
        player2 = new Player("Player2");

        // Crée les decks en passant directement le Player comme owner
        player1.SetDeck(CreateDeck(dummyDeck, player1));
        player2.SetDeck(CreateDeck(dummyDeck, player2));

        // Setup controllers (later)
        //controller1 = new HumanController();
        //controller2 = new AIController();

        // Init player hands
        player1Hand.Init(player1, true);  // true = flip
        player2Hand.Init(player2, false); // false = no flip

        player1Hand.DrawStartingHand();
        player2Hand.DrawStartingHand();

        // StartCoroutine(GameLoop()); REFz1 ----------------
    }

    // TESTING REFz2 (connect the real deck later)
    List<Card> CreateDeck(List<CardData> data, Player owner)
    {
        List<Card> deck = new List<Card>();
        foreach (var d in data)
            deck.Add(new Card(d, owner));
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
