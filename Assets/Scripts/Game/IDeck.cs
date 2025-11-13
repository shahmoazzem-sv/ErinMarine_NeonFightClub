public interface IDeck
{
    void Shuffle();
    ICard DrawCard();
    void DiscardCard(ICard card);
}
