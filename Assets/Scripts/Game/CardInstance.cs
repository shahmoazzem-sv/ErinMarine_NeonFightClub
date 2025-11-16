using UnityEngine;

public class CardInstance : MonoBehaviour, ICard
{
    public bool InHand { get; set; }
    public int HandPosition { get; set; }
    public bool IsFacingUp { get; set; }


    [SerializeField] SpriteRenderer frontfaceSpriteRenderer;




    public void FlipCard(bool faceUp)
    {
        IsFacingUp = !IsFacingUp;
    }

    public void SetFrontFace(Sprite frontFaceSprite)
    {
        frontfaceSpriteRenderer.sprite = frontFaceSprite;
    }

    public void MoveToThePoint(Vector3 pointToMoveTo, Quaternion rotToMatch)
    {
        //
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
