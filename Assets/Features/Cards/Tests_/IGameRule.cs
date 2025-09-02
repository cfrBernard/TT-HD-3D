public interface IGameRule
{
    void Apply(BoardManager board, int x, int y, Card playedCard);
}
