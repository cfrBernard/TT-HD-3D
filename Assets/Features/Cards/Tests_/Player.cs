using System.Collections.Generic;

public class Player
{
    public string Name; // For logs

    public IPlayerController Controller { get; set; }

    public List<Card> Deck { get; private set; } = new List<Card>();
    public List<Card> Hand { get; private set; } = new List<Card>();

    public Player(string name)
    {
        Name = name;
    }

    public void SetDeck(List<Card> deck)
    {
        Deck = deck;

        foreach (var card in Deck)
            card.Owner = this;
    }

    public override string ToString() => Name;

    public Card DrawCard()
    {
        if (Deck.Count == 0) return null;
        Card drawn = Deck[0];
        Deck.RemoveAt(0);
        Hand.Add(drawn);
        return drawn;
    }

    public void RemoveFromHand(Card card) => Hand.Remove(card);
}
