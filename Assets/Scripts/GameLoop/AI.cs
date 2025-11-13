using UnityEngine;

public class AI : MonoBehaviour
{
    // This field will be assigned the newly created HeroCard component by the State.
    public HeroCard aiHero;

    // This method is now redundant and can be removed.
    // public void SelectHeroCard(HeroCard setAIHero)
    // {
    //     aiHero = setAIHero;
    // }

    // This method is now redundant and removed to prevent NullReferenceException.
    // public void SetHeroCard(CardScriptableObject cardSO)
    // {
    //     aiHero.Initialize(cardSO);
    // }
}