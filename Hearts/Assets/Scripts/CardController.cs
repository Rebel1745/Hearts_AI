using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardController : MonoBehaviour {

    private void Start()
    {
        cm = GameObject.FindObjectOfType<CardManager>();
    }

    CardManager cm;

    public Card card;

    private void OnMouseDown()
    {
        cm.OnCardSelected(card);
    }
}
