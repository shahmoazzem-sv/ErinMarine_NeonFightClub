using System.Collections.Generic;
using UnityEngine; // Required for Random.Range

// Assuming IDeck and ICard interfaces exist and HeroCardData is a non-MonoBehaviour data class.
// (HeroCardData is the model created in the previous solution steps)

public class HeroDeck // Assuming it implements IDeck, though IDeck is not provided
{
    // FIX: The deck stores HeroCardData (the plain C# data model), not the HeroCard component (MonoBehaviour).
    private List<HeroCardData> deck = new List<HeroCardData>();

    public void Shuffle()
    {
        for (int i = 0; i < deck.Count; i++)
        {
            // The temp variable is HeroCardData
            HeroCardData temp = deck[i];
            int randomIndex = UnityEngine.Random.Range(i, deck.Count);
            deck[i] = deck[randomIndex];
            deck[randomIndex] = temp;
        }
    }

    // FIX: DrawCard returns HeroCardData (which implements ICard in the previous setup, 
    // or should return HeroCardData if the deck's purpose is purely data storage).
    // Based on the surrounding context, we assume HeroCardData is the expected return type
    // that can be cast to ICard (or the deck should just return the data object).
    // We return HeroCardData and rely on the calling code to cast/use it correctly.
    // NOTE: For simplicity, we assume ICard means the data object interface/base class.
    public object DrawCard() // Changed return type to object/HeroCardData if IDeck doesn't mandate ICard
    {
        if (deck.Count > 0)
        {
            HeroCardData drawnCard = deck[0];
            deck.RemoveAt(0);
            return drawnCard; // Returns the data object
        }
        return null;
    }

    // This method is often unused in simple card selection stages, but is fixed for consistency.
    public void DiscardCard(object card) // Changed parameter type from ICard to object/HeroCardData
    {
        if (card is HeroCardData heroCardData)
        {
            deck.Remove(heroCardData);
        }
    }

    // FIX: AddCard accepts HeroCardData
    public void AddCard(HeroCardData card)
    {
        deck.Add(card);
    }
}