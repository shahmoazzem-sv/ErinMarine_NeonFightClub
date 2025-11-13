using System.Collections.Generic;
public class DiscardDeck : IDeck
{
    private List<ICard> discardPile = new List<ICard>();

    public void Shuffle()
    {
        // Shuffle logic for DiscardDeck
    }

    public ICard DrawCard()
    {
        return discardPile.Count > 0 ? discardPile[0] : null;
    }

    public void DiscardCard(ICard card)
    {
        discardPile.Add(card);
    }
}