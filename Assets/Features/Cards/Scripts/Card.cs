public class Card
{
    public CardData Data { get; private set; }
    public Player Owner { get; set; }

    public Card(CardData data, Player owner)
    {
        Data = data;
        Owner = owner;
    }

    public void SetOwner(Player newOwner)
    {
        var oldOwner = Owner;
        Owner = newOwner;

        var view = CardViewRegistry.Instance.GetView(this);
        view?.UpdateOwnerVisual();
        
        GameEventBus.Publish(new CardOwnerChanged(this, oldOwner, newOwner));
    }
}
