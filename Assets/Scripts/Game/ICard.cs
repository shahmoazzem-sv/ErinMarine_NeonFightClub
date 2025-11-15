using UnityEngine;

public interface ICard
{
    bool InHand { get; set; }
    int HandPosition { get; set; }
    bool IsFacingUp { get; set; }


    void MoveToThePoint(Vector3 pointToMoveTo, Quaternion rotToMatch);

    // FIX: Added the missing FlipCard method to the interface
    void FlipCard(bool faceUp);
}