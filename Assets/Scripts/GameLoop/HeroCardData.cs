// NEW File: HeroCardData.cs (Not a MonoBehaviour)

public class HeroCardData
{
    public CardScriptableObject cardSO { get; set; }
    public HeroCardType HeroCardType { get; set; }
    public HeroClassType HeroClassType { get; set; }
    public int HeroAge { get; set; }

    // The HeroDeck should store these Data objects
}