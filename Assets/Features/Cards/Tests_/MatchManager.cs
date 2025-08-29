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
        bool isPlayer1Turn = true;

        while (!board.IsFull())
        {
            Player current = isPlayer1Turn ? player1 : player2;

            // Pour l’instant : prend la 1ère carte de la main
            Card cardToPlay = current.Hand[0];
            current.RemoveFromHand(cardToPlay);

            // Placement dummy : toujours dans la première case libre
            bool placed = false;
            for (int x = 0; x < BoardManager.SIZE && !placed; x++)
            {
                for (int y = 0; y < BoardManager.SIZE && !placed; y++)
                {
                    if (board.GetSlot(x, y).IsEmpty)
                    {
                        board.TryPlaceCard(x, y, cardToPlay);
                        placed = true;
                    }
                }
            }

            // Attends un peu pour debug
            yield return new WaitForSeconds(1f);

            // Change de joueur
            isPlayer1Turn = !isPlayer1Turn;
        }

        Debug.Log("Game Over! Board is full.");
    }
}
