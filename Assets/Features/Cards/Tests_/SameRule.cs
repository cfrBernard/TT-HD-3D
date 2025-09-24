using System;
using UnityEngine;
using System.Collections.Generic;

public class SameRule // flip les cartes dont le chiffre correspond à l'opposé de la carte jouée
{
    public void Apply(IBoard board, int x, int y, Card playedCard, List<Card> flippedThisTurn)
    {
        var directions = new (int dx, int dy, Func<Card, int> myVal, Func<Card, int> otherVal)[]
        {
            (0, -1, c => c.Data.north, c => c.Data.south),
            (0, 1,  c => c.Data.south, c => c.Data.north),
            (1, 0,  c => c.Data.east,  c => c.Data.west),
            (-1,0,  c => c.Data.west,  c => c.Data.east)
        };

        var matchingNeighbors = new List<Card>();

        foreach (var (dx, dy, myVal, otherVal) in directions)
        {
            int nx = x + dx, ny = y + dy;
            if (nx < 0 || nx >= BoardManager.SIZE || ny < 0 || ny >= BoardManager.SIZE) continue;

            var neighborSlot = board.GetSlot(nx, ny);
            if (neighborSlot.IsEmpty || neighborSlot.Occupant.Owner == playedCard.Owner) continue;

            if (myVal(playedCard) == otherVal(neighborSlot.Occupant))
                matchingNeighbors.Add(neighborSlot.Occupant);
        }

        // Only flip if 2 or more matches
        if (matchingNeighbors.Count >= 2)
        {
            foreach (var card in matchingNeighbors)
            {
                card.SetOwner(playedCard.Owner);
                flippedThisTurn.Add(card);
                Debug.Log($"[SameRule] {card.Data.name} flipped by {playedCard.Data.name}");
            }
        }
    }
}
