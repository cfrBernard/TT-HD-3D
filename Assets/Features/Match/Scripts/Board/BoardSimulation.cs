using System.Collections.Generic;

public class BoardState
{
    public const int SIZE = 3;
    private Card[,] slots = new Card[SIZE, SIZE];

    public bool IsEmpty(int x, int y) => slots[x, y] == null;
    public Card GetCard(int x, int y) => slots[x, y];

    public void PlaceCard(int x, int y, Card card)
    {
        if (slots[x, y] != null)
            throw new System.Exception($"[BoardState] Slot {x},{y} already occupied.");
        slots[x, y] = card;
    }

    public IEnumerable<(int x, int y)> GetFreeSlots()
    {
        for (int x = 0; x < SIZE; x++)
        {
            for (int y = 0; y < SIZE; y++)
            {
                if (slots[x, y] == null)
                    yield return (x, y);
            }
        }
    }

    public BoardState Clone()
    {
        var clone = new BoardState();
        for (int x = 0; x < SIZE; x++)
        {
            for (int y = 0; y < SIZE; y++)
            {
                if (slots[x, y] != null)
                    clone.slots[x, y] = slots[x, y].CloneForSim();
            }
        }
        return clone;
    }

    public IEnumerable<Card> GetAllCards()
    {
        for (int x = 0; x < SIZE; x++)
        {
            for (int y = 0; y < SIZE; y++)
            {
                if (slots[x, y] != null)
                    yield return slots[x, y];
            }
        }
    }
}

public static class BoardConverter
{
    public static BoardState ToState(BoardManager board)
    {
        var state = new BoardState();
        foreach (var slot in board.GetAllSlots())
        {
            if (!slot.IsEmpty)
                state.PlaceCard(slot.X, slot.Y, slot.Occupant.CloneForSim());
        }
        return state;
    }
}

public class SimBoardManager : IBoard
{
    private BoardState state;

    public SimBoardManager(BoardState state)
    {
        this.state = state;
    }

    public IBoardSlot GetSlot(int x, int y)
    {
        return new SimSlot(x, y, state.GetCard(x, y));
    }

    public IEnumerable<IBoardSlot> GetAllSlots()
    {
        for (int x = 0; x < BoardState.SIZE; x++)
            for (int y = 0; y < BoardState.SIZE; y++)
                yield return GetSlot(x, y);
    }

    public bool TryPlaceCard(int x, int y, Card card)
    {
        if (!state.IsEmpty(x, y)) return false;
        state.PlaceCard(x, y, card);
        card.MarkAsOnBoard();
        return true;
    }

    public IBoardSlot GetSlotOfCard(Card card)
    {
        for (int x = 0; x < BoardState.SIZE; x++)
            for (int y = 0; y < BoardState.SIZE; y++)
                if (state.GetCard(x, y) == card)
                    return new SimSlot(x, y, card);
        return null;
    }

    public IEnumerable<(int x, int y)> GetFreeSlots()
    {
        for (int x = 0; x < BoardState.SIZE; x++)
            for (int y = 0; y < BoardState.SIZE; y++)
                if (state.IsEmpty(x, y))
                    yield return (x, y);
    }
}

public class SimSlot : IBoardSlot
{
    public int X { get; }
    public int Y { get; }
    public Card Occupant { get; private set; }
    public bool IsEmpty => Occupant == null;

    public SimSlot(int x, int y, Card occupant)
    {
        X = x;
        Y = y;
        Occupant = occupant;
    }
}
