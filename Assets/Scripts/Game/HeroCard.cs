using UnityEngine;

public class HeroCard : Card
{
    public HeroCardType HeroCardType { get; set; }
    public HeroClassType HeroClassType { get; set; }
    public int HeroAge { get; set; }

    public CardScriptableObject cardSO { get; set; }

    public override void MoveToThePoint(Vector3 pointToMoveTo, Quaternion rotToMatch)
    {
        base.MoveToThePoint(pointToMoveTo, rotToMatch);
        // Implement specific behavior for HeroCard
    }

    public void Initialize(CardScriptableObject selectedCardSO)
    {
        // Set the primary data source
        this.cardSO = selectedCardSO;

        // Transfer the specific data fields
        this.HeroCardType = selectedCardSO.heroCardType;
        this.HeroClassType = selectedCardSO.heroClassType;
        this.HeroAge = selectedCardSO.heroAge;

        // REMOVED: Sprite setting logic. This must be done manually later.

        // You might also want to set the SpriteRenderer's sprite here if the component exists
        // SpriteRenderer sr = GetComponentInChildren<SpriteRenderer>();
        // if (sr != null)
        // {
        //     sr.sprite = selectedCardSO.cardSprite;
        // }
    }
}