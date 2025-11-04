using System.Collections.Generic;
using UnityEngine;

public class RuleEngine // Applique les règles avec le flow Triple Triad
{
    public bool enableSame = true;
    public bool enablePlus = true;
    public bool enableCombo = true;
    public bool enableCoinToss = true;

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

    public void LoadFromSettings(SettingsManager sm)
    {
        enableSame = sm.GetSetting<bool>("rules", "same");
        enablePlus = sm.GetSetting<bool>("rules", "plus");
        enableCombo = sm.GetSetting<bool>("rules", "combo");
        enableCoinToss = sm.GetSetting<bool>("rules", "coinToss");
        
        // --- Debug ---
        string logMessage = "Active rules : ";
        if (enableSame)  logMessage += "Same, ";
        if (enablePlus)  logMessage += "Plus, ";
        if (enableCombo) logMessage += "Combo, ";
        if (enableCoinToss) logMessage += "Coin Toss, ";
        if (logMessage == "Active rules : ")
            Debug.Log("No rules enabled.");
        else
            Debug.Log(logMessage.TrimEnd(',', ' '));
    }

    // Flow Triple Triad : Same -> Plus -> Combo -> BasicCapture
    public void Resolve(IBoard board, int x, int y, Card playedCard)
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
        if (flippedThisTurn.Count == 0)
            basicRule.Apply(board, x, y, playedCard);

        // Fin du tour: vide le tracking
        flippedThisTurn.Clear();
    }
}
