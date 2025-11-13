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
    }

    public void Exit()
    {

    }

    public void Update()
    {

    }

    void InitializeCardDeck()
    {
        CardDeck cardDeck = new CardDeck();
        // 1. Load all Move Cards and Special Cards from their respective Resources folders
        CardScriptableObject[] moveCardSOs = Resources.LoadAll<CardScriptableObject>("MoveCards");
        CardScriptableObject[] specialCardSOs = Resources.LoadAll<CardScriptableObject>("SpecialCards");

        // Helper function to find the specific CardScriptableObject and add copies to the deck.

        void AddCardsToDeck(CardScriptableObject[] cardSOs, CardType targetType, MoveCardType? moveType, SpecialCardType? specialType, int count)
        {
            CardScriptableObject matchingSO = null;
            foreach (var cardSO in cardSOs)
            {
                // Check general type
                if (cardSO.cardType != targetType) continue;

                // Check specific subtype
                if (targetType == CardType.MoveCard && cardSO.moveCardType == moveType)
                {
                    matchingSO = cardSO;
                    break;
                }
                else if (targetType == CardType.SpecialCard && cardSO.specialCardType == specialType)
                {
                    matchingSO = cardSO;
                    break;
                }
            }

            if (matchingSO == null)
            {
                Debug.LogError($"Required CardScriptableObject not found for {targetType}. Check Resources folders 'Movecard' and 'SpecialCards'.");
                return;
            }

            // Add the required number of CardData objects to the deck
            for (int i = 0; i < count; i++)
            {
                CardData cardData = new CardData(matchingSO);
                cardDeck.AddCard(cardData);
            }
        }

        // --- Move Cards (24 total) ---
        AddCardsToDeck(moveCardSOs, CardType.MoveCard, MoveCardType.LightAttack, null, 8);
        AddCardsToDeck(moveCardSOs, CardType.MoveCard, MoveCardType.MediumAttack, null, 8);
        AddCardsToDeck(moveCardSOs, CardType.MoveCard, MoveCardType.StrongAttack, null, 8);

        // --- Special Cards (22 total) ---
        AddCardsToDeck(specialCardSOs, CardType.SpecialCard, null, SpecialCardType.BlockCard, 6);
        AddCardsToDeck(specialCardSOs, CardType.SpecialCard, null, SpecialCardType.FuryCard, 6);
        AddCardsToDeck(specialCardSOs, CardType.SpecialCard, null, SpecialCardType.StageAttackCard, 6);
        AddCardsToDeck(specialCardSOs, CardType.SpecialCard, null, SpecialCardType.FrenzyCard, 2);
        AddCardsToDeck(specialCardSOs, CardType.SpecialCard, null, SpecialCardType.BlitzCard, 2);

        // 2. Assign the final deck to GameLoopManager
        gameLoopManager.cardDeck = cardDeck;

        Debug.Log($"Main Card Deck initialized with 46 cards.");

    }
}