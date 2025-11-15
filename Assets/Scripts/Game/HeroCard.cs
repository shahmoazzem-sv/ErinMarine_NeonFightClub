using UnityEngine;

public class HeroCard : Card
{
    public HeroCardType HeroCardType { get; set; }
    public HeroClassType HeroClassType { get; set; }
    public int HeroAge { get; set; }

    // NOTE: The previous 'public CardScriptableObject cardSO { get; set; }' is REMOVED
    // The data is now accessed via the inherited 'CardSO' property.

    public override void Initialize(CardScriptableObject cardDataSO, Sprite commonBackFace)
    {
        if (cardDataSO.cardType != CardType.HeroCard)
        {
            Debug.LogError("Attempted to initialize HeroCard with incorrect CardScriptableObject type.");
            return;
        }

        // 1. Call the base implementation to set CardSO and backFace
        base.Initialize(cardDataSO, commonBackFace);

        // 2. Set the HeroCard specific data using the inherited CardSO
        this.HeroCardType = CardSO.heroCardType;
        this.HeroClassType = CardSO.heroClassType;
        this.HeroAge = CardSO.heroAge;

        // 3. Set the front face sprite specific to this card's SO
        this.frontFace = CardSO.cardSprite;

        // 4. Set initial visual
        IsFacingUp = false;

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