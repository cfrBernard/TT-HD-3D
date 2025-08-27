using System.Collections.Generic;

public class Player
{
    public List<Card> Deck = new List<Card>(); // deck complet
    public List<Card> Hand = new List<Card>(); // main du joueur

    public Player(List<Card> startingDeck)
    {
        Deck = new List<Card>(startingDeck);
        Hand = new List<Card>();
    }

    public Card DrawCard()
    {
        if (Deck.Count == 0) return null;

        Card drawn = Deck[0];
        Deck.RemoveAt(0);
        Hand.Add(drawn);
        return drawn;
    }

    public void RemoveFromHand(Card card)
    {
        Hand.Remove(card);
    }
}

