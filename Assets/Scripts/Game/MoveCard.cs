using UnityEngine;
public class MoveCard : Card
{
    public MoveCardType MoveCardType { get; set; }

    public override void MoveToThePoint(Vector3 pointToMoveTo, Quaternion rotToMatch)
    {
        base.MoveToThePoint(pointToMoveTo, rotToMatch);
        // Implement specific behavior for MoveCard
    }
}
