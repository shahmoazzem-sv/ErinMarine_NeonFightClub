using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class HeroCardSelectionState : IState
{

    private GameLoopManager gameLoopManager;
    private bool heroSelected;
    private bool heroSelectionPanelOpened;

    public HeroCardSelectionState(GameLoopManager gameLoopManager)
    {
        this.gameLoopManager = gameLoopManager;
    }

    // Enter the Hero Card Selection state
    public void Enter()
    {
        InitializeHeroDeck();
        OpenHeroSelectionPanel();
        SetHeroSelectionUIPanelVisualization();
        SelectAIHero(); // Select AI hero data and prepare component (no visual yet)
    }


    public void Exit()
    {
        if (heroSelectionPanelOpened) CloseHeroSelectionPanel();

        gameLoopManager.ChangeState(GameState.InitialCardDistribution);
    }

    public void Update()
    {
        if (!heroSelected) return;

        if (heroSelected)
        {
            Exit();
        }
    }

    // Method to initialize HeroDeck
    private void InitializeHeroDeck()
    {
        HeroDeck heroDeck = new HeroDeck();

        // Load all Hero Cards from Resources folder
        CardScriptableObject[] heroCardSO = Resources.LoadAll<CardScriptableObject>("HeroCards");

        if (heroCardSO.Length != 8)
        {
            Debug.LogError("Hero cards not found in Resources/HeroCards or there aren't exactly 8 cards.");
            return;
        }

        // Ensure the cards are Hero cards and add them to HeroDeck
        foreach (var cardSO in heroCardSO)
        {
            if (cardSO.cardType == CardType.HeroCard)
            {
                // FIX: Create the HeroCardData object (non-MonoBehaviour)
                HeroCardData heroCardData = new HeroCardData // <-- FIX applied here
                {
                    cardSO = cardSO,
                    HeroCardType = cardSO.heroCardType,
                    HeroClassType = cardSO.heroClassType,
                    HeroAge = cardSO.heroAge,
                };
                // Assuming HeroDeck.AddCard now accepts HeroCardData
                heroDeck.AddCard(heroCardData);
            }
            else
            {
                Debug.LogError("Card is not a HeroCard, skipping...");
            }
        }
        gameLoopManager.heroDeck = heroDeck;
    }

    void OpenHeroSelectionPanel()
    {
        gameLoopManager.heroSelectionUIPanel.SetActive(true);
        heroSelectionPanelOpened = true;
    }
    void CloseHeroSelectionPanel()
    {
        gameLoopManager.heroSelectionUIPanel.SetActive(false);
        heroSelectionPanelOpened = false;
    }

    private void SetHeroSelectionUIPanelVisualization()
    {
        //suffle card
        gameLoopManager.heroDeck.Shuffle();

        // FIX: Draw HeroCardData objects from the deck
        HeroCardData heroCardOneData = gameLoopManager.heroDeck.DrawCard() as HeroCardData;
        HeroCardData heroCardTwoData = gameLoopManager.heroDeck.DrawCard() as HeroCardData;

        // Set visualizer for card one
        gameLoopManager.HeroSelectionImageOne.sprite = heroCardOneData.cardSO.cardSprite;
        HeroCardSelectionVisualizer heroOneVisualizer = gameLoopManager.HeroSelectionImageOne.gameObject.GetComponent<HeroCardSelectionVisualizer>();
        heroOneVisualizer.cardSo = heroCardOneData.cardSO;

        // Set visualizer for card two
        gameLoopManager.HeroSelectionImageTwo.sprite = heroCardTwoData.cardSO.cardSprite;
        HeroCardSelectionVisualizer heroTwoVisualizer = gameLoopManager.HeroSelectionImageTwo.gameObject.GetComponent<HeroCardSelectionVisualizer>();
        heroTwoVisualizer.cardSo = heroCardTwoData.cardSO;

        // Enable the hero selection panel UI (buttons to select)
        Button buttonOne = gameLoopManager.HeroSelectionImageOne.GetComponent<Button>();
        Button buttonTwo = gameLoopManager.HeroSelectionImageTwo.GetComponent<Button>();

        // FIX: Pass the HeroCardData objects to SelectHero
        buttonOne.onClick.AddListener(() => SelectHero(heroCardOneData));
        buttonTwo.onClick.AddListener(() => SelectHero(heroCardTwoData));

    }

    private void SelectAIHero()
    {
        // FIX: Draw HeroCardData objects
        HeroCardData aiHeroCardOneData = gameLoopManager.heroDeck.DrawCard() as HeroCardData;
        HeroCardData aiHeroCardTwoData = gameLoopManager.heroDeck.DrawCard() as HeroCardData;

        // Select one card randomly for AI (Data object)
        HeroCardData selectedAIHeroCardData = (UnityEngine.Random.Range(0, 2) == 0) ? aiHeroCardOneData : aiHeroCardTwoData;

        // 1. Attach the HeroCard component to the AI's GameObject
        // This is where the MonoBehaviour is created.
        HeroCard aiHeroComponent = gameLoopManager.AIHeroCardGameObject.AddComponent<HeroCard>();

        // 2. Transfer data using the Initialize method
        // Only pass the CardScriptableObject (cardSO) from the data object
        aiHeroComponent.Initialize(selectedAIHeroCardData.cardSO);

        // 3. Store the ATTACHED component (MonoBehaviour)
        gameLoopManager.AIHeroCard = aiHeroComponent;

        // Visuals are still updated later.
    }


    // FIX: Method now accepts HeroCardData
    public void SelectHero(HeroCardData selectedHeroCardData)
    {
        CloseHeroSelectionPanel();

        // Attach the HeroCard component to the player's GameObject
        // This is where the MonoBehaviour is created.
        HeroCard playerHeroComponent = gameLoopManager.playerHeroCardGameObject.AddComponent<HeroCard>();

        // 1. Set all properties in one call using the CardScriptableObject
        playerHeroComponent.Initialize(selectedHeroCardData.cardSO);

        // The Initialize method should handle the sprite update, 
        // but keeping this line for direct control if Initialize doesn't:
        gameLoopManager.playerHeroCardGameObject.GetComponentInChildren<SpriteRenderer>().sprite = selectedHeroCardData.cardSO.cardSprite;

        // Assign the *component* (MonoBehaviour) to the manager field
        gameLoopManager.playerHeroCard = playerHeroComponent;

        // Use the component for movement
        playerHeroComponent.MoveToThePoint(gameLoopManager.playerHeroCardPlacePoint.position, playerHeroComponent.gameObject.transform.rotation);

        // Also move AI card
        UpdateAIHeroVisual();

        heroSelected = true;
    }

    // Update AI hero's visual after the player selects
    private void UpdateAIHeroVisual()
    {
        // This method relies on AIHeroCard being the attached MonoBehaviour component, which the fix ensures.
        HeroCard selectedAIHeroCard = gameLoopManager.AIHeroCard;

        // Ensure AIHeroCardGameObject and SpriteRenderer are assigned
        if (gameLoopManager.AIHeroCardGameObject != null)
        {
            SpriteRenderer aiSpriteRenderer = gameLoopManager.AIHeroCardGameObject.GetComponentInChildren<SpriteRenderer>();

            if (aiSpriteRenderer != null && selectedAIHeroCard != null && selectedAIHeroCard.cardSO != null && selectedAIHeroCard.cardSO.cardSprite != null)
            {
                // Assign the AI hero's card sprite to the AI's GameObject
                aiSpriteRenderer.sprite = selectedAIHeroCard.cardSO.cardSprite;

                // Move AI hero card to its position
                selectedAIHeroCard.MoveToThePoint(gameLoopManager.AIHeroCardPlacePoint.position, selectedAIHeroCard.gameObject.transform.rotation);
            }
            else
            {
                Debug.LogError("SpriteRenderer or card data is missing for AI hero.");
            }
        }
        else
        {
            Debug.LogError("AIHeroCardGameObject is not assigned.");
        }
    }
}