using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Gère l'application des règles avec le flow classique Triple Triad:
/// Same → Plus → Combo → BasicCapture
/// </summary>
public class RuleEngine
{
    public bool enableSame = true;
    public bool enablePlus = true;
    public bool enableCombo = true;
    public bool enableBasic = true;

    private SameRule sameRule;
    private PlusRule plusRule;
    private ComboRule comboRule;
    private BasicCaptureRule basicRule;

    // Liste des cartes flip par Same ou Plus pour Combo
    private List<Card> flippedThisTurn = new List<Card>();

    public RuleEngine()
    {
        basicRule = new BasicCaptureRule();
        sameRule = new SameRule();
        plusRule = new PlusRule();
        comboRule = new ComboRule(this, basicRule);
    }

    public void Resolve(BoardManager board, int x, int y, Card playedCard)
    {
        flippedThisTurn.Clear();

        // 1. Same
        if (enableSame)
            sameRule.Apply(board, x, y, playedCard, flippedThisTurn);

        // 2. Plus
        if (enablePlus)
            plusRule.Apply(board, x, y, playedCard, flippedThisTurn);

        // 3. Combo (applique Basic sur les cartes flipées)
        if (enableCombo)
            comboRule.Apply(board, flippedThisTurn);

        // 4. BasicCapture sur la carte jouée
        if (enableBasic && flippedThisTurn.Count == 0)
            basicRule.Apply(board, x, y, playedCard);

        // Fin du tour: vide le tracking
        flippedThisTurn.Clear();
    }
}
