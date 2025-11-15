using System.Collections.Generic;
// UnityEngine is required for Random.Range, assumed to be in this file for Shuffle()
using UnityEngine;

public class CardDeck : IDeck
{
    // Deck stores the instantiated ICard (MonoBehaviour) components
    private List<ICard> deck = new List<ICard>();

    // This method is called by the state to add a newly instantiated component
    public void AddCard(ICard cardComponent)
    {
        if (cardComponent == null) return;

        deck.Add(cardComponent);
        cardComponent.FlipCard(false); // Ensure all cards added to the deck are face down

        // Optionally, move the card game object to a default deck position here
        // ((MonoBehaviour)cardComponent).transform.position = defaultDeckPosition;
    }
    // The previous implementation of AddCard only accepted HeroCardData.
    // If you intend for this general deck to accept any card data, you may need a common data base class,
    // but for now, we leave the internal storage as CardData.

    public void Shuffle()
    {
        // Standard List shuffle implementation using Fisher-Yates (or similar)
        for (int i = 0; i < deck.Count; i++)
        {
            ICard temp = deck[i];
            int randomIndex = UnityEngine.Random.Range(i, deck.Count);
            deck[i] = deck[randomIndex];
            deck[randomIndex] = temp;
        }
    }

    public ICard DrawCard()
    {
        if (deck.Count > 0)
        {
            ICard drawnCard = deck[0];
            deck.RemoveAt(0);
            return drawnCard; // Returns the instantiated ICard component
        }
        return null;
    }
    // FIX 2: Parameter type now correctly matches IDeck.DiscardCard(ICard)
    public void DiscardCard(ICard card)
    {
        if (card != null)
        {
            deck.Remove(card);
            // You might want to move the card object to a discard pile position here
        }
    }

    // Helper property to check card count (used by Debug.Log in State)
    public int Count => deck.Count;

}