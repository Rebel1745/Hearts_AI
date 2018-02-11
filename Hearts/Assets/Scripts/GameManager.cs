﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public enum SUIT { HEARTS, DIAMONDS, SPADES, CLUBS };

public class GameManager : MonoBehaviour {

    // Use this for initialization
    void Start() {
        GameObjectToCard = new Dictionary<GameObject, Card>();
        CardToGameObject = new Dictionary<Card, GameObject>();

        PlayerAIs = new AIPlayer[Players.Length];

        PlayerAIs[0] = null;    // Is a human player
        //PlayerAIs[0] = new AIPlayer();
        PlayerAIs[1] = new AIPlayer();
        PlayerAIs[2] = new AIPlayer();
        PlayerAIs[3] = new AIPlayer();

        SetupRound();
	}

    public Card[] Cards;
    private Card[] shuffledCards;
    public Transform CardHolder;
    public int CurrentPlayer = 0;
    public int currentTrick = 0; // each set of four cards is called a trick (max no. = no. of cards per player)
    public int currentHand = 0; // each set of 13 cards is called a hand
    public int currentRound = 0; // rounds are played until a player score reaches or exceded 100

    public Player[] Players;
    AIPlayer[] PlayerAIs;

    public enum GAME_STATE{ SWAPPING, PICKING, AI_CALCULATING, SCORING};
    public GAME_STATE GameState = GAME_STATE.PICKING;

    public Dictionary<GameObject, Card> GameObjectToCard;
    public Dictionary<Card, GameObject> CardToGameObject;

    // Per-trick variables
    public Card[] trickCards = new Card[4];
    public SUIT startingSuit;
    public int startingValue;
    public int startingPlayer;
    public bool firstTrick = true;
    public bool heartsBroken = false;
    // whether 1st, 2nd, 3rd or 4th card to be placed
    public int CurrentPlaceInTrick = 1;

    void SetupRound()
    {
        GameState = GAME_STATE.PICKING;
        ShuffleCards(Cards);
        DealCards();
        SortPlayerCards();
        DisplayCards();
        currentTrick = 0;
        currentHand = 0;
        firstTrick = true;
        heartsBroken = false;
        CurrentPlaceInTrick = 1;
        GetStartingPlayer();
    }

    void GetStartingPlayer()
    {
        foreach(Card c in shuffledCards)
        {
            if (c.IsTwoOfClubs())
            {
                CurrentPlayer = c.PlayerId;
                c.isLegal = true;
            }
        }
        Debug.Log("Starting Player: " + Players[CurrentPlayer].PlayerName);
        if (PlayerAIs[CurrentPlayer] != null)
        {
            PlayerAIs[CurrentPlayer].DoAI();
        }
    }

    // next trick is called 13 times before a new hand is dealt
    void NextTrick()
    {
        ScoreCards();
        currentTrick++;
        firstTrick = false;
        CurrentPlaceInTrick = 1;
        Players[CurrentPlayer].GetLegalMoves(firstTrick, heartsBroken, startingSuit, CurrentPlaceInTrick);
        if (PlayerAIs[CurrentPlayer] != null)
        {
            PlayerAIs[CurrentPlayer].DoAI();
        }
    }

    void ScoreCards()
    {
        int trickWinner = startingPlayer;
        int currentHighestCard = startingValue;

        foreach(Card c in trickCards)
        {
            if(c.Suit == startingSuit && c.CardNumber > currentHighestCard)
            {
                trickWinner = c.PlayerId;
                currentHighestCard = c.CardNumber;
            }
        }

        Debug.Log("Trick Won by: " + Players[trickWinner].PlayerName);
        foreach (Card c in trickCards)
        {
            Players[trickWinner].ScoredCards.Add(c);
            Players[trickWinner].Score += c.ScoreValue;
            c.card_state = Card.CARD_STATE.SCORED;
            GameObject cardGO = GetGameObjectFromCard(c);
            cardGO.SetActive(false);
        }

        CurrentPlayer = trickWinner;
    }

    public void NextPlayer()
    {
        CurrentPlayer = (CurrentPlayer + 1) % Players.Length;
        CurrentPlaceInTrick++;

        if (CurrentPlaceInTrick == Players.Length + 1)
        {
            NextTrick();
        }
        else
        {
            if (PlayerAIs[CurrentPlayer] != null)
            {
                PlayerAIs[CurrentPlayer].DoAI();
            }
            Players[CurrentPlayer].GetLegalMoves(firstTrick, heartsBroken, startingSuit, CurrentPlaceInTrick);
        }
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
        int cardsPerPlayer = shuffledCards.Length / Players.Length;
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
