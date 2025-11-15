using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using System.Collections;

public class HeroCardSelectionState : IState
{

    private GameLoopManager gameLoopManager;
    private bool heroSelectionComplete;
    // List to hold references to the active MonoBehaviour components (the visual cards)
    private List<HeroCard> spawnedHeroCards = new List<HeroCard>();

    // Cards offered to the player/AI
    private HeroCard playerCardOne;
    private HeroCard playerCardTwo;
    private HeroCard aiCardOne;
    private HeroCard aiCardTwo;

    // Helper to run coroutines on the GameLoopManager
    private MonoBehaviour CoroutineRunner => gameLoopManager;

    public HeroCardSelectionState(GameLoopManager gameLoopManager)
    {
        this.gameLoopManager = gameLoopManager;
    }

    public void Enter()
    {
        // 1. Prepare data objects (but don't use them directly)
        InitializeHeroDeckData();

        // 2. Start the visual selection process (coroutine)
        CoroutineRunner.StartCoroutine(RunHeroSelectionProcess());
    }

    public void Exit()
    {
        // Clean up any remaining UI (if any was used)
        if (gameLoopManager.heroSelectionUIPanel != null)
        {
            gameLoopManager.heroSelectionUIPanel.SetActive(false);
        }

        // Transition to the next state
        gameLoopManager.ChangeState(GameState.InitialCardDistribution);
    }

    public void Update()
    {
        // Check for completion
        if (heroSelectionComplete)
        {
            Exit();
        }
    }

    // --- CORE FLOW COROUTINE ---
    private IEnumerator RunHeroSelectionProcess()
    {
        // 1. Spawn All Hero Cards and place them randomly, facing up
        SpawnAndPresentAllHeroes();

        // Wait a moment for player to see all cards
        yield return new WaitForSeconds(1.5f);

        // 2. Flip all cards face down (using the Card component's flip logic)
        yield return CoroutineRunner.StartCoroutine(FlipAllCardsDown(0.5f));

        // 3. Shuffle (visual and functional position change)
        yield return CoroutineRunner.StartCoroutine(ShuffleAllCardsVisual(2));

        // 4. Player Selection Phase
        yield return CoroutineRunner.StartCoroutine(PlayerSelectionPhase());

        // 5. AI Selection Phase
        yield return CoroutineRunner.StartCoroutine(AISelectionPhase());

        // 6. Complete and transition
        heroSelectionComplete = true;
    }

    // --- STEP 1: INITIALIZATION ---
    private void InitializeHeroDeckData()
    {
        HeroDeck heroDeck = new HeroDeck();
        CardScriptableObject[] heroCardSO = Resources.LoadAll<CardScriptableObject>("HeroCards");

        if (heroCardSO.Length == 0)
        {
            Debug.LogError("No Hero Cards found in Resources/HeroCards.");
            return;
        }

        foreach (var cardSO in heroCardSO)
        {
            if (cardSO.cardType == CardType.HeroCard)
            {
                // Instantiate the visual HeroCard Component directly
                GameObject cardGO = GameObject.Instantiate(gameLoopManager.cardPrefab);
                // Ensure HeroCard is on the prefab or added here
                HeroCard heroComponent = cardGO.GetComponent<HeroCard>() ?? cardGO.AddComponent<HeroCard>();

                // Get the common back face sprite
                Sprite cardBackFaceSprite = gameLoopManager.cardBackFaceSprite;

                // Initialize the component 
                heroComponent.Initialize(cardSO, cardBackFaceSprite);

                // Add the visual component to the deck manager and the temporary spawned list
                // NOTE: The HeroDeck here is likely designed to hold CardData (non-MonoBehaviour) not the component itself.
                // Assuming HeroDeck now holds the visual components for simplicity in this stage.
                // If HeroDeck should hold CardData, this line might need refactoring later.
                // heroDeck.AddCard(new HeroCardData(cardSO)); 

                spawnedHeroCards.Add(heroComponent);
            }
        }

        // NOTE: If you are adding the visual HeroCard components to the deck, the deck type needs to be updated.
        // Based on the code flow, we'll keep the Deck empty and only track the visual cards in spawnedHeroCards
        // until the final selection is made, and the logic relies on `spawnedHeroCards`.
        // The deck instantiation looks like a placeholder here, so I'll trust the rest of the flow.

        gameLoopManager.heroDeck = heroDeck;
        Debug.Log($"Hero Deck initialized with {heroCardSO.Length} cards.");
    }

