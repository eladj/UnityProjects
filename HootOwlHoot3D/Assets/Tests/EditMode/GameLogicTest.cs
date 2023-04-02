using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class GameLogicTest
{
    [Test]
    public void InitDefaultTest()
    {
        GameLogic gameLogic = new GameLogic();
        gameLogic.InitGame();
        Assert.AreEqual(GameLogicStage.Ongoing, gameLogic.Stage());
    }

    [Test]
    public void InitFromGameLogicConfigTest()
    {
        GameLogicConfig gameLogicConfig = new GameLogicConfig(4, 5, numSunCardToLose_: 9);
        GameLogic gameLogic = new GameLogic(gameLogicConfig);
        gameLogic.InitGame();
        Assert.AreEqual(GameLogicStage.Ongoing, gameLogic.Stage());
        Assert.AreEqual(4, gameLogic.GetConfig().numPlayers);
        Assert.AreEqual(5, gameLogic.GetConfig().numDragons);
        Assert.AreEqual(9, gameLogic.GetConfig().numSunCardToLose);
    }

    [Test]
    public void SunCardsMovesTest()
    {
        // Generate a deck with only Sun cards
        GameLogicConfig gameLogicConfig = new GameLogicConfig(2, 3, numColorCardsInDeck_: 0, numSunCardToLose_: 5, numSunCardsInDeck_: 40);
        GameLogic gameLogic = new GameLogic(gameLogicConfig);

        // Try to play before init
        Assert.AreEqual(GameLogicStage.Uninitialized, gameLogic.Stage());
        Assert.False(gameLogic.MakeMoveSunCard(0));
        Assert.False(gameLogic.MakeMoveSunCard(0));

        // Play after initalization
        gameLogic.InitGame();
        Assert.AreEqual(GameLogicStage.Ongoing, gameLogic.Stage());

        // Play 5 sun card and expect to lose
        for (int i = 0; i < 5; i++)
        {
            bool success = gameLogic.MakeMoveSunCard(0);
            Assert.True(success);
        }
        Assert.AreEqual(GameLogicStage.Lost, gameLogic.Stage());

        // Try to play after we already lost
        Assert.False(gameLogic.MakeMoveSunCard(0));
        Assert.False(gameLogic.MakeMoveSunCard(0));

        // Initalize again and play
        gameLogic.InitGame();
        Assert.AreEqual(GameLogicStage.Ongoing, gameLogic.Stage());
        for (int i = 0; i < 5; i++)
        {
            bool success = gameLogic.MakeMoveSunCard(0);
            Assert.True(success);
        }
        Assert.AreEqual(GameLogicStage.Lost, gameLogic.Stage());

        // Make sure all cards are sun
        List<CardType> cards = gameLogic.GetCards(0);
        foreach (CardType card in cards){
            Assert.True(card == CardType.Sun);
        }
    }

    [Test]
    public void ColorCardsMovesTest()
    {
        // Generate a deck with only color cards and a game with only single dragon,
        // so we are guarnteed to win after 40 iterations at most, given we have 40 islands.
        GameLogicConfig gameLogicConfig = new GameLogicConfig(2, 1, numColorCardsInDeck_: 10, numSunCardToLose_: 100, numSunCardsInDeck_: 0);
        GameLogic gameLogic = new GameLogic(gameLogicConfig);

        // Try to play before init
        Assert.AreEqual(GameLogicStage.Uninitialized, gameLogic.Stage());
        Assert.False(gameLogic.MakeMoveColorAndDragon(0, 0));
        Assert.False(gameLogic.MakeMoveColorAndDragon(0, 0));

        // Play after initalization
        gameLogic.InitGame();
        Assert.AreEqual(GameLogicStage.Ongoing, gameLogic.Stage());

        for (int i = 0; i < 40; i++)
        {
            bool success = gameLogic.MakeMoveColorAndDragon(0, 0);
        }
        Assert.AreEqual(GameLogicStage.Won, gameLogic.Stage());

        // Try to play after we already won
        Assert.False(gameLogic.MakeMoveSunCard(0));
        Assert.False(gameLogic.MakeMoveSunCard(0));

        // Initalize again and play
        gameLogic.InitGame();
        Assert.AreEqual(GameLogicStage.Ongoing, gameLogic.Stage());
        for (int i = 0; i < 40; i++)
        {
            bool success = gameLogic.MakeMoveColorAndDragon(0, 0);
            if (!success) break;
        }
        Assert.AreEqual(GameLogicStage.Won, gameLogic.Stage());

        // Make sure all cards are colors
        List<CardType> cards = gameLogic.GetCards(0);
        foreach (CardType card in cards){
            Assert.True(card != CardType.Sun);
        }        
    }

    [Test]
    public void IlegalMovesTest()
    {
        GameLogicConfig gameLogicConfig = new GameLogicConfig(2, 3, numCardsPerPlayer_: 4, numColorCardsInDeck_: 0, numSunCardToLose_: 5, numSunCardsInDeck_: 40);
        GameLogic gameLogic = new GameLogic(gameLogicConfig);
        gameLogic.InitGame();
        // We shouldn't have any color card
        Assert.False(gameLogic.MakeMoveColorAndDragon(0, 0));
        // There are only 3 dragons
        Assert.False(gameLogic.MakeMoveColorAndDragon(0, 3));
        // We have only 4 cards per player
        Assert.False(gameLogic.MakeMoveColorAndDragon(4, 0));

        gameLogicConfig = new GameLogicConfig(2, 3, numCardsPerPlayer_: 4, numColorCardsInDeck_: 10, numSunCardToLose_: 5, numSunCardsInDeck_: 0);
        gameLogic = new GameLogic(gameLogicConfig);
        gameLogic.InitGame();
        // We shouldn't have any sun card
        Assert.False(gameLogic.MakeMoveSunCard(0));
        // We have only 4 cards per player
        Assert.False(gameLogic.MakeMoveSunCard(4));
    }
}
