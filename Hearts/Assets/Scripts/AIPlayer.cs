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

    Card pickedCard;

    virtual public void DoAI()
    {
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
            // if we get no legal moves, try it again but with hearts broken
            // there is an unlikely case where all non-heart cards are played and the player only has hearts left and they are not yet broken
            Debug.LogWarning("No legal moves, trying again with hearts broken");
            legalCards = gm.Players[gm.CurrentPlayer].GetLegalMoves(gm.firstTrick, true, gm.startingSuit, gm.CurrentPlaceInTrick);
        }

        // BasicAI simply picks a legal move at random
        pickedCard = PickCardToMove(legalCards);

        cm.PlayPickedCardCoroutine(pickedCard);
    }

    virtual protected Card PickCardToMove(Card[] legalCards)
    {
        return legalCards[Random.Range(0, legalCards.Length)];
    }
}
