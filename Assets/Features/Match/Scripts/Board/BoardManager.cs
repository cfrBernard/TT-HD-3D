using System;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour, IBoard
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

    public IEnumerable<(int x, int y)> GetFreeSlots()
    {
        for (int x = 0; x < SIZE; x++)
        {
            for (int y = 0; y < SIZE; y++)
            {
                var slot = GetSlot(x, y);
                if (slot != null && slot.IsEmpty)
                    yield return (x, y);
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

        // Marque la carte comme jouée
        card.MarkAsOnBoard();

        OnCardPlaced?.Invoke(x, y, card);

        Debug.Log($"[BoardManager] {card.Data.name} placed at {x},{y} by {card.Owner}");
        return true;
    }
    
    // --- Implémentation explicite de IBoard (délègue aux méthodes existantes) ---
    IBoardSlot IBoard.GetSlot(int x, int y) => GetSlot(x, y);
    IEnumerable<IBoardSlot> IBoard.GetAllSlots()
    {
        foreach (var s in GetAllSlots())
            yield return s;
    }
    bool IBoard.TryPlaceCard(int x, int y, Card card) => TryPlaceCard(x, y, card);
    IBoardSlot IBoard.GetSlotOfCard(Card card) => GetSlotOfCard(card);
    IEnumerable<(int x, int y)> IBoard.GetFreeSlots() => GetFreeSlots();
}
