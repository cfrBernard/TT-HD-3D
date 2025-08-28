using UnityEngine;

public class HandManager : MonoBehaviour
{
    public HandLayout handLayout;
    public Transform deckPosition;
    public GameObject cardPrefab;

    public CardViewRegistry registry;

    private Player player;
    private bool isPlayer1;

    public void Init(Player p, bool isP1)
    {
        player = p;
        isPlayer1 = isP1;
    }

    public void DrawStartingHand(int count = 5)
    {
        for (int i = 0; i < count; i++)
        {
            Card card = player.DrawCard();
            if (card == null) continue;

            GameObject cardGO = Instantiate(cardPrefab);

            Debug.Log($"[HandManager] Player {(isPlayer1 ? "1" : "2")} drew card '{card.Data.name}' and instantiated a view.");

            CardView view = cardGO.GetComponent<CardView>();
            view.Setup(card);

            registry.Register(card, view);

            // Pop en rotation locale 0,180,0 et position du deck
            cardGO.transform.rotation = Quaternion.Euler(-90, 180, 0);
            cardGO.transform.position = deckPosition.position;

            float delay = i * 0.5f; // Draw delay
            Transform targetSlot = handLayout.slots[i];

            // Animation vers slot + flip
            view.AnimateDraw(deckPosition.position, targetSlot, isPlayer1, delay);
        }
    }
}
