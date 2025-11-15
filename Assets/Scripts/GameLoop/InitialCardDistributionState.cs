using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

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
        // Transition to the next state after initialization
        // gameLoopManager.ChangeState(GameState.GameRunning); 
    }

    public void Exit()
    {

    }

    public void Update()
    {

    }

    void InitializeCardDeck()
    {
        // if (gameLoopManager.cardPrefab == null || gameLoopManager.cardBackFaceSprite == null)
        // {
        //     Debug.LogError("Card Prefab or Card Back Face Sprite is not assigned in GameLoopManager. Cannot initialize card deck.");
        //     return;
        // }

        // CardDeck cardDeck = new CardDeck();

        // // Load all CardScriptableObjects from their respective Resources folders
        // CardScriptableObject[] moveCardSOs = Resources.LoadAll<CardScriptableObject>("Movecard");
        // CardScriptableObject[] specialCardSOs = Resources.LoadAll<CardScriptableObject>("SpecialCards");

        // // Helper function to find the specific CardScriptableObject and instantiate Card components.
        // void AddCardsToDeck(CardScriptableObject[] cardSOs, CardType targetType, MoveCardType? moveType, SpecialCardType? specialType, int count)
        // {
        //     CardScriptableObject matchingSO = null;
        //     Type requiredComponentType = null;

        //     // Find the correct SO and determine the component type
        //     foreach (var cardSO in cardSOs)
        //     {
        //         if (cardSO.cardType != targetType) continue;

        //         if (targetType == CardType.MoveCard && cardSO.moveCardType == moveType)
        //         {
        //             matchingSO = cardSO;
        //             requiredComponentType = typeof(MoveCard);
        //             break;
        //         }
        //         else if (targetType == CardType.SpecialCard && cardSO.specialCardType == specialType)
        //         {
        //             matchingSO = cardSO;
        //             requiredComponentType = typeof(SpecialCard);
        //             break;
        //         }
        //     }

        //     if (matchingSO == null)
        //     {
        //         Debug.LogError($"Required CardScriptableObject not found for {targetType}.");
        //         return;
        //     }

        //     // Instantiate and Initialize the required number of cards
        //     for (int i = 0; i < count; i++)
        //     {
        //         // Instantiate the Card Prefab at the deck's location, initially
        //         GameObject cardGO = GameObject.Instantiate(gameLoopManager.cardPrefab, gameLoopManager.deckPlacePoint.position, Quaternion.identity);

        //         // Add the correct derived Card component and initialize it
        //         ICard cardComponent = null;

        //         if (requiredComponentType == typeof(MoveCard))
        //         {
        //             // AddComponent returns the component instance
        //             MoveCard moveCard = cardGO.AddComponent<MoveCard>();
        //             // Initialize the instance
        //             moveCard.Initialize(matchingSO, gameLoopManager.cardBackFaceSprite);
        //             cardComponent = moveCard;
        //         }
        //         else if (requiredComponentType == typeof(SpecialCard))
        //         {
        //             SpecialCard specialCard = cardGO.AddComponent<SpecialCard>();
        //             specialCard.Initialize(matchingSO, gameLoopManager.cardBackFaceSprite);
        //             cardComponent = specialCard;
        //         }

        //         // Remove the base Card component if it exists on the prefab before adding the specific one
        //         if (cardGO.GetComponent<Card>() != null && cardGO.GetComponent<Card>().GetType() != requiredComponentType)
        //         {
        //             // Safely destroy any conflicting base Card script if the prefab was set up incorrectly
        //             // For best practice, the prefab should only contain visual/collider components.
        //         }


        //         if (cardComponent != null)
        //         {
        //             // Add the initialized component to the deck
        //             cardDeck.AddCard(cardComponent);
        //         }
        //         else
        //         {
        //             Debug.LogError($"Failed to add correct component for {matchingSO.name}. Destroying object.");
        //             GameObject.Destroy(cardGO);
        //         }
        //     }
        //     }

        //     // --- Deck Build Logic ---
        //     // Move Cards (24 total)
        //     AddCardsToDeck(moveCardSOs, CardType.MoveCard, MoveCardType.LightAttack, null, 8);
        //     AddCardsToDeck(moveCardSOs, CardType.MoveCard, MoveCardType.MediumAttack, null, 8);
        //     AddCardsToDeck(moveCardSOs, CardType.MoveCard, MoveCardType.StrongAttack, null, 8);

        //     // Special Cards (22 total)
        //     AddCardsToDeck(specialCardSOs, CardType.SpecialCard, null, SpecialCardType.BlockCard, 6);
        //     AddCardsToDeck(specialCardSOs, CardType.SpecialCard, null, SpecialCardType.FuryCard, 6);
        //     AddCardsToDeck(specialCardSOs, CardType.SpecialCard, null, SpecialCardType.StageAttackCard, 6);
        //     AddCardsToDeck(specialCardSOs, CardType.SpecialCard, null, SpecialCardType.FrenzyCard, 2);
        //     AddCardsToDeck(specialCardSOs, CardType.SpecialCard, null, SpecialCardType.BlitzCard, 2);

        //     // Assign the final deck to GameLoopManager
        //     cardDeck.Shuffle();
        //     gameLoopManager.cardDeck = cardDeck;

        //     Debug.Log($"Main Card Deck initialized and shuffled with {cardDeck.Count} cards.");

    }
}