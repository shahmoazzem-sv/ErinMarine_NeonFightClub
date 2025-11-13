using UnityEngine;

public class Player : MonoBehaviour
{
    // This field will be assigned the newly created HeroCard component by the State.
    public HeroCard playerHeroCard;

    // This method is now redundant and removed to prevent NullReferenceException.
    // public void SetHeroCard(CardScriptableObject cardSo)
    // {
    //     playerHeroCard.Initialize(cardSo);
    // }
}