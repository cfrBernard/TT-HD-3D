public class Card
{
    public CardData Data { get; private set; }
    public Player Owner { get; set; }

    public bool IsOnBoard { get; private set; }

    public Card(CardData data, Player owner)
    {
        Data = data;
        Owner = owner;
        IsOnBoard = false;
    }

    public void SetOwner(Player newOwner)
    {
        var oldOwner = Owner;
        Owner = newOwner;

        var view = CardViewRegistry.Instance.GetView(this);
        view?.UpdateOwnerVisual();

        GameEventBus.Publish(new CardOwnerChanged(this, oldOwner, newOwner));
    }

    // Appelé par BoardManager (et sa simu) quand la carte est posée
    public void MarkAsOnBoard()
    {
        IsOnBoard = true;
    }

    public Card CloneForSim()
    {
        var newCard = new Card(this.Data, this.Owner);
        // Copier l’état "déjà posée ou pas" ???
        return newCard;
    }
}
