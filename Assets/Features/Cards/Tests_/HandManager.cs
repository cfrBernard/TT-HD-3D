using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class HandManager : MonoBehaviour
{
    [Header("Setup")]
    public CardDatabase database;
    public GameObject cardPrefab;

    [Header("Players Hands")]
    public Transform handParentP1;
    public Transform handParentP2;

    [Header("Deck Positions")]
    public Transform deckP1;
    public Transform deckP2;

    private Player player1;
    private Player player2;
    private List<Card> virtualDeck;

    void Start()
    {
        // Init Players
        player1 = new Player();
        player2 = new Player();

        // Crée un deck virtuel commun depuis la database
        virtualDeck = database.allCards.Select(cd => new Card(cd)).ToList();

        // donne 5 cartes à chaque joueur
        DrawInitialHand(player1, handParentP1, deckP1, 5, flipOnDraw: true);
        DrawInitialHand(player2, handParentP2, deckP2, 5, flipOnDraw: false);
    }

    void DrawInitialHand(Player player, Transform handParent, Transform deckPos, int number, bool flipOnDraw = false)
    {
        var randomCards = virtualDeck.OrderBy(x => Random.value).Take(number).ToList();
        player.Hand = randomCards;
        
        float delayBetweenCards = 0.2f;

        for (int i = 0; i < player.Hand.Count; i++)
        {
            var cardGO = Instantiate(cardPrefab, handParent);
            Vector3 startWorldPos = deckPos.position;
            cardGO.transform.position = startWorldPos;
            cardGO.transform.rotation = Quaternion.Euler(-90, 180, 0);

            cardGO.GetComponent<CardView>().Setup(player.Hand[i].Data);

            // target local dans la main
            Vector3 targetLocalPos = new Vector3(0, i * 0.8f, i * -0.01f);
            Quaternion targetLocalRot = Quaternion.identity;

            cardGO.GetComponent<CardAnimator>().AnimateTo(targetLocalPos, targetLocalRot, flipOnDraw, i * delayBetweenCards);

        }
    }


}
