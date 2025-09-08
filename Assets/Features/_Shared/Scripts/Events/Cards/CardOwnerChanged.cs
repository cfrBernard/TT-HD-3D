public struct CardOwnerChanged
{
    public Card Card;
    public Player OldOwner;
    public Player NewOwner;

    public CardOwnerChanged(Card card, Player oldOwner, Player newOwner)
    {
        Card = card;
        OldOwner = oldOwner;
        NewOwner = newOwner;
    }
}
