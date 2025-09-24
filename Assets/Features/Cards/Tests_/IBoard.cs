using System.Collections.Generic;

public interface IBoard
{
    IBoardSlot GetSlot(int x, int y);
    IEnumerable<IBoardSlot> GetAllSlots();
    bool TryPlaceCard(int x, int y, Card card);
    IBoardSlot GetSlotOfCard(Card card);
    IEnumerable<(int x, int y)> GetFreeSlots();
}
