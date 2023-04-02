using System.Collections;
using System.Collections.Generic;

class GameLogicState
{
    public List<CardType> deck;
    public CardType[,] cardsPerPlayer;
    public DragonLogic[] dragons;
    public int currentPlayerInd;
    public int sunCardPlayed;
    public bool ended;
    public bool victory;

    public GameLogicState(GameLogicConfig config)
    {
        Init(config);
    }

    public void Init(GameLogicConfig config)
    {
        cardsPerPlayer = new CardType[config.numPlayers, config.numCardsPerPlayer];
        dragons = new DragonLogic[config.numDragons];
        currentPlayerInd = 0;
        sunCardPlayed = 0;
        ended = false;
        victory = false;
    }
}