using UnityEngine;
public class SpecialCard : Card
{
    public SpecialCardType SpecialCardType { get; set; }

    public override void MoveToThePoint(Vector3 pointToMoveTo, Quaternion rotToMatch)
    {
        base.MoveToThePoint(pointToMoveTo, rotToMatch);
        // Implement specific behavior for SpecialCard
    }
}
