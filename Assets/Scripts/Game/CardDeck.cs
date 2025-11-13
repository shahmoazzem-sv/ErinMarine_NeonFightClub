using System.Collections.Generic;
// UnityEngine is required for Random.Range, assumed to be in this file for Shuffle()
using UnityEngine;

public class CardDeck : IDeck
{
    // Deck stores CardData objects (which now implement ICard)
    private List<CardData> deck = new List<CardData>();

    public void AddCard(CardData cardData)
    {
        deck.Add(cardData);
    }

    // The previous implementation of AddCard only accepted HeroCardData.
    // If you intend for this general deck to accept any card data, you may need a common data base class,
    // but for now, we leave the internal storage as CardData.

    public void Shuffle()
    {
        for (int i = 0; i < deck.Count; i++)
        {
            CardData temp = deck[i];
            int randomIndex = UnityEngine.Random.Range(i, deck.Count);
            deck[i] = deck[randomIndex];
            deck[randomIndex] = temp;
        }
    }

    // FIX 1: Return type now correctly matches IDeck.DrawCard()
    public ICard DrawCard()
    {
        if (deck.Count > 0)
        {
            CardData drawnCard = deck[0];
            deck.RemoveAt(0);
            // Returns CardData, which is implicitly cast to ICard
            return drawnCard;
        }
        return null;
    }

    // FIX 2: Parameter type now correctly matches IDeck.DiscardCard(ICard)
    public void DiscardCard(ICard card)
    {
        // Only remove it if it is a CardData object
        if (card is CardData cardData)
        {
            deck.Remove(cardData);
        }
    }
}