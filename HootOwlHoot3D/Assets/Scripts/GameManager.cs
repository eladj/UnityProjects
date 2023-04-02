using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Linq;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static UnityEvent dragonReachedPosition;

    public int currentTurn;
    public int maxTurns;
    [Range(2, 4)] public int numPlayers;
    [Range(2, 6)] public int numDragons;
    public GameObject[] playersUI = new GameObject[4];
    public GameObject instructionsUI;
    public GameObject islandsRoot;
    public GameObject dragonsRoot;

    private GameLogicConfig gameLogicConfig;
    private GameLogic gameLogic;

    private int currentPlayerInd;
    private const int numCardsPerPlayer = 3;
    private List<CardType> deck;
    private List<List<CardType>> cardsPerPlayer;
    private Island[] islands;
    private Dragon[] dragons;

    private void Awake()
    {
        dragonReachedPosition = new UnityEvent();
        dragonReachedPosition.AddListener(_OnDragonReachedPosition);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Mouse clicked");
            Vector3 mousePosition = Input.mousePosition;
            Ray ray = Camera.main.ScreenPointToRay(mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Debug.Log("Raycast hit detected");
                Dragon dragon = hit.collider.gameObject.GetComponent<Dragon>();
                if (dragon != null)
                {
                    DragonSelected(dragon);
                }
            }
        }
    }

    void Start()
    {
        islands = BuildIslandList();
        dragons = BuildDragonList();
        InitGame();
        StartTurn();
    }

    private void InitGame()
    {
        // TODO: Set this from UI menu
        gameLogicConfig = new GameLogicConfig(numPlayers_: 2, numDragons_: 3);
        gameLogic = new GameLogic(gameLogicConfig);

        cardsPerPlayer = new List<List<CardType>>();
        deck = new List<CardType>();
        deck = GenerateDeck();
        currentPlayerInd = 0;
        DealCardsToPlayers();
        UpdatePlayersUI();
        InitDragons();
    }

    private void SwitchTurnToNextPlayer()
    {
        Debug.Log("Switching turn to player #" + currentPlayerInd.ToString());

        // Disable last player UI
        GetCurrentPlayer().SetHighlightFrame(false);
        GetCurrentPlayer().SetInteractable(false);

        // Advance to new player
        currentPlayerInd++;
        if (currentPlayerInd >= numPlayers)
        {
            currentPlayerInd = 0;
        }

        // Turn on new player UI
        GetCurrentPlayer().SetHighlightFrame(true);
        GetCurrentPlayer().SetInteractable(true);
    }

    // Set the initial position of the dragons on the islands
    private void InitDragons()
    {
        // First dragon is placed on the 5-th island, and from there we go beckwards
        for (int dragonInd = 0, islandInd = 5; dragonInd < numDragons; dragonInd++, islandInd--)
        {
            dragons[dragonInd].gameObject.SetActive(true);
            Debug.Log("Set dragon # " + dragonInd + " position to island #" + islandInd + " : " + islands[islandInd].Position().ToString());
            dragons[dragonInd].transform.position = islands[islandInd].Position();
            dragons[dragonInd].index = dragonInd;
            dragons[dragonInd].curIslandIndex = islandInd;
            dragons[dragonInd].SetInteractable(true);
        }

        // Disable all remaining dragons
        for (int dragonInd = numDragons; dragonInd < dragons.Count(); dragonInd++)
        {
            dragons[dragonInd].gameObject.SetActive(false);
        }
    }

    private void StartTurn()
    {
        Player curPlayer = GetCurrentPlayer();
        curPlayer.SetHighlightFrame(true);
        curPlayer.SetInteractable(true);
        int sunCardIndex = SunCardIndex(currentPlayerInd);
        if (sunCardIndex >= 0)
        {
            SetInstructionsText("Sun card was played");
        }
        else
        {
            SetInstructionsText("Select a color card");
        }
    }

    private void _OnDragonReachedPosition()
    {
        int currentSelectedCardInd = GetCurrentPlayer().SelectedCard().cardIndex;
        CardType newCard = DrawCard();
        GetCurrentPlayer().SetCardUI(currentSelectedCardInd, newCard);
        cardsPerPlayer[currentPlayerInd][currentSelectedCardInd] = newCard;
        GetCurrentPlayer().DeselectAllCards();
        SwitchTurnToNextPlayer();
        currentTurn++;
    }

    public void OnCardClicked()
    {
        Player curPlayer = GetCurrentPlayer();
        CardDisplay selectedCard = EventSystem.current.currentSelectedGameObject.GetComponent<CardDisplay>();
        CardType cardType = selectedCard.card.cardType;
        Debug.Log("Selected card: " + cardType.ToString() + ". Please select a dragon to move");
        curPlayer.DeselectAllCards();
        selectedCard.Click();
    }

    public void DragonSelected(Dragon dragon)
    {
        Debug.Log("Dragon #" + dragon.index.ToString() + " was selected.");
        //TODO: Find next available island. Move dragon to this island
        if (GetCurrentPlayer().SelectedCard() == null)
        {
            SetInstructionsText("You must select a card before selecting a dragon");
        }
        for (int islandIndex = dragon.curIslandIndex + 1; islandIndex < islands.Count(); islandIndex++)
        {
            Debug.Log("Check island #" + islandIndex.ToString());
            string islandStr = islands[islandIndex].IslandColor();
            string cardStr = GetCurrentPlayer().SelectedCard().card.cardType.ToString();
            if (islandStr == cardStr)
            {
                // Check that island isn't occupied by any other dragon
                bool occupied = false;
                for (int otherDragonIndex = 0; otherDragonIndex < numDragons; otherDragonIndex++)
                {
                    if (otherDragonIndex == dragon.index) continue;
                    if (dragons[otherDragonIndex].curIslandIndex == islandIndex)
                    {
                        occupied = true;
                    }
                }
                if (!occupied)
                {
                    MoveDragon(dragon.index, islandIndex);
                    return;
                }
            }
        }
        // Move to final island
        MoveDragon(dragon.index, islands.Last().IslandIndex());
        dragons[dragon.index].SetInteractable(false);
    }

    private void MoveDragon(int dragonIndex, int islandIndex)
    {
        Debug.Log("Move dragon #" + dragonIndex + " to island #" + islandIndex + ". Position: " + islands[islandIndex].Position().ToString());
        // dragons[dragonIndex].transform.position = islands[islandIndex].Position();
        List<GameObject> islandsPath = new List<GameObject>();
        for (int i = dragons[dragonIndex].curIslandIndex; i <= islandIndex; i++)
        {
            islandsPath.Add(islands[i].gameObject);
        }
        dragons[dragonIndex].MoveInPath(islandsPath);
        dragons[dragonIndex].curIslandIndex = islandIndex;
    }

    private int SunCardIndex(int playerIndex)
    {
        int sunCardIndex = -1;
        for (int i = 0; i < cardsPerPlayer[playerIndex].Count(); i++)
        {
            if (cardsPerPlayer[playerIndex][i] == CardType.Sun)
            {
                sunCardIndex = i;
            }
        }
        return sunCardIndex;
    }

    private void SetInstructionsText(string s)
    {
        instructionsUI.GetComponent<TMPro.TMP_Text>().text = s;
    }

    private void UpdatePlayersUI()
    {
        for (int playerIndex = 0; playerIndex < playersUI.Count(); playerIndex++)
        {
            if (playerIndex >= numPlayers)
            {
                playersUI[playerIndex].SetActive(false);
                continue;
            }

            Player curPlayer = playersUI[playerIndex].GetComponent<Player>();
            curPlayer.SetHighlightFrame(false);
            curPlayer.SetInteractable(false);
            for (int cardIndex = 0; cardIndex < numCardsPerPlayer; cardIndex++)
            {
                curPlayer.SetCardUI(cardIndex, cardsPerPlayer[playerIndex][cardIndex]);
                Debug.Log("Player #" + playerIndex.ToString() + ", Card: " + cardsPerPlayer[playerIndex][cardIndex].ToString());
            }


        }
    }

    // Give each one of the players their initial cards
    private void DealCardsToPlayers()
    {
        for (int playerInd = 0; playerInd < numPlayers; playerInd++)
        {
            List<CardType> cards = new List<CardType>();
            for (int cardInd = 0; cardInd < numCardsPerPlayer; cardInd++)
            {
                cards.Add(DrawCard());
            }
            cardsPerPlayer.Add(cards);
        }
    }

    // Returns a single card out of the deck
    private CardType DrawCard()
    {
        if (deck.Count() == 0)
        {
            Debug.LogError("Deck is empty, can't draw a card");
        }
        CardType result = deck.Last();
        deck.RemoveAt(deck.Count() - 1);
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
        // Shuffle
        deck = deck.OrderBy(x => Random.value).ToList();
        return deck;
    }

    private Island[] BuildIslandList()
    {
        Island[] islandsOut = new Island[islandsRoot.transform.childCount];
        Island[] islandsInChildren = islandsRoot.GetComponentsInChildren<Island>();
        if (islandsInChildren.Count() != islandsOut.Count())
        {
            Debug.LogError("Island number mismatch");
        }
        // GameObject[] islandsArray = new GameObject[islandsRoot.transform.childCount];
        foreach (Island child in islandsInChildren)
        {
            islandsOut[child.IslandIndex()] = child;
        }
        Debug.Log("Found " + islandsOut.Count().ToString() + " islands.");
        return islandsOut;
    }

    private Dragon[] BuildDragonList()
    {
        Dragon[] dragonsOut = new Dragon[dragonsRoot.transform.childCount];
        Dragon[] dragonsInChildren = dragonsRoot.GetComponentsInChildren<Dragon>();
        if (dragonsInChildren.Count() != dragonsOut.Count())
        {
            Debug.LogError("Dragons number mismatch");
        }
        foreach (Dragon child in dragonsInChildren)
        {
            dragonsOut[child.index] = child;
        }
        Debug.Log("Found " + dragonsOut.Count().ToString() + " dragons.");
        return dragonsOut;
    }

    private Player GetCurrentPlayer()
    {
        return playersUI[currentPlayerInd].GetComponent<Player>();
    }    
}
