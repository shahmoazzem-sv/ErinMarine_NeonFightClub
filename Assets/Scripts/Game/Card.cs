using UnityEngine;
using System.Collections;

public abstract class Card : MonoBehaviour, ICard
{
    public bool InHand { get; set; }
    public int HandPosition { get; set; }

    [Header("Sprites")]
    // Sprites need to be public or serialized fields to be assigned in the Inspector
    [SerializeField] protected Sprite frontFace;
    [SerializeField] protected Sprite backFace;
    [SerializeField] protected SpriteRenderer spriteRenderer;

    [Header("Movement and Flip")]
    [SerializeField] protected float moveSpeed = 5f;
    [SerializeField] protected float rotateSpeed = 180f;
    public float flipDuration = 0.5f; // Duration of the flip animation

    // FIX: Changed to a public property with a protected setter
    // This allows outside classes (like the State Machine) to READ the card data, 
    // but only the Card class and derived classes can SET it during initialization.
    public CardScriptableObject CardSO { get; protected set; }

    protected Vector3 targetPoint;
    protected Quaternion targetRot;
    protected Coroutine movementCoroutine;
    protected Coroutine flipCoroutine;

    // Internal state tracking
    private bool isFacingUp = false;

    // Public property to control the face-up/down state
    public bool IsFacingUp
    {
        get => isFacingUp;
        set
        {
            if (isFacingUp != value)
            {
                isFacingUp = value;
                // Calls the interface method
                FlipCard(value);
            }
        }
    }

    public virtual void Initialize(CardScriptableObject cardDataSO, Sprite commonBackFace)
    {
        // 1. Set data using the new public property
        this.CardSO = cardDataSO;
        this.backFace = commonBackFace;
        // The frontFace will be set by the specific card type (Hero, Special, etc.)
    }


    // --- ICard Implementations ---

    public virtual void MoveToThePoint(Vector3 pointToMoveTo, Quaternion rotToMatch)
    {
        targetPoint = pointToMoveTo;
        targetRot = rotToMatch;

        if (movementCoroutine != null)
        {
            StopCoroutine(movementCoroutine);
        }

        movementCoroutine = StartCoroutine(MoveAndRotateCoroutine());
    }

    public void FlipCard(bool faceUp)
    {
        if (flipCoroutine != null)
        {
            StopCoroutine(flipCoroutine);
        }
        flipCoroutine = StartCoroutine(FlipCoroutine(faceUp));
    }


    // --- Coroutines ---

    protected IEnumerator MoveAndRotateCoroutine()
    {
        const float positionThreshold = 0.01f;
        const float angleThreshold = 0.1f;

        while (Vector3.Distance(transform.position, targetPoint) > positionThreshold
        || Quaternion.Angle(transform.rotation, targetRot) > angleThreshold)
        {
            transform.position = Vector3.Lerp(transform.position, targetPoint, moveSpeed * Time.deltaTime);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, rotateSpeed * Time.deltaTime);
            yield return null;
        }

        transform.position = targetPoint;
        transform.rotation = targetRot;
        movementCoroutine = null;
    }

    protected IEnumerator FlipCoroutine(bool faceUp)
    {
        Quaternion startRotation = transform.rotation;
        Quaternion targetRotation = faceUp ? Quaternion.Euler(0, 0, 0) : Quaternion.Euler(0, 0, 180);

        float timeElapsed = 0f;

        while (timeElapsed < flipDuration)
        {
            timeElapsed += Time.deltaTime;
            float t = timeElapsed / flipDuration;

            // Interpolate rotation
            transform.rotation = Quaternion.Slerp(startRotation, targetRotation, t);

            // Change sprite halfway through the flip
            if (t > 0.45f && t < 0.55f && spriteRenderer != null)
            {
                spriteRenderer.sprite = faceUp ? frontFace : backFace;
            }

            yield return null;
        }

        // Ensure final state is set
        transform.rotation = targetRotation;

        // Final sprite check after flip finishes
        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = faceUp ? frontFace : backFace;
        }

        flipCoroutine = null;
    }
}