using NUnit.Framework;
using System.Collections.Generic;

public class TripleTriadRuleTests
{
    private BoardManager board;
    private Player player1;
    private Player player2;

    [SetUp]
    public void Setup()
    {
        board = new BoardManager();
        board.Awake(); // Init des slots

        player1 = new Player("P1");
        player2 = new Player("P2");
    }

    [Test]
    public void SameRule_FlipsOnlyIfTwoOrMoreMatches()
    {
        var cardA = new Card(new CardData { north = 2, east = 3, south = 1, west = 4 }, player1);
        var cardB = new Card(new CardData { south = 2 }, player2);
        var cardC = new Card(new CardData { north = 2 }, player2);

        board.TryPlaceCard(1, 1, cardA);
        board.TryPlaceCard(1, 0, cardB);
        board.TryPlaceCard(1, 2, cardC);

        var sameRule = new SameRule();
        var flipped = new List<Card>();
        sameRule.Apply(board, 1, 1, cardA, flipped);

        Assert.AreEqual(2, flipped.Count);
        Assert.AreEqual(player1, cardB.Owner);
        Assert.AreEqual(player1, cardC.Owner);
    }

    [Test]
    public void SameRule_DoesNotFlipIfOnlyOneMatch()
    {
        var cardA = new Card(new CardData { north = 2 }, player1);
        var cardB = new Card(new CardData { south = 2 }, player2);

        board.TryPlaceCard(1, 1, cardA);
        board.TryPlaceCard(1, 0, cardB);

        var sameRule = new SameRule();
        var flipped = new List<Card>();
        sameRule.Apply(board, 1, 1, cardA, flipped);

        Assert.AreEqual(0, flipped.Count);
        Assert.AreEqual(player2, cardB.Owner);
    }

    [Test]
    public void PlusRule_FlipsOnlyIfTwoOrMoreSumsMatch()
    {
        var cardA = new Card(new CardData { north = 1, east = 3, south = 2, west = 5 }, player1);
        var cardB = new Card(new CardData { south = 8 }, player2); // 1+8=9
        var cardC = new Card(new CardData { north = 6 }, player2); // 3+6=9

        board.TryPlaceCard(1, 1, cardA);
        board.TryPlaceCard(1, 0, cardB);
        board.TryPlaceCard(2, 1, cardC);

        var plusRule = new PlusRule();
        var flipped = new List<Card>();
        plusRule.Apply(board, 1, 1, cardA, flipped);

        Assert.AreEqual(2, flipped.Count);
        Assert.AreEqual(player1, cardB.Owner);
        Assert.AreEqual(player1, cardC.Owner);
    }

    [Test]
    public void PlusRule_DoesNotFlipIfOnlyOneSumMatch()
    {
        var cardA = new Card(new CardData { north = 1 }, player1);
        var cardB = new Card(new CardData { south = 8 }, player2); // 1+8=9

        board.TryPlaceCard(1, 1, cardA);
        board.TryPlaceCard(1, 0, cardB);

        var plusRule = new PlusRule();
        var flipped = new List<Card>();
        plusRule.Apply(board, 1, 1, cardA, flipped);

        Assert.AreEqual(0, flipped.Count);
        Assert.AreEqual(player2, cardB.Owner);
    }

    [Test]
    public void ComboRule_AppliesBasicCaptureOnFlippedCards()
    {
        var basic = new BasicCaptureRule();
        var combo = new ComboRule(null, basic);

        var cardA = new Card(new CardData { north = 5, east = 3, south = 2, west = 1 }, player1);
        var cardB = new Card(new CardData { south = 4 }, player2);

        board.TryPlaceCard(1, 1, cardA);
        board.TryPlaceCard(1, 0, cardB);

        // simulate flipped card
        var flipped = new List<Card> { cardB };

        combo.Apply(board, flipped);

        Assert.AreEqual(player1, cardB.Owner);
    }

    [Test]
    public void BasicCaptureRule_FlipsIfGreater()
    {
        var cardA = new Card(new CardData { north = 5 }, player1);
        var cardB = new Card(new CardData { south = 3 }, player2);

        board.TryPlaceCard(1, 1, cardA);
        board.TryPlaceCard(1, 0, cardB);

        var basic = new BasicCaptureRule();
        basic.Apply(board, 1, 1, cardA);

        Assert.AreEqual(player1, cardB.Owner);
    }
}
