using UnityEngine;

public enum CardType
{
    HeroCard,
    MoveCard,
    SpecialCard
}

public enum HeroCardType
{
    Elemental,
    Human
}

public enum MoveCardType
{
    LightAttack,
    MediumAttack,
    StrongAttack
}

public enum SpecialCardType
{
    BlockCard,
    FuryCard,
    StageAttackCard,
    FrenzyCard,
    BlitzCard
}

public enum HeroClassType
{
    LightWeight,
    MiddleWeight,
    HeavyWeight
}

[CreateAssetMenu(fileName = "NewCardSO", menuName = "Card/Create New Card")]
public class CardScriptableObject : ScriptableObject
{
    public CardType cardType;

    // HERO ONLY
    public HeroCardType heroCardType;
    public HeroClassType heroClassType;
    public int heroAge = 18;

    // MOVE ONLY
    public MoveCardType moveCardType;

    // SPECIAL ONLY
    public SpecialCardType specialCardType;

    //Common properties
    public Sprite cardSprite;

}
