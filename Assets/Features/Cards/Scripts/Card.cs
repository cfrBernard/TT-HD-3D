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
        Owner = newOwner;

        // Optionnel : notifier la vue si tu as un CardView associé
        // var view = CardViewRegistry.Instance.GetView(this);
        // view?.UpdateOwnerVisual();
    }
}
