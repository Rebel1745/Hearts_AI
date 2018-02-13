using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Card {

    public string Name;
    public Sprite CardFace;
    public Sprite CardBack;
    public int ScoreValue;    
    public SUIT Suit;
    public int CardNumber;
    public int PlayerId;
    public bool Selected = false;
    public enum CARD_STATE { IN_HAND, IN_PLAY, SCORED };
    public CARD_STATE card_state;
    public int CardId;
    public bool isLegal = false;

    public bool IsTwoOfClubs()
    {
        if (Suit == SUIT.CLUBS && CardNumber == 2)
        {
            return true;
        }

        return false;
    }

    public bool IsQueenOfSpades()
    {
        if (Suit == SUIT.SPADES && CardNumber == 12)
        {
            return true;
        }

        return false;
    }

    public void SetLegality(bool legal)
    {
        isLegal = legal;
    }
}
