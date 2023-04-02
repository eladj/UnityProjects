using System.Collections;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class GameLogicException : System.Exception
{
    public GameLogicException() { }
    public GameLogicException(string message) : base(message) { }
    public GameLogicException(string message, System.Exception inner) : base(message, inner) { }
    protected GameLogicException(
        System.Runtime.Serialization.SerializationInfo info,
        System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}

public class GameLogic
{
    private GameLogicState _gameLogicState;
    private GameLogicConfig _gameLogicConfig = null;
    private IslandLogic[] _islands;

    public void SetGameLogicConfig(int numPlayers, int numDragons)
    {
        _gameLogicConfig = new GameLogicConfig(numPlayers, numDragons);
    }

    public void InitGame()
    {
        if (_gameLogicConfig == null)
        {
            System.Console.Write("Initializing GameLogicConfig with default values. If you want to change it, call to SetGameLogicConfig() first");
            _gameLogicConfig = new GameLogicConfig();
        }
        _gameLogicState = new GameLogicState(_gameLogicConfig);
        _gameLogicState.deck = GenerateDeck();
        DealCardsToPlayers();
        BuildIslandArray();
    }

    // Play a sun card with the current player
    // Returns true if the move was valid and false if it was invalid and wasn't actually done
    public bool MakeMoveSunCard(int cardIndex)
    {
        if (_gameLogicState.ended) {
            System.Console.Write("Game already ended, can't play a move");
            return false;
        }
        if (!IsCardIndexValid(cardIndex)) return false;
        CardType selectedCard = _gameLogicState.cardsPerPlayer[_gameLogicState.currentPlayerInd, cardIndex];

        // Make sure the selected card is indeed a sun
        if (selectedCard != CardType.Sun)
        {
            System.Console.Write("Card index #" + cardIndex.ToString() + " is not a sun card. Move isn't valid");
            return false;
        }
        _gameLogicState.sunCardPlayed++;
        System.Console.Write("Sun cards played until now: " + _gameLogicState.sunCardPlayed);

        if (CheckIfLose()){
            _gameLogicState.ended = true;
            _gameLogicState.victory = false;
            return true;
        }

        _gameLogicState.cardsPerPlayer[_gameLogicState.currentPlayerInd, cardIndex] = DrawCard();
        SwitchTurnToNextPlayer();
        return true;
    }

    // Make a color+dragon move with the current player.
    // Returns true if the move was valid and false if it was invalid and wasn't actually done
    public bool MakeMoveColorAndDragon(int cardIndex, int dragonIndex)
    {
        // Validate inputs
        if (_gameLogicState.ended) {
            System.Console.Write("Game already ended, can't play a move");
            return false;
        }
        if (!IsCardIndexValid(cardIndex)) return false;
        if (dragonIndex < 0 || dragonIndex >= _gameLogicConfig.numDragons)
        {
            System.Console.Write("Dragon index " + dragonIndex.ToString() + " is not valid");
            return false;
        }
        // Check that dragon isn't already at the final island
        if (IsDragonAtEnd(dragonIndex))
        {
            System.Console.Write("Dragon index " + dragonIndex.ToString() + " already at the final island. Move is not valid");
            return false;
        }
        CardType selectedCard = _gameLogicState.cardsPerPlayer[_gameLogicState.currentPlayerInd, cardIndex];
        if (selectedCard == CardType.Sun)
        {
            System.Console.Write("Selected card is of type Sun, it should be played using the function MakeMoveSunCard");
            return false;
        }
        // If player has sun card he must play it first
        List<int> sunCardIndices = SunCardIndices(_gameLogicState.currentPlayerInd);
        if (sunCardIndices.Count > 0 && !sunCardIndices.Contains(cardIndex))
        {
            System.Console.Write("Current player has sun card, so it must be played");
            return false;
        }
        // Make the actual move
        DragonLogic selectedDragon = _gameLogicState.dragons[dragonIndex];
        System.Console.Write("Player #" + _gameLogicState.currentPlayerInd.ToString() + " selected card: " + selectedCard.ToString() + " and dragon: " + dragonIndex.ToString());

        // Build a list with all the islands which already has a dragon on them
        List<int> occupiedIslandsIndices = new List<int>();
        foreach (DragonLogic dragon in _gameLogicState.dragons)
        {
            occupiedIslandsIndices.Add(dragon.islandIndex);
        }

        for (int islandIndex = selectedDragon.islandIndex + 1; islandIndex < _islands.Count(); islandIndex++)
        {
            if (IsCardTypeAndIslandTypeEqual(selectedCard, _islands[islandIndex].type)){
                if (_islands[islandIndex].type == IslandType.Final){
                    // If it is the final island, we will always move the dragon there
                    _gameLogicState.dragons[dragonIndex].islandIndex = islandIndex;
                } else if (!occupiedIslandsIndices.Contains(islandIndex) ){
                    // Island fits the card color and is not occupied by another dragon
                    _gameLogicState.dragons[dragonIndex].islandIndex = islandIndex;
                    break;
                }
            }
        }

        if (CheckIfVictory()){
            _gameLogicState.ended = true;
            _gameLogicState.victory = true;
            return true;
        }

        // Replace with new card
        _gameLogicState.cardsPerPlayer[_gameLogicState.currentPlayerInd, cardIndex] = DrawCard();
        SwitchTurnToNextPlayer();
        return true;
    }

    private bool IsCardTypeAndIslandTypeEqual(CardType card, IslandType island)
    {
        if ((card == CardType.Red && island == IslandType.Red) ||
            (card == CardType.Blue && island == IslandType.Blue) ||
            (card == CardType.Green && island == IslandType.Green) ||
            (card == CardType.Orange && island == IslandType.Orange) ||
            (card == CardType.Purple && island == IslandType.Purple) ||
            (card == CardType.Yellow && island == IslandType.Yellow)) return true;
        return false;
    }

    private bool CheckIfVictory(){
        foreach (DragonLogic dragon in _gameLogicState.dragons){
            if (_islands[dragon.islandIndex].type != IslandType.Final) return false;
        }
        return true;
    }

    private bool CheckIfLose(){
        if (_gameLogicState.sunCardPlayed >= _gameLogicConfig.numSunCardToLose) return true;
        return false;
    }

    private bool IsCardIndexValid(int cardIndex)
    {
        if (cardIndex < 0 || cardIndex >= _gameLogicConfig.numCardsPerPlayer)
        {
            System.Console.Write("Card index " + cardIndex.ToString() + " is not valid");
            return false;
        }
        return true;
    }

    private void SwitchTurnToNextPlayer()
    {
        _gameLogicState.currentPlayerInd++;
        if (_gameLogicState.currentPlayerInd >= _gameLogicConfig.numPlayers)
        {
            _gameLogicState.currentPlayerInd = 0;
        }
        System.Console.Write("Switched turn to player #" + _gameLogicState.currentPlayerInd);
    }

    private bool IsDragonAtEnd(int dragonIndex)
    {
        return _islands[_gameLogicState.dragons[dragonIndex].islandIndex].type == IslandType.Final ? true : false;
    }

    // Returns the indices of the sun cards
    private List<int> SunCardIndices(int playerIndex)
    {
        List<int> sunCardIndices = new List<int>();
        for (int cardIndex = 0; cardIndex < _gameLogicConfig.numCardsPerPlayer; cardIndex++)
        {
            if (_gameLogicState.cardsPerPlayer[playerIndex, cardIndex] == CardType.Sun)
            {
                sunCardIndices.Add(cardIndex);
            }
        }
        return sunCardIndices;
    }

    // Give each one of the players their initial cards
    private void DealCardsToPlayers()
    {
        for (int playerInd = 0; playerInd < _gameLogicConfig.numPlayers; playerInd++)
        {
            for (int cardInd = 0; cardInd < _gameLogicConfig.numCardsPerPlayer; cardInd++)
            {
                _gameLogicState.cardsPerPlayer[playerInd, cardInd] = DrawCard();
            }
        }
    }

    // Returns a single card out of the deck
    private CardType DrawCard()
    {
        if (_gameLogicState.deck.Count == 0)
        {
            System.Console.Write("Deck is empty, can't draw a card");
        }
        CardType result = _gameLogicState.deck.Last();
        _gameLogicState.deck.RemoveAt(_gameLogicState.deck.Count - 1);
        return result;
    }

    // Returns a shuffled list of cards
    private List<CardType> GenerateDeck()
    {
        // Total of 50 cards. 14 sun cards, 36 color cards (six each in six colors) 
        List<CardType> deck = new List<CardType>();
        foreach (CardType cardType in System.Enum.GetValues(typeof(CardType)))
        {
            int numIters = 6;
            if (cardType == CardType.Sun) numIters = 14;
            for (int i = 0; i < numIters; i++)
            {
                deck.Add(cardType);
            }
        }
        deck = ShuffleList(deck);
        return deck;
    }

    // Fisher-Yates Shuffle Algorithm
    private List<T> ShuffleList<T>(List<T> listToShuffle)
    {
        System.Random rand = new System.Random();
        for (int i = listToShuffle.Count - 1; i > 0; i--)
        {
            var k = rand.Next(i + 1);
            var value = listToShuffle[k];
            listToShuffle[k] = listToShuffle[i];
            listToShuffle[i] = value;
        }
        return listToShuffle;
    }

    private IslandLogic[] BuildIslandArray(){
        IslandType[] islandTypes = new IslandType[]{
            IslandType.Yellow, IslandType.Green, IslandType.Orange, IslandType.Blue, IslandType.Purple, IslandType.Red, 
            IslandType.Blue, IslandType.Purple, IslandType.Red, IslandType.Yellow, IslandType.Green, IslandType.Blue,
            IslandType.Orange, IslandType.Red, IslandType.Purple, IslandType.Yellow, IslandType.Green, IslandType.Orange,
            IslandType.Blue, IslandType.Purple, IslandType.Red, IslandType.Green, IslandType.Yellow, IslandType.Orange,
            IslandType.Blue, IslandType.Purple, IslandType.Red, IslandType.Yellow, IslandType.Green, IslandType.Blue, 
            IslandType.Orange, IslandType.Red, IslandType.Purple, IslandType.Yellow, IslandType.Green, IslandType.Blue,
            IslandType.Orange, IslandType.Red, IslandType.Purple, IslandType.Final  
        };
        IslandLogic[] res = new IslandLogic[islandTypes.Length];
        for (int i=0; i < islandTypes.Length; i++){
            res[i].index = i;  // TODO: Remove this index, not needed
            res[i].type = islandTypes[i];
        }
        return res;
    }
}
