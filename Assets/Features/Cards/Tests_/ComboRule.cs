using System.Collections.Generic;

public class ComboRule // applique BasicCapture sur toutes les cartes flipées par Same ou Plus
{
    private RuleEngine engine;
    private BasicCaptureRule basicRule;

    public ComboRule(RuleEngine engine, BasicCaptureRule basic)
    {
        this.engine = engine;
        this.basicRule = basic;
    }

    public void Apply(BoardManager board, List<Card> flippedThisTurn)
    {
        foreach (var card in flippedThisTurn)
        {
            var slot = board.GetSlotOfCard(card);
            if (slot != null)
                basicRule.Apply(board, slot.X, slot.Y, card);
        }
    }
}
