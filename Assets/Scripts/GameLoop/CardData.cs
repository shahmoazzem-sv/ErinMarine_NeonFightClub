// CardData.cs (REQUIRED UPDATE)

using UnityEngine;

// CardData must now implement ICard
public class CardData : ICard
{
    // ICard properties
    public bool InHand { get; set; }
    public int HandPosition { get; set; }
    public bool IsFacingUp { get; set; }

    public CardScriptableObject cardSO { get; set; }

    public CardData(CardScriptableObject so)
    {
        this.cardSO = so;
    }

    // ICard method implementation (Data objects do not move, so this is passive)
    public void MoveToThePoint(Vector3 pointToMoveTo, Quaternion rotToMatch)
    {
        // Data object does not move. The actual HeroCard/Card component handles movement.
        // You can leave this empty or log a warning.
    }
}