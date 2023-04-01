using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardDisplay : MonoBehaviour
{
    public Card card;
    public Image artworkImage;
    public int cardIndex;
    public bool selected;
    private Vector3 originalScale;

    void Start()
    {
        originalScale = transform.GetComponent<RectTransform>().localScale;
    }

    public void UpdateImage()
    {
        Debug.Log("artworkImage.sprite: " + artworkImage.sprite.ToString());
        Debug.Log("card.artwork: " + card.artwork.ToString());
        artworkImage.sprite = card.artwork;
    }

    public void Click()
    {
        if (!selected)
        {
            selected = true;
            transform.GetComponent<RectTransform>().localScale *= 1.2f;
        } else {
            selected = false;
            transform.GetComponent<RectTransform>().localScale = originalScale;
        }
    }

    public void Deselect()
    {
        if (selected)
        {
            selected = false;
            transform.GetComponent<RectTransform>().localScale = originalScale;
        }
    }

    // void OnSelect(){
    //     transform.GetComponent<Button>().
    //     selected = true;
    // }

    // void OnDeselect(){
    //     selected = false;
    // }

    // void OnMouseDown()
    // {
    //     artworkImage.transform.position += Vector3.up * 50;
    //     // TODO
    // }    

    // void OnMouseUp()
    // {
    //     artworkImage.transform.position += Vector3.down * 50;
    //     // TODO
    // }        
}
