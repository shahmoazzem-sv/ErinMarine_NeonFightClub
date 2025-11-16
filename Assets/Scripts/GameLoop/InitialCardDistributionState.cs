using System;
using System.Collections.Generic;
using UnityEngine;

public class InitialCardDistributionState : IState
{
    GameLoopManager gameLoopManager;

    public InitialCardDistributionState(GameLoopManager gameLoopManager)
    {
        this.gameLoopManager = gameLoopManager;
    }

    public void Enter()
    {
        InitializeCardDeck();
        // gameLoopManager.ChangeState(GameState.GameRunning);
    }

    public void Exit() { }

    public void Update() { }

    void InitializeCardDeck()
    {
        if (gameLoopManager.cardPrefab == null || gameLoopManager.cardBackFaceSprite == null)
        {
            Debug.LogError("Card Prefab or Card Back Face Sprite is not assigned in GameLoopManager.");
            return;
        }

        Transform deckPos = gameLoopManager.DeckPosition;
        if (deckPos == null)
        {
            Debug.LogError("DeckPosition transform is missing in GameLoopManager.");
            return;
        }

        CardDeck cardDeck = new CardDeck();

        // Load ScriptableObjects
        CardScriptableObject[] moveCardSOs = Resources.LoadAll<CardScriptableObject>("Movecards");
        CardScriptableObject[] specialCardSOs = Resources.LoadAll<CardScriptableObject>("SpecialCards");

        // ------------------------------
        // Helper: Add multiple instances of a card
        // ------------------------------
        void AddCardCopies(
            CardScriptableObject[] sourceArray,
            CardType type,
            MoveCardType? moveType,
            SpecialCardType? specialType,
            int count)
        {
            CardScriptableObject so = null;

            foreach (var c in sourceArray)
            {
                if (c.cardType != type) continue;

                if (type == CardType.MoveCard && c.moveCardType == moveType)
                    so = c;

                if (type == CardType.SpecialCard && c.specialCardType == specialType)
                    so = c;
            }

            if (so == null)
            {
                Debug.LogError($"SO for {type} not found!");
                return;
            }

            for (int i = 0; i < count; i++)
            {
                GameObject cardGO = GameObject.Instantiate(gameLoopManager.cardPrefab);

                // Physical stacking
                cardGO.transform.position = new Vector3(
                    deckPos.position.x,
                    deckPos.position.y + (i * 0.02f),
                    deckPos.position.z
                );

                ICard cardComponent = null;

                if (type == CardType.MoveCard)
                {
                    MoveCard mc = cardGO.AddComponent<MoveCard>();
                    mc.Initialize(so, gameLoopManager.cardBackFaceSprite);
                    cardComponent = mc;
                }
                else if (type == CardType.SpecialCard)
                {
                    SpecialCard sc = cardGO.AddComponent<SpecialCard>();
                    sc.Initialize(so, gameLoopManager.cardBackFaceSprite);
                    cardComponent = sc;
                }

                if (cardComponent == null)
                {
                    Debug.LogError("Failed to add card component. Destroying object.");
                    GameObject.Destroy(cardGO);
                    continue;
                }

                cardComponent.IsFacingUp = false;
                cardDeck.AddCard(cardComponent);
            }
        }

        // ------------------------------
        // Build the final deck
        // ------------------------------
        AddCardCopies(moveCardSOs, CardType.MoveCard, MoveCardType.LightAttack, null, 8);
        AddCardCopies(moveCardSOs, CardType.MoveCard, MoveCardType.MediumAttack, null, 8);
        AddCardCopies(moveCardSOs, CardType.MoveCard, MoveCardType.StrongAttack, null, 8);

        AddCardCopies(specialCardSOs, CardType.SpecialCard, null, SpecialCardType.BlockCard, 6);
        AddCardCopies(specialCardSOs, CardType.SpecialCard, null, SpecialCardType.FuryCard, 6);
        AddCardCopies(specialCardSOs, CardType.SpecialCard, null, SpecialCardType.StageAttackCard, 6);
        AddCardCopies(specialCardSOs, CardType.SpecialCard, null, SpecialCardType.FrenzyCard, 2);
        AddCardCopies(specialCardSOs, CardType.SpecialCard, null, SpecialCardType.BlitzCard, 2);

        // Finalize
        cardDeck.Shuffle();
        gameLoopManager.cardDeck = cardDeck;

        Debug.Log("Card Deck Initialized with " + cardDeck.Count + " cards.");
    }
}
