using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardManager : MonoBehaviour {

    void Awake()
    {
        gm = GameObject.FindObjectOfType<GameManager>();
        if (gm == null)
        {
            Debug.LogError("NO GAME MANAGER");
        }
    }

    GameManager gm;

    Card previousCard = null;
    Card pickedCard;

    public float timeBeforeCardPlayed = 0.25f;

    public void OnCardSelected(Card card)
    {
        if(gm.CurrentPlayer == card.PlayerId && card.isLegal)
        {
            GameObject cardGO = gm.GetGameObjectFromCard(card);
            if (card.Selected)
            {
                PlayCard(card);
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

    public void PlayPickedCardCoroutine(Card card)
    {
        pickedCard = card;
        StartCoroutine("PlayPickedCard");
    }

    IEnumerator PlayPickedCard()
    {
        yield return new WaitForSeconds(timeBeforeCardPlayed);

        PlayCard(pickedCard);
    }
    public void PlayCard(Card card)
    {
        GameObject cardGO = gm.GetGameObjectFromCard(card);
        // if card has already been clicked once, move it to the middle
        cardGO.transform.position = gm.Players[gm.CurrentPlayer].CardInPlay.transform.position;
        cardGO.transform.rotation = gm.Players[gm.CurrentPlayer].CardInPlay.transform.rotation;
        cardGO.GetComponent<SpriteRenderer>().sortingOrder = gm.CurrentPlaceInTrick;
        card.card_state = Card.CARD_STATE.IN_PLAY;
        card.Selected = false;
        if (gm.CurrentPlaceInTrick == 1)
        {
            gm.startingSuit = card.Suit;
            gm.startingValue = card.CardNumber;
            gm.startingPlayer = card.PlayerId;
        }
        gm.trickCards[gm.CurrentPlaceInTrick - 1] = card;
        if (card.Suit == SUIT.HEARTS)
        {
            gm.heartsBroken = true;
        }
        gm.NextPlayer();
    }
}
