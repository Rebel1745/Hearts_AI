using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
        gm = GameObject.FindObjectOfType<GameManager>();
	}

    private Card previousCard = null;

    GameManager gm;

    public void OnCardSelected(Card card)
    {
        if(gm.CurrentPlayer == card.PlayerId)
        {
            GameObject cardGO = gm.GetGameObjectFromCard(card);
            if (card.Selected)
            {
                Debug.Log("Selected " + card.Name);
                // if card has already been clicked once, move it to the middle
                cardGO.transform.position = gm.Players[gm.CurrentPlayer].CardInPlay.transform.position;
                cardGO.transform.rotation = gm.Players[gm.CurrentPlayer].CardInPlay.transform.rotation;
                cardGO.GetComponent<SpriteRenderer>().sortingOrder = gm.currentRound;
                card.card_state = Card.CARD_STATE.IN_PLAY;
                card.Selected = false;
                gm.NextPlayer();
            }
            else
            {
                // Select card and move it by offset
                cardGO.transform.position += gm.Players[gm.CurrentPlayer].SelectedCardOffset;
                card.Selected = true;
                
                if (previousCard != null && previousCard != card && previousCard.card_state != Card.CARD_STATE.IN_PLAY)
                {
                    GameObject previousCardGO = gm.GetGameObjectFromCard(previousCard);
                    previousCardGO.transform.position -= gm.Players[gm.CurrentPlayer].SelectedCardOffset;
                    previousCard.Selected = false;
                }
            }

            previousCard = card;
        }
    }

    public Card[] GetPossibleCards()
    {
        Card[] playersCards = gm.Players[gm.CurrentPlayer].Cards;

        List<Card> possibleCards = new List<Card>();

        return possibleCards.ToArray();
    }
}
