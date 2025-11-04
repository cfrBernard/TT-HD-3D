public class BoardSlot : IBoardSlot
{
    public int X { get; private set; }
    public int Y { get; private set; }
    public Card Occupant { get; private set; }

    public bool IsEmpty => Occupant == null;

    public BoardSlot(int x, int y)
    {
        X = x;
        Y = y;
    }

    public void PlaceCard(Card card)
    {
        Occupant = card;
    }
}
