using UnityEngine;

public class HandManager : MonoBehaviour
{
    public HandLayout handLayout;
    public Transform deckPosition;
    public GameObject cardPrefab;

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
            CardView view = cardGO.GetComponent<CardView>();
            view.Setup(card);

            // Pop en rotation locale 0,180,0 et position du deck
            cardGO.transform.rotation = Quaternion.Euler(-90, 180, 0);
            cardGO.transform.position = deckPosition.position;

            float delay = i * 0.5f;
            Transform targetSlot = handLayout.slots[i];

            // Animation vers slot + flip
            view.AnimateDraw(deckPosition.position, targetSlot, isPlayer1, delay);
        }
    }
}
