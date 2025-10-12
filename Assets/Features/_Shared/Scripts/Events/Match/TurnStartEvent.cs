public struct TurnStartEvent
{
    public Player CurrentPlayer;

    public TurnStartEvent(Player currentPlayer)
    {
        CurrentPlayer = currentPlayer;
    }
}
