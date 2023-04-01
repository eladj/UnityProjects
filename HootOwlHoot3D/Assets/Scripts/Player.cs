using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public GameObject[] cardsGameObjects = new GameObject[3];
    public Card[] cardsScriptableObjects;  // This should follow the CardType enum numbers
    public Image frame;
    // public EventSystem eventSystem;
    private Dictionary<GameManager.CardType, Card> cardTypeToScriptableObject;


    void Start()
    {
        buildDict();
    }

    public void SetCardUI(int cardIndex, GameManager.CardType cardType)
    {
        CardDisplay cardDisplay = cardsGameObjects[cardIndex].GetComponent<CardDisplay>();
        cardDisplay.card = cardTypeToScriptableObject[cardType];
        cardDisplay.UpdateImage();
    }

    public CardDisplay SelectedCard(){
        foreach (GameObject cardGO in cardsGameObjects)
        {
            if (cardGO.GetComponent<CardDisplay>().selected){
                return cardGO.GetComponent<CardDisplay>();
            }
        }
        return null;
    }

    public void SetHighlightFrame(bool value)
    {
        frame.gameObject.SetActive(value);
    }

    public void SetInteractable(bool value)
    {
        foreach (GameObject cardGO in cardsGameObjects)
        {
            cardGO.GetComponent<Button>().interactable = value;
        }
    }

    private void buildDict()
    {
        cardTypeToScriptableObject = new Dictionary<GameManager.CardType, Card>();
        foreach (int cardValue in System.Enum.GetValues(typeof(GameManager.CardType)))
        {
            cardTypeToScriptableObject.Add((GameManager.CardType)cardValue, cardsScriptableObjects[cardValue]);
        }
    }

    public void DeselectAllCards(){
        foreach (GameObject cardGO in cardsGameObjects)
        {
            cardGO.GetComponent<CardDisplay>().Deselect();
        }
    }
    
    // public GameManager.CardType GetSelectedCard()
    // {
    //     GameManager.CardType cardType = EventSystem.current.currentSelectedGameObject.GetComponent<CardDisplay>().card.cardType;
    //     Debug.Log("Selected card: " + cardType.ToString());
    //     return cardType;

    //     // foreach (GameObject cardGO in cardsGameObjects)
    //     // {
    //     //     if (EventSystem.current.currentSelectedGameObject == cardGO)
    //     //     {
    //     //         GameManager.CardType cardType = cardGO.GetComponent<CardDisplay>().card.cardType;
    //     //         Debug.Log("Selected card: " + cardType.ToString());
    //     //         return cardType;
    //     //     }
    //     // }
    // }
}
