public class GameLogicConfig
{
    public int numPlayers { get; private set; }
    public int numDragons { get; private set; }
    public int numCardsPerPlayer { get; private set; }
    public int numSunCardToLose { get; private set; }
    public int numColorCardsInDeck { get; private set; }
    public int numSunCardsInDeck { get; private set; }

    public GameLogicConfig()
    {
        numPlayers = 2;
        numDragons = 3;
        numCardsPerPlayer = 3;
        numSunCardToLose = 13;
        numColorCardsInDeck = 6;
        numSunCardsInDeck = 14;
    }

    public GameLogicConfig(int numPlayers_, int numDragons_, int numCardsPerPlayer_ = 3, int numSunCardToLose_ = 13, int numColorCardsInDeck_ = 6, int numSunCardsInDeck_ = 14)
    {
        if (numPlayers_ < 1 || numPlayers_ > 4){
            throw new System.ArgumentException("numPlayers_ needs to be between 1 to 4");
        }
        if (numDragons_ < 1 || numDragons_ > 6){
            throw new System.ArgumentException("numDragons_ needs to be between 1 to 6");
        }
        if (numCardsPerPlayer_ < 1){
            throw new System.ArgumentException("numCardsPerPlayer_ needs to be larger than 0");
        }       
        numPlayers = numPlayers_;
        numDragons = numDragons_;
        numCardsPerPlayer = numCardsPerPlayer_;
        numSunCardToLose = numSunCardToLose_;
        numColorCardsInDeck = numColorCardsInDeck_;
        numSunCardsInDeck = numSunCardsInDeck_;
    }
}