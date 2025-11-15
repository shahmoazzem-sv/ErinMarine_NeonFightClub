using System.Collections.Generic;
using UnityEngine; // Required for Random.Range

// Assuming IDeck and ICard interfaces exist and HeroCardData is a non-MonoBehaviour data class.
// (HeroCardData is the model created in the previous solution steps)

public class HeroDeck // Assuming it implements IDeck, though IDeck is not provided
{
    // The deck stores the actual visual components (HeroCard), which implement ICard.
    private List<HeroCard> deck = new List<HeroCard>();

    public int Count => deck.Count;

    public void AddCard(HeroCard card)
    {
        deck.Add(card);
    }

    public void Shuffle()
    {
        // Standard Fisher-Yates shuffle algorithm
        for (int i = 0; i < deck.Count; i++)
        {
            HeroCard temp = deck[i];
            // Randomly swap the current card with any card from the remaining part of the deck
            int randomIndex = UnityEngine.Random.Range(i, deck.Count);
            deck[i] = deck[randomIndex];
            deck[randomIndex] = temp;
        }
    }

    // Draws the card from the top (index 0) and removes it from the deck.
    public ICard DrawCard()
    {
        if (deck.Count > 0)
        {
            HeroCard drawnCard = deck[0];
            deck.RemoveAt(0);
            return drawnCard;
        }
        return null;
    }

    public void DiscardCard(ICard card)
    {
        if (card is HeroCard heroCard)
        {
            deck.Remove(heroCard);
        }
    }
}