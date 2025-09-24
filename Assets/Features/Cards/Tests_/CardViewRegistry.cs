using System.Collections.Generic;
using UnityEngine;

public class CardViewRegistry : MonoBehaviour
{
    // --- Singleton ---
    public static CardViewRegistry Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    // --- Registry ---
    private Dictionary<Card, CardView> registry = new Dictionary<Card, CardView>();

    public void Register(Card card, CardView view)
    {
        if (!registry.ContainsKey(card))
        {
            registry.Add(card, view);
            Debug.Log($"[Registry] Registered card '{card.Data.name}' for owner {card.Owner}");
        }
        else
        {
            Debug.LogWarning($"[Registry] Tried to register card '{card.Data.name}' but it already exists.");
        }
    }

    public void Unregister(Card card)
    {
        if (registry.ContainsKey(card))
        {
            registry.Remove(card);
            Debug.Log($"[Registry] Unregistered card '{card.Data.name}' for owner {card.Owner}.");
        }
        else
        {
            Debug.LogWarning($"[Registry] Tried to unregister card '{card.Data.name}' but it was not found.");
        }
    }

    public CardView GetView(Card card)
    {
        if (registry.TryGetValue(card, out var view))
        {
            Debug.Log($"[Registry] Found view for card '{card.Data.name}' for owner {card.Owner}.");
            return view;
        }

        // Pendant simulation (IA, clones)
        if (Application.isPlaying)
        {
            Debug.LogWarning($"[Registry] No view found for card '{card.Data.name}'. Probably a clone used in simulation.");
        }

        return null;
    }
}
