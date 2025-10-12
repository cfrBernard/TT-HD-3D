using System.Collections.Generic;
using UnityEngine;

public class Player
{
    public string Name { get; private set; }
    public bool IsHuman { get; private set; }
    public int Score { get; set; }

    public HandManager HandManager { get; set; }
    public IPlayerController Controller { get; set; }

    public List<Card> Deck { get; private set; } = new List<Card>();
    public List<Card> Hand { get; private set; } = new List<Card>();

    public Player(string name, bool isHuman = false)
    {
        Name = name;
        IsHuman = isHuman;
    }

    public void SetDeck(List<Card> deck)
    {
        Deck = deck;

        foreach (var card in Deck)
            card.Owner = this;
    }

    public Card DrawCard()
    {
        if (Deck.Count == 0) return null;
        Card drawn = Deck[0];
        Deck.RemoveAt(0);
        Hand.Add(drawn);

        Debug.Log($"[Player] {Name} drew '{drawn.Data.name}'. Current hand : {Hand.Count} cards.");
        
        return drawn;
    }

    public override string ToString() => Name;
    public void RemoveFromHand(Card card) => Hand.Remove(card);
}