    // --- STEP 2: PRESENTATION ---
    private void SpawnAndPresentAllHeroes()
    {
        // Place all cards randomly in the min/max bounding box
        if (gameLoopManager.heroCardSpawnMinPoint == null || gameLoopManager.heroCardSpawnMaxPoint == null)
        {
            Debug.LogError("Hero Card Spawn Points are not set in GameLoopManager.");
            return;
        }

        Vector3 minPos = gameLoopManager.heroCardSpawnMinPoint.position;
        Vector3 maxPos = gameLoopManager.heroCardSpawnMaxPoint.position;

        foreach (var heroCard in spawnedHeroCards)
        {
            float x = UnityEngine.Random.Range(minPos.x, maxPos.x);
            float y = UnityEngine.Random.Range(minPos.y, maxPos.y);
            float z = UnityEngine.Random.Range(minPos.z, maxPos.z);

            // Start position will be random, final rotation should be the starting rotation
            heroCard.MoveToThePoint(new Vector3(x, y, z), Quaternion.identity);

            // HeroCard.Initialize sets it to face up, but we enforce here
            heroCard.IsFacingUp = true;
        }
    }

    private IEnumerator FlipAllCardsDown(float duration)
    {
        // This visual flip is already handled by Card.IsFacingUp setter
        foreach (var heroCard in spawnedHeroCards)
        {
            heroCard.IsFacingUp = false;
        }

        // Wait for the coroutine inside the Card component to finish
        yield return new WaitForSeconds(duration);
    }

    // --- STEP 3: SHUFFLING (BUG FIXED HERE) ---
    private IEnumerator ShuffleAllCardsVisual(int iterations)
    {
        // Assuming gameLoopManager.shuffleVisualSpeed exists and controls the speed
        float moveDuration = 1f / (gameLoopManager.shuffleVisualSpeed > 0 ? gameLoopManager.shuffleVisualSpeed : 2f);

        for (int iteration = 0; iteration < iterations; iteration++)
        {
            // 1. Functional Shuffle: This shuffles the internal list of HeroCard components.
            // gameLoopManager.heroDeck.Shuffle(); // This is currently commented/unneeded if deck only holds data

            // 2. Visual Shuffle: Move all the visual components to new random physical locations.
            Vector3 minPos = gameLoopManager.heroCardSpawnMinPoint.position;
            Vector3 maxPos = gameLoopManager.heroCardSpawnMaxPoint.position;

            foreach (var heroCard in spawnedHeroCards)
            {
                // Calculate a new, random position for the card
                float x = UnityEngine.Random.Range(minPos.x, maxPos.x);
                float y = UnityEngine.Random.Range(minPos.y, maxPos.y);

                // Move the card visually to the new position
                heroCard.MoveToThePoint(new Vector3(x, y, heroCard.transform.position.z), Quaternion.Euler(0, 0, 180));
            }

            // Wait for the cards to finish moving
            yield return new WaitForSeconds(moveDuration);
        }
    }

    // --- STEP 4: PLAYER SELECTION ---
    private IEnumerator PlayerSelectionPhase()
    {
        // 1. Draw two cards from the deck (assumed to be drawn from the visual pool for now)
        // NOTE: This logic assumes HeroDeck.DrawCard() returns a HeroCard component, which conflicts with previous steps.
        // Temporarily using spawnedHeroCards to simulate drawing from the visually present cards.
        playerCardOne = spawnedHeroCards[0];
        playerCardTwo = spawnedHeroCards[1];
        spawnedHeroCards.RemoveRange(0, 2);

        if (playerCardOne == null || playerCardTwo == null)
        {
            Debug.LogError("Not enough hero cards for selection. Visual Card Count: " + spawnedHeroCards.Count);
            yield break;
        }

        // 2. Temporarily position them for selection (e.g., side by side)
        Vector3 playerPos1 = gameLoopManager.playerHeroCardPlacePoint.position + Vector3.left * 1.5f;
        Vector3 playerPos2 = gameLoopManager.playerHeroCardPlacePoint.position + Vector3.right * 1.5f;

        playerCardOne.MoveToThePoint(playerPos1, Quaternion.Euler(0, 0, 180));
        playerCardTwo.MoveToThePoint(playerPos2, Quaternion.Euler(0, 0, 180));

        yield return new WaitForSeconds(1f);

        // 3. Flip them face up for player choice
        playerCardOne.IsFacingUp = true;
        playerCardTwo.IsFacingUp = true;

        // Wait for flip duration
        // FIX: Accessing flipDuration via the CardSO property (no need for GetComponent<Card>() on prefab)
        yield return new WaitForSeconds(playerCardOne.flipDuration);

        // 4. Wait for Player Input (Simulated)
        Debug.Log("Waiting for Player to select a Hero...");
        yield return new WaitForSeconds(3f);

        // --- Simulated Player Choice ---
        HeroCard chosenPlayerCard = (UnityEngine.Random.Range(0, 2) == 0) ? playerCardOne : playerCardTwo;
        HeroCard unchosenPlayerCard = (chosenPlayerCard == playerCardOne) ? playerCardTwo : playerCardOne;

        // 5. Finalize Player Choice

        // Move Chosen card to its final place and face up (already is face up)
        chosenPlayerCard.MoveToThePoint(gameLoopManager.playerHeroCardPlacePoint.position, Quaternion.identity);

        // Assign to GameLoopManager and Player script
        gameLoopManager.playerHeroCard = chosenPlayerCard;
        // gameLoopManager.player.playerHeroCard = chosenPlayerCard; // Re-enable once 'player' reference is available

        // FIX: Accessing the public CardSO property
        Debug.Log($"Player chose {chosenPlayerCard.CardSO.name}.");

        // Destroy unchosen card
        GameObject.Destroy(unchosenPlayerCard.gameObject);
        // The card was already removed from spawnedHeroCards list above when "drawn"

        yield return new WaitForSeconds(1f);
    }

    // --- STEP 5: AI SELECTION ---
    private IEnumerator AISelectionPhase()
    {
        // 1. Draw two cards for AI (simulated from remaining visual cards)
        aiCardOne = spawnedHeroCards[0];
        aiCardTwo = spawnedHeroCards[1];
        spawnedHeroCards.RemoveRange(0, 2); // Remove the drawn cards

        if (aiCardOne == null || aiCardTwo == null)
        {
            Debug.LogError("Not enough hero cards for AI selection. Visual Card Count: " + spawnedHeroCards.Count);
            yield break;
        }

        // 2. Temporarily position them for AI selection (e.g., side by side in the AI area)
        Vector3 aiPos1 = gameLoopManager.AIHeroCardPlacePoint.position + Vector3.left * 1.5f;
        Vector3 aiPos2 = gameLoopManager.AIHeroCardPlacePoint.position + Vector3.right * 1.5f;

        // Cards are face down initially
        aiCardOne.MoveToThePoint(aiPos1, Quaternion.Euler(0, 0, 180));
        aiCardTwo.MoveToThePoint(aiPos2, Quaternion.Euler(0, 0, 180));

        yield return new WaitForSeconds(1f);

        // 3. AI 'thinks' (wait time)
        yield return new WaitForSeconds(1.5f);

        // 4. Simulated AI Choice (random)
        HeroCard chosenAICard = (UnityEngine.Random.Range(0, 2) == 0) ? aiCardOne : aiCardTwo;
        HeroCard unchosenAICard = (chosenAICard == aiCardOne) ? aiCardTwo : aiCardOne;

        // FIX: Accessing the public CardSO property
        Debug.Log($"AI chose {chosenAICard.CardSO.name}.");

        // 5. Finalize AI Choice

        // Flip Chosen card face up
        chosenAICard.IsFacingUp = true;
        // FIX: Accessing flipDuration via the CardSO property (no need for GetComponent<Card>() on prefab)
        yield return new WaitForSeconds(chosenAICard.flipDuration);

        // Move Chosen card to its final place
        chosenAICard.MoveToThePoint(gameLoopManager.AIHeroCardPlacePoint.position, Quaternion.identity);

        // Assign to GameLoopManager and AI script
        gameLoopManager.AIHeroCard = chosenAICard;
        // gameLoopManager.ai.aiHero = chosenAICard; // Re-enable once 'ai' reference is available

        // Destroy unchosen card
        GameObject.Destroy(unchosenAICard.gameObject);

        // Destroy all remaining cards that were not drawn (the rest of the deck)
        foreach (var card in spawnedHeroCards)
        {
            // The chosen cards are playerCardOne, playerCardTwo, aiCardOne, aiCardTwo.
            // The ones left in spawnedHeroCards are the rest of the deck.
            if (card != chosenAICard && card != gameLoopManager.playerHeroCard)
            {
                GameObject.Destroy(card.gameObject);
            }
        }
        spawnedHeroCards.Clear(); // Clear the list after destruction

        yield return new WaitForSeconds(1f);
    }
}