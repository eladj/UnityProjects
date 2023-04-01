using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Card")]
public class Card : ScriptableObject
{
    public GameManager.CardType cardType;
    public Sprite artwork;

    public void Print(){
        Debug.Log("CardType: " + cardType.ToString());
    }
}
