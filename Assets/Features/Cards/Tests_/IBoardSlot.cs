public interface IBoardSlot
{
    int X { get; }
    int Y { get; }
    Card Occupant { get; }
    bool IsEmpty { get; }
}
