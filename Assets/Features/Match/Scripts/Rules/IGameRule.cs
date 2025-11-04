public interface IGameRule
{
    void Apply(IBoard board, int x, int y, Card playedCard);
}
