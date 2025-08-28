public class Card
{
    public CardData Data { get; private set; }
    public Player Owner { get; set; }

    public Card(CardData data, Player owner)
    {
        Data = data;
        Owner = owner;
    }
}
