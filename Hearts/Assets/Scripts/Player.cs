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
    public bool IsAI;

    public bool HasCardOfSuit(SUIT suit)
    {
        foreach (Card c in Cards)
        {
            if(c.Suit == suit && c.card_state == Card.CARD_STATE.IN_HAND)
            {
                return true;
            }
        }

        return false;
    }

    public void SetLegalMoves(bool firstTrick, bool heartsBroken, SUIT startingSuit, int CurrentPlaceInTrick)
    {
        foreach (Card c in Cards)
        {
            // default all moves to illegal
            c.SetLegality(false);

            // during the first trick, hearts cannot be played
            if(firstTrick)
            {
                // if player has clubs, only clubs can be played
                if (HasCardOfSuit(SUIT.CLUBS))
                {
                    if(c.Suit == SUIT.CLUBS)
                    {
                        c.SetLegality(true);
                    }
                }
                else
                {
                    // if player doesn't have clubs, all cards apart from hearts and QoS can be played
                    if (c.Suit != SUIT.HEARTS && !c.IsQueenOfSpades())
                    {
                        c.SetLegality(true);
                    }
                }
            }
            // not the first trick
            else
            {
                // first player to play a card can play any card (not hearts if hearts not broken)
                if(CurrentPlaceInTrick == 1)
                {
                    if (heartsBroken)
                    {
                        c.SetLegality(true);
                    }
                    else
                    {
                        if(c.Suit != SUIT.HEARTS)
                        {
                            c.SetLegality(true);
                        }
                    }
                }
                // not the first person to pick
                else
                {
                    // see if player can match suit already played
                    if (HasCardOfSuit(startingSuit))
                    {
                        if(c.Suit == startingSuit)
                        {
                            c.SetLegality(true);
                        }
                    }
                    // doesnt have a card of the starting suit; play anything
                    else 
                    {
                        c.SetLegality(true);
                    }
                }
            }
        }
    }

    public Card[] GetLegalMoves(bool firstTrick, bool heartsBroken, SUIT startingSuit, int CurrentPlaceInTrick)
    {
        List<Card> legalCards = new List<Card>();

        SetLegalMoves(firstTrick, heartsBroken, startingSuit, CurrentPlaceInTrick);

        foreach (Card c in Cards)
        {
            if (c.isLegal)
            {
                legalCards.Add(c);
            }
        }

        return legalCards.ToArray();
    }
}
