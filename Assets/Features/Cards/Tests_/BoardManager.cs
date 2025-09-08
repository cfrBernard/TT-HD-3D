using System;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public const int SIZE = 3;
    private BoardSlot[,] slots = new BoardSlot[SIZE, SIZE];

    public event Action<int, int, Card> OnCardPlaced;

    public void Awake()
    {
        // Init logique du plateau
        for (int x = 0; x < SIZE; x++)
        {
            for (int y = 0; y < SIZE; y++)
            {
                slots[x, y] = new BoardSlot(x, y);
            }
        }
    }

    public bool IsFull()
    {
        foreach (var slot in slots)
            if (slot.IsEmpty) return false;
        return true;
    }

    public IEnumerable<BoardSlot> GetAllSlots()
    {
        for (int x = 0; x < SIZE; x++)
        {
            for (int y = 0; y < SIZE; y++)
            {
                yield return slots[x, y];
            }
        }
    }

    public BoardSlot GetSlotOfCard(Card card)
    {
        for (int x = 0; x < SIZE; x++)
        {
            for (int y = 0; y < SIZE; y++)
            {
                var slot = slots[x, y];
                if (!slot.IsEmpty && slot.Occupant == card)
                    return slot;
            }
        }
        return null; // pas trouvé
    }

    public BoardSlot GetSlot(int x, int y) => slots[x, y];

    public bool TryPlaceCard(int x, int y, Card card)
    {
        BoardSlot slot = GetSlot(x, y);
        if (!slot.IsEmpty)
        {
            Debug.LogWarning($"[BoardManager] Slot {x},{y} already occupied.");
            return false;
        }

        slot.PlaceCard(card);

        OnCardPlaced?.Invoke(x, y, card);

        Debug.Log($"[BoardManager] {card.Data.name} placed at {x},{y} by {card.Owner}");
        return true;
    }
}
