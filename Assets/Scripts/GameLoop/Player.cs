using UnityEngine;

public class Player : MonoBehaviour
{
    // This field will be assigned the live HeroCard component by the State.
    public HeroCard playerHeroCard;

    // The state now directly manages the assignment of the instantiated HeroCard component.
}