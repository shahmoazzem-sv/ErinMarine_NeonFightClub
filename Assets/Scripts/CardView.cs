using Fusion;
using UnityEngine;

public class CardView : NetworkBehaviour
{
    public bool faceUp = false;
    public Sprite frontSprite;
    public Sprite backSprite;
    private SpriteRenderer sr;

    void Awake()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
    }

    public override void Render()
    {
        sr.sprite = faceUp ? frontSprite : backSprite;
    }
}
