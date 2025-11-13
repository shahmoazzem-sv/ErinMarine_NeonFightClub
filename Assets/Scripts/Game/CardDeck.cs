using System.Collections.Generic;
public class CardDeck : IDeck
{
    private List<Card> deck = new List<Card>();

    public void Shuffle()
    {
        // Shuffle logic for CardDeck
    }

    public ICard DrawCard()
    {
        return deck.Count > 0 ? deck[0] : null;
    }

    public void DiscardCard(ICard card)
    {
        // Discard logic for CardDeck
    }
}