using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class AI : MonoBehaviour
{
    public HeroCard aiHero;

    public void SelectHeroCard(HeroCard setAIHero)
    {
        aiHero = setAIHero;
    }
}