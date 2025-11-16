using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardDeck : IDeck
{
    private List<ICard> deck = new List<ICard>();

    // Deck parameters
    public Transform deckBase;
    public float maxDeckHeight = 0.35f;
    public float cardOffsetY = 0.02f;

    public int Count => deck.Count;

    // --------------------------
    // Build the entire deck here
    // --------------------------
    public void BuildDeck(GameObject cardPrefab, Sprite backFace)
    {
        if (deckBase == null)
        {
            Debug.LogError("Deck base transform not assigned!");
            return;
        }

        // Load from Resources
        CardScriptableObject[] moveSOs = Resources.LoadAll<CardScriptableObject>("Movecards");
        CardScriptableObject[] specialSOs = Resources.LoadAll<CardScriptableObject>("SpecialCards");

        // Hardcoded counts
        AddCopies(moveSOs, CardType.MoveCard, MoveCardType.LightAttack, null, 8, cardPrefab, backFace);
        AddCopies(moveSOs, CardType.MoveCard, MoveCardType.MediumAttack, null, 8, cardPrefab, backFace);
        AddCopies(moveSOs, CardType.MoveCard, MoveCardType.StrongAttack, null, 8, cardPrefab, backFace);

        AddCopies(specialSOs, CardType.SpecialCard, null, SpecialCardType.BlockCard, 6, cardPrefab, backFace);
        AddCopies(specialSOs, CardType.SpecialCard, null, SpecialCardType.FuryCard, 6, cardPrefab, backFace);
        AddCopies(specialSOs, CardType.SpecialCard, null, SpecialCardType.StageAttackCard, 6, cardPrefab, backFace);
        AddCopies(specialSOs, CardType.SpecialCard, null, SpecialCardType.FrenzyCard, 2, cardPrefab, backFace);
        AddCopies(specialSOs, CardType.SpecialCard, null, SpecialCardType.BlitzCard, 2, cardPrefab, backFace);

        // Stack properly after creation
        Restack();
    }

    // Helper to instantiate cards
    private void AddCopies(
        CardScriptableObject[] source,
        CardType type,
        MoveCardType? move,
        SpecialCardType? special,
        int count,
        GameObject prefab,
        Sprite backFace)
    {
        CardScriptableObject so = null;

        foreach (var c in source)
        {
            if (c.cardType != type) continue;

            if (type == CardType.MoveCard && c.moveCardType == move)
                so = c;

            if (type == CardType.SpecialCard && c.specialCardType == special)
                so = c;
        }

        if (so == null)
        {
            Debug.LogError($"SO not found for {type}/{move}/{special}");
            return;
        }

        for (int i = 0; i < count; i++)
        {
            GameObject cardGO = GameObject.Instantiate(prefab);

            float rawY = deckBase.position.y + deck.Count * cardOffsetY;
            float clampedY = Mathf.Min(rawY, deckBase.position.y + maxDeckHeight);

            cardGO.transform.position = new Vector3(
                deckBase.position.x,
                clampedY,
                deckBase.position.z
            );

            ICard component = null;

            if (type == CardType.MoveCard)
            {
                MoveCard mc = cardGO.AddComponent<MoveCard>();
                mc.Initialize(so, backFace);
                component = mc;
            }
            else
            {
                SpecialCard sc = cardGO.AddComponent<SpecialCard>();
                sc.Initialize(so, backFace);
                component = sc;
            }

            component.IsFacingUp = false;
            deck.Add(component);
        }
    }

    // --------------------------
    // Logical shuffle
    // --------------------------
    public void Shuffle()
    {
        for (int i = 0; i < deck.Count; i++)
        {
            int r = Random.Range(i, deck.Count);
            (deck[i], deck[r]) = (deck[r], deck[i]);
        }
    }

    // --------------------------
    // Visual shuffle (safe inside deck)
    // --------------------------
    public IEnumerator ShuffleVisual()
    {
        float lift = 0.1f;

        // Lift cards
        foreach (ICard c in deck)
        {
            MonoBehaviour mono = (MonoBehaviour)c;
            mono.transform.position += new Vector3(0, lift, 0);
        }

        yield return new WaitForSeconds(0.1f);

        // Random reorder
        for (int i = 0; i < deck.Count; i++)
        {
            int r = Random.Range(0, deck.Count);
            (deck[i], deck[r]) = (deck[r], deck[i]);
        }

        // Restack after reorder
        Restack();

        yield return new WaitForSeconds(0.15f);
    }

    // --------------------------
    // Restack deck (always clamp height)
    // --------------------------
    public void Restack()
    {
        for (int i = 0; i < deck.Count; i++)
        {
            MonoBehaviour mono = (MonoBehaviour)deck[i];

            float rawY = deckBase.position.y + (i * cardOffsetY);
            float clampedY = Mathf.Min(rawY, deckBase.position.y + maxDeckHeight);

            mono.transform.position = new Vector3(
                deckBase.position.x,
                clampedY,
                deckBase.position.z
            );
        }
    }

    // --------------------------
    // Draw / discard (for later use)
    // --------------------------
    public ICard DrawCard()
    {
        if (deck.Count == 0) return null;

        ICard c = deck[0];
        deck.RemoveAt(0);

        Restack();
        return c;
    }

    public void DiscardCard(ICard card)
    {
        deck.Remove(card);
        Restack();
    }

    // Internal access if needed
    public List<ICard> GetInternalList() => deck;
}
