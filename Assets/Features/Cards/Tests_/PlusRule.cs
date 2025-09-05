using System;
using UnityEngine;
using System.Collections.Generic;

public class PlusRule // flip les cartes si les sommes sont égales
{
    public void Apply(BoardManager board, int x, int y, Card playedCard, List<Card> flippedThisTurn)
    {
        var directions = new (int dx, int dy, Func<Card, int> myVal, Func<Card, int> otherVal)[]
        {
            (0, -1, c => c.Data.north, c => c.Data.south),
            (0, 1,  c => c.Data.south, c => c.Data.north),
            (1, 0,  c => c.Data.east,  c => c.Data.west),
            (-1,0,  c => c.Data.west,  c => c.Data.east)
        };

        var sums = new Dictionary<int, List<Card>>();
        foreach (var (dx, dy, myVal, otherVal) in directions)
        {
            int nx = x + dx, ny = y + dy;
            if (nx < 0 || nx >= BoardManager.SIZE || ny < 0 || ny >= BoardManager.SIZE) continue;

            var neighborSlot = board.GetSlot(nx, ny);
            if (neighborSlot.IsEmpty || neighborSlot.Occupant.Owner == playedCard.Owner) continue;

            int sum = myVal(playedCard) + otherVal(neighborSlot.Occupant);
            if (!sums.ContainsKey(sum)) sums[sum] = new List<Card>();
            sums[sum].Add(neighborSlot.Occupant);
        }

        foreach (var pair in sums)
        {
            if (pair.Value.Count >= 2)
            {
                foreach (var card in pair.Value)
                {
                    card.SetOwner(playedCard.Owner);
                    flippedThisTurn.Add(card);
                    Debug.Log($"[PlusRule] {card.Data.name} flipped by {playedCard.Data.name}");
                }
            }
        }
    }
}
