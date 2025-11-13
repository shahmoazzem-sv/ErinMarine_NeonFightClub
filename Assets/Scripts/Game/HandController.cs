using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HandController : MonoBehaviour
{
    [SerializeField] public List<Card> heldCards = new List<Card>();

    public List<Vector3> cardPositions = new List<Vector3>();

    [SerializeField] Transform minPos, maxPos;
    [SerializeField] Vector3 inHandRoatateAmount;

    void Start()
    {
        SetCardPositionsInHand();
    }

    public void SetCardPositionsInHand()
    {
        cardPositions.Clear();

        Vector3 distanceBetweenPoints = Vector3.zero;
        if (heldCards.Count > 1)
        {
            distanceBetweenPoints = (maxPos.position - minPos.position) / (heldCards.Count - 1);
        }

        for (int i = 0; i < heldCards.Count; i++)
        {
            // Cards positions added to the card positions list
            cardPositions.Add(minPos.position + (distanceBetweenPoints * i));

            // Move card to the specific position
            heldCards[i].MoveToThePoint(cardPositions[i], Quaternion.Euler(inHandRoatateAmount));

            // Register as in hand
            heldCards[i].InHand = true;
            heldCards[i].HandPosition = i; // position index
        }
    }
}
