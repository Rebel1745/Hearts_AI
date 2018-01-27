using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Card {

    public string Name;
    public Sprite CardFace;
    public Sprite CardBack;
    public int ScoreValue;
    public enum SUIT { HEARTS, DIAMONDS, SPADES, CLUBS };
    public SUIT Suit;
    public int CardNumber;
    public int PlayerId;
    public bool Selected = false;
    public enum CARD_STATE { IN_HAND, IN_PLAY, SCORED };
    public CARD_STATE card_state;
    public int CardId;

    public bool IsTwoOfClubs()
    {
        if(CardId == 0)
        {
            return true;
        }

        return false;
    }
}
