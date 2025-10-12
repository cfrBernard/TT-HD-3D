public struct TurnEndEvent
{
    public Player CurrentPlayer;

    public TurnEndEvent(Player currentPlayer)
    {
        CurrentPlayer = currentPlayer;
    }
}
