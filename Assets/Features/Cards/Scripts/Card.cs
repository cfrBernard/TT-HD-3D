public class Card
{
    public CardData Data { get; private set; }
    public bool IsFacingUp { get; set; }

    public Card(CardData data)
    {
        Data = data;
        IsFacingUp = false;
    }
}
