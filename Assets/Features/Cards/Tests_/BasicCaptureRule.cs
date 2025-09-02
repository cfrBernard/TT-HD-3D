using System;
using UnityEngine;

public class BasicCaptureRule : IGameRule
{
    public void Apply(BoardManager board, int x, int y, Card playedCard)
    {
        var directions = new (int dx, int dy, Func<Card,int> myValue, Func<Card,int> otherValue)[]
        {
            (0, -1, c => c.Data.north,  c => c.Data.south),
            (0, 1,  c => c.Data.south,  c => c.Data.north),
            (1, 0,  c => c.Data.east,   c => c.Data.west),
            (-1,0,  c => c.Data.west,   c => c.Data.east),
        };

        foreach (var (dx, dy, myVal, otherVal) in directions)
        {
            var nx = x + dx;
            var ny = y + dy;

            // skip si hors board
            if (nx < 0 || nx >= BoardManager.SIZE || ny < 0 || ny >= BoardManager.SIZE)
                continue;

            var neighborSlot = board.GetSlot(nx, ny);
            if (neighborSlot == null || neighborSlot.IsEmpty) continue;

            var neighborCard = neighborSlot.Occupant;
            if (neighborCard.Owner == playedCard.Owner) continue;

            Debug.Log($"Compare {playedCard.Data.name} ({myVal(playedCard)}) vs {neighborCard.Data.name} ({otherVal(neighborCard)}) at ({nx},{ny})");
            Debug.Log($"PlayedCard: {playedCard.Data.name} | N:{playedCard.Data.north} S:{playedCard.Data.south} E:{playedCard.Data.east} W:{playedCard.Data.west}");
            Debug.Log($"NeighborCard: {neighborCard.Data.name} | N:{neighborCard.Data.north} S:{neighborCard.Data.south} E:{neighborCard.Data.east} W:{neighborCard.Data.west}");
            Debug.Log($"Direction: dx={dx}, dy={dy} → Slot({nx},{ny})");

            if (myVal(playedCard) > otherVal(neighborCard))
            {
                neighborCard.SetOwner(playedCard.Owner);

                Debug.Log($"[Rule] {neighborCard.Data.name} captured by {playedCard.Data.name}");
            }
        }
    }
}
