using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Player {
    
    public string PlayerName;
    public Card[] Cards = new Card[13];
    public int Score;
    public Transform CardSpawn;
    public Transform CardInPlay;
    public List<Card> ScoredCards = new List<Card>();
    public Vector3 OffsetPerCard;
    public Vector3 SelectedCardOffset;
}
