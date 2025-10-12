public struct MatchEndEvent
{
    public string Result; // "win" | "draw" | "lose"
    public int Coins;
    public int Xp;

    public MatchEndEvent(string result, int coins, int xp)
    {
        Result = result;
        Coins = coins;
        Xp = xp;
    }
}
