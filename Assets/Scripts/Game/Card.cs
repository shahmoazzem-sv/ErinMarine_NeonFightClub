using UnityEngine;
using System.Collections;

public abstract class Card : MonoBehaviour, ICard
{
    public bool InHand { get; set; }
    public int HandPosition { get; set; }
    public bool IsFacingUp { get; set; }

    [SerializeField] protected float moveSpeed = 5f;
    [SerializeField] protected float rotateSpeed = 180f;

    protected Vector3 targetPoint;
    protected Quaternion targetRot;
    protected Coroutine movementCoroutine;


    public virtual void MoveToThePoint(Vector3 pointToMoveTo, Quaternion rotToMatch)
    {
        // Logic for moving the card
        targetPoint = pointToMoveTo;
        targetRot = rotToMatch;

        // Start the movement coroutine (or just call directly for simple cases)
        if (movementCoroutine != null)
        {
            StopCoroutine(movementCoroutine);
        }

        movementCoroutine = StartCoroutine(MoveAndRotateCoroutine());
    }





    // This is the Coroutine that performs the actual movement
    protected IEnumerator MoveAndRotateCoroutine()
    {
        // Define a small threshold for completion
        const float positionThreshold = 0.01f;
        const float angleThreshold = 0.1f;

        // Loop until both position and rotation are close enough to the target
        while (Vector3.Distance(transform.position, targetPoint) > positionThreshold
        || Quaternion.Angle(transform.rotation, targetRot) > angleThreshold)
        {
            transform.position = Vector3.Lerp(transform.position, targetPoint, moveSpeed * Time.deltaTime);

            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, rotateSpeed * Time.deltaTime);

            //wait until next fram
            yield return null;
        }

        // Optional: Ensure it snaps exactly to the final target when done
        transform.position = targetPoint;
        transform.rotation = targetRot;

        movementCoroutine = null; // Mark as complete



    }
}