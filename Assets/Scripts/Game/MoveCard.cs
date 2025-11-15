using UnityEngine;
public class MoveCard : Card
{
    public MoveCardType MoveCardType { get; set; }

    public override void Initialize(CardScriptableObject cardDataSO, Sprite commonBackFace)
    {
        // Call the base class Initialize first to set the public cardSO and backFace
        base.Initialize(cardDataSO, commonBackFace);

        if (cardDataSO.cardType != CardType.MoveCard)
        {
            Debug.LogError("Attempted to initialize MoveCard with incorrect CardScriptableObject type.");
            return;
        }

        // 1. Set MoveCard-specific data
        this.MoveCardType = cardDataSO.moveCardType;

        // 3. Set initial visual: cards in the deck are face down (false)
        IsFacingUp = false;
    }

    public override void MoveToThePoint(Vector3 pointToMoveTo, Quaternion rotToMatch)
    {
        base.MoveToThePoint(pointToMoveTo, rotToMatch);
    }
}