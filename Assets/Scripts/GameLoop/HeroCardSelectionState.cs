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

    // Method to initialize HeroDeck (Unchanged from previous fix)
    private void InitializeHeroDeck()
    {
        HeroDeck heroDeck = new HeroDeck();
        CardScriptableObject[] heroCardSO = Resources.LoadAll<CardScriptableObject>("HeroCards");

        if (heroCardSO.Length != 8)
        {
            Debug.LogError("Hero cards not found in Resources/HeroCards or there aren't exactly 8 cards.");
            return;
        }

        foreach (var cardSO in heroCardSO)
        {
            if (cardSO.cardType == CardType.HeroCard)
            {
                HeroCardData heroCardData = new HeroCardData
                {
                    cardSO = cardSO,
                    HeroCardType = cardSO.heroCardType,
                    HeroClassType = cardSO.heroClassType,
                    HeroAge = cardSO.heroAge,
                };
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
        gameLoopManager.heroDeck.Shuffle();

        HeroCardData heroCardOneData = gameLoopManager.heroDeck.DrawCard() as HeroCardData;
        HeroCardData heroCardTwoData = gameLoopManager.heroDeck.DrawCard() as HeroCardData;

        gameLoopManager.HeroSelectionImageOne.sprite = heroCardOneData.cardSO.cardSprite;
        HeroCardSelectionVisualizer heroOneVisualizer = gameLoopManager.HeroSelectionImageOne.gameObject.GetComponent<HeroCardSelectionVisualizer>();
        heroOneVisualizer.cardSo = heroCardOneData.cardSO;

        gameLoopManager.HeroSelectionImageTwo.sprite = heroCardTwoData.cardSO.cardSprite;
        HeroCardSelectionVisualizer heroTwoVisualizer = gameLoopManager.HeroSelectionImageTwo.gameObject.GetComponent<HeroCardSelectionVisualizer>();
        heroTwoVisualizer.cardSo = heroCardTwoData.cardSO;

        Button buttonOne = gameLoopManager.HeroSelectionImageOne.GetComponent<Button>();
        Button buttonTwo = gameLoopManager.HeroSelectionImageTwo.GetComponent<Button>();

        buttonOne.onClick.AddListener(() => SelectHero(heroCardOneData));
        buttonTwo.onClick.AddListener(() => SelectHero(heroCardTwoData));
    }

    private void SelectAIHero()
    {
        HeroCardData aiHeroCardOneData = gameLoopManager.heroDeck.DrawCard() as HeroCardData;
        HeroCardData aiHeroCardTwoData = gameLoopManager.heroDeck.DrawCard() as HeroCardData;

        HeroCardData selectedAIHeroCardData = (UnityEngine.Random.Range(0, 2) == 0) ? aiHeroCardOneData : aiHeroCardTwoData;

        // 1. Attach the HeroCard component to the AI's GameObject
        HeroCard aiHeroComponent = gameLoopManager.AIHeroCardGameObject.AddComponent<HeroCard>();

        // 2. Transfer data using the Initialize method
        aiHeroComponent.Initialize(selectedAIHeroCardData.cardSO);

        // FIX 1: Directly assign the newly created component to the AI script's reference.
        gameLoopManager.ai.aiHero = aiHeroComponent;

        // 3. Store the ATTACHED component (MonoBehaviour) in GameLoopManager
        gameLoopManager.AIHeroCard = aiHeroComponent;

        // REMOVED: gameLoopManager.ai.SetHeroCard(selectedAIHeroCardData.cardSO);
    }


    public void SelectHero(HeroCardData selectedHeroCardData)
    {
        CloseHeroSelectionPanel();

        // Attach the HeroCard component to the player's GameObject
        HeroCard playerHeroComponent = gameLoopManager.playerHeroCardGameObject.AddComponent<HeroCard>();

        // 1. Set all properties in one call using the CardScriptableObject
        playerHeroComponent.Initialize(selectedHeroCardData.cardSO);

        // Explicitly set the player's sprite here
        gameLoopManager.playerHeroCardGameObject.GetComponentInChildren<SpriteRenderer>().sprite = selectedHeroCardData.cardSO.cardSprite;

        // FIX 2: Directly assign the newly created component to the Player script's reference.
        gameLoopManager.player.playerHeroCard = playerHeroComponent;

        // Assign the *component* (MonoBehaviour) to the manager field
        gameLoopManager.playerHeroCard = playerHeroComponent;

        // Use the component for movement
        playerHeroComponent.MoveToThePoint(gameLoopManager.playerHeroCardPlacePoint.position, playerHeroComponent.gameObject.transform.rotation);

        // Also move AI card
        UpdateAIHeroVisual();

        // REMOVED: gameLoopManager.player.SetHeroCard(selectedHeroCardData.cardSO);

        heroSelected = true;
    }

    // Update AI hero's visual after the player selects (Unchanged)
    private void UpdateAIHeroVisual()
    {
        HeroCard selectedAIHeroCard = gameLoopManager.AIHeroCard;

        if (gameLoopManager.AIHeroCardGameObject != null)
        {
            SpriteRenderer aiSpriteRenderer = gameLoopManager.AIHeroCardGameObject.GetComponentInChildren<SpriteRenderer>();

            if (aiSpriteRenderer != null && selectedAIHeroCard != null && selectedAIHeroCard.cardSO != null && selectedAIHeroCard.cardSO.cardSprite != null)
            {
                aiSpriteRenderer.sprite = selectedAIHeroCard.cardSO.cardSprite;
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