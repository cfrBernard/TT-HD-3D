using System.Collections.Generic;
using UnityEngine;

public class RuleEngine
{
    private List<IGameRule> rules = new List<IGameRule>();

    public RuleEngine(params IGameRule[] baseRules)
    {
        rules.AddRange(baseRules);
    }

    public void Resolve(BoardManager board, int x, int y, Card playedCard)
    {
        foreach (var rule in rules)
        {
            Debug.Log("[RuleEngine] TRY RESOLVE");
            rule.Apply(board, x, y, playedCard);
        }
    }

    public void AddRule(IGameRule rule)
    {
        rules.Add(rule);
    }
}
