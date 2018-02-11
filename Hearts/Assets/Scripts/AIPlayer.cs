using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIPlayer {

	public AIPlayer()
    {
        gm = GameObject.FindObjectOfType<GameManager>();
        cm = GameObject.FindObjectOfType<CardManager>();
    }

    GameManager gm;
    CardManager cm;

    virtual public void DoAI()
    {
        Debug.Log("DoAI()");
        // Do the thing for the current stage we're in

        if (gm.GameState == GameManager.GAME_STATE.PICKING)
        {
            // Pick a card!
            DoClick();
            return;
        }

    }

    virtual protected void DoClick()
    {
        // Pick a card to move, then "click" it.


        Card[] legalCards = gm.Players[gm.CurrentPlayer].GetLegalMoves(gm.firstTrick, gm.heartsBroken, gm.startingSuit, gm.CurrentPlaceInTrick);

        if (legalCards == null || legalCards.Length == 0)
        {
            // We have no legal moves.  How did we get here?
            // We might still be in a delayed coroutine somewhere. Let's not freak out.
            return;
        }

        // BasicAI simply picks a legal move at random

        Card pickedCard = PickCardToMove(legalCards);

        cm.PlayCard(pickedCard);
    }

    virtual protected Card PickCardToMove(Card[] legalCards)
    {
        return legalCards[Random.Range(0, legalCards.Length)];
    }
}
