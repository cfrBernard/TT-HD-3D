using UnityEngine;
using DG.Tweening;
using System.Linq;

public class HandManager : MonoBehaviour
{
    public Player Player => player;

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

            // Hover handler
            var hover = cardGO.GetComponent<CardHoverHandler>();
            if (hover != null)
                hover.InitHover(handLayout, targetSlot);
        }
    }

    public void MulliganCard(Card oldCard, Card newCard, int slotIndex)
    {
        Player.RemoveFromHand(oldCard);
        var oldView = registry.GetView(oldCard);
        if (oldView == null)
        {
            Debug.LogWarning("[HandManager] Old card view not found");
            return;
        }

        var animator = oldView.GetComponent<CardAnimator>();
        animator.AnimateReturn(deckPosition.position, () =>
        {
            registry.Unregister(oldCard);
            DOTween.Kill(oldView.gameObject, true);
            Destroy(oldView.gameObject);

            if (newCard != null)
            {
                GameObject cardGO = Instantiate(cardPrefab);
                CardView newView = cardGO.GetComponent<CardView>();
                newView.Setup(newCard);

                registry.Register(newCard, newView);

                cardGO.transform.rotation = Quaternion.Euler(-90, 180, 0);
                cardGO.transform.position = deckPosition.position;

                Transform targetSlot = handLayout.slots[slotIndex];
                newView.AnimateDraw(deckPosition.position, targetSlot, isPlayer1, 0f);

                // Hover handler
                var hover = cardGO.GetComponent<CardHoverHandler>();
                if (hover != null)
                    hover.InitHover(handLayout, targetSlot);

                Debug.Log($"[HandManager][Hand Debug] {player.Name} hand after mulligan : " + string.Join(", ", player.Hand.Select(c => c.Data.name)));
            }
        });
    }
}
