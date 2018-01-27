using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GameManager : MonoBehaviour {

    // Use this for initialization
    void Start() {
        GameObjectToCard = new Dictionary<GameObject, Card>();
        CardToGameObject = new Dictionary<Card, GameObject>();
        ShuffleCards(Cards);
        DealCards();
        SortPlayerCards();
        DisplayCards();
        GetStartingPlayer();
	}

    public Card[] Cards;
    private Card[] shuffledCards;
    public Transform CardHolder;
    public int NumberOfCards = 52;
    public int CurrentPlayer = 0;
    int totalRounds = 4;
    public int currentRound = 0;
    public int currentHand = 0;

    public Player[] Players;

    public enum GAME_STATE{ SWAPPING, PICKING, AI_CALCULATING, SCORING};
    public GAME_STATE GameState = GAME_STATE.PICKING;

    public Dictionary<GameObject, Card> GameObjectToCard;
    public Dictionary<Card, GameObject> CardToGameObject;


    void GetStartingPlayer()
    {
        foreach(Card c in shuffledCards)
        {
            if (c.IsTwoOfClubs())
            {
                CurrentPlayer = c.PlayerId;
            }
        }
    }

    void NextTurn()
    {
        Debug.Log("NextTurn()");
        CurrentPlayer = 0;
        currentHand = 0;
    }

    public void NextPlayer()
    {
        if(currentHand == Players.Length - 1)
        {
            NextTurn();
        }

        CurrentPlayer = (CurrentPlayer + 1) % Players.Length;
        currentHand++;

        Debug.Log("NextPlayer()::" + Players[CurrentPlayer].PlayerName);
    }

    void SortPlayerCards()
    {
        for (int i = 0; i < Players.Length; i++)
        {
            Players[i].Cards = Players[i].Cards.OrderBy(t => t.CardId).ToArray();
        }
    }

    void DisplayCards()
    {
        GameObject nextCard;
        Vector3 cardPos, cardOffset, playerOffset;
        SpriteRenderer sr;

        for (int p = 0; p < Players.Length; p++)
        {
            for (int c = 0; c < Players[p].Cards.Length; c++)
            {
                nextCard = GetGameObjectFromCard(Players[p].Cards[c]);
                sr = nextCard.GetComponent<SpriteRenderer>();
                sr.sortingOrder = c;

                cardPos = Players[p].CardSpawn.transform.position;
                playerOffset = Players[p].OffsetPerCard;
                cardOffset = new Vector3(c * playerOffset.x, c * playerOffset.y, 0f);
                nextCard.transform.position = cardPos + cardOffset;
                nextCard.transform.rotation = Players[p].CardSpawn.transform.rotation;
            }
        }
    }

    void ShuffleCards(Card[] cards)
    {
        shuffledCards = cards;
        // Knuth shuffle algorithm :: courtesy of Wikipedia :)
        for (int t = 0; t < shuffledCards.Length; t++)
        {
            Card tmp = shuffledCards[t];
            int r = Random.Range(t, shuffledCards.Length);
            shuffledCards[t] = cards[r];
            shuffledCards[r] = tmp;
        }
    }

    void DealCards()
    {
        GameObject nextCard;
        int cardIndex = 0;
        int cardsPerPlayer = NumberOfCards / Players.Length;
        SpriteRenderer sr;

        for (int p = 0; p < Players.Length; p++)
        {
            for (int c = 0; c < cardsPerPlayer; c++)
            {
                cardIndex = (p * cardsPerPlayer) + c;

                // Create and populate a new card GameObject
                nextCard = new GameObject();
                sr = nextCard.AddComponent<SpriteRenderer>();
                sr.sprite = shuffledCards[cardIndex].CardFace;
                nextCard.AddComponent<BoxCollider2D>();
                nextCard.transform.SetParent(CardHolder);
                nextCard.name = shuffledCards[cardIndex].Name;

                // Set variables of CardContoller script
                CardController cc = nextCard.AddComponent<CardController>();
                cc.card = shuffledCards[cardIndex];
                cc.card.PlayerId = p;

                // Add card details to individual player
                Players[p].Cards[c] = Cards[cardIndex];

                // Add to corresponding dictionaries
                CardToGameObject[shuffledCards[cardIndex]] = nextCard;
                GameObjectToCard[nextCard] = Cards[cardIndex];
            }
        }
        
    }

    public Card GetCardFromGameObject(GameObject cardGO)
    {
        if (GameObjectToCard.ContainsKey(cardGO))
        {
            return GameObjectToCard[cardGO];
        }

        return null;
    }

    public GameObject GetGameObjectFromCard(Card card)
    {
        if (CardToGameObject.ContainsKey(card))
        {
            return CardToGameObject[card];
        }

        return null;
    }
}
