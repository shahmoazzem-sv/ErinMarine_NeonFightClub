using UnityEngine;

public class SpecialCard : Card
{
    public SpecialCardType SpecialCardType { get; set; }

    // This now correctly overrides the new virtual method in Card.cs
    public override void Initialize(CardScriptableObject cardDataSO, Sprite commonBackFace)
    {
        if (cardDataSO.cardType != CardType.SpecialCard)
        {
            Debug.LogError("Attempted to initialize SpecialCard with incorrect CardScriptableObject type.");
            return;
        }

        // 1. Call the base implementation to set cardSO and backFace
        base.Initialize(cardDataSO, commonBackFace);

        // 2. Set the SpecialCard specific data
        this.SpecialCardType = cardDataSO.specialCardType;

        // 3. Set the front face sprite specific to this card's SO
        this.frontFace = cardDataSO.cardSprite;

        // 4. Set initial visual: cards in the deck are face down (false)
        IsFacingUp = false; // This triggers the flip logic via the setter

        // Ensure the spriteRenderer is initialized and set to the backface if it's face down
        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = backFace;
        }
    }

    public override void MoveToThePoint(Vector3 pointToMoveTo, Quaternion rotToMatch)
    {
        base.MoveToThePoint(pointToMoveTo, rotToMatch);
    }
}