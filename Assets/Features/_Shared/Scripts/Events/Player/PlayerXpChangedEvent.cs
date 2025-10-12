public struct PlayerXpChangedEvent
{
    public int NewXp;
    public int NewLevel;

    public PlayerXpChangedEvent(int newXp, int newLevel)
    {
        NewXp = newXp;
        NewLevel = newLevel;
    }
}
