using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using TMPro;

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

    // Field to temporarily hold the player's chosen card
    private HeroCard chosenPlayerCard;

    // Helper to run coroutines on the GameLoopManager
    private MonoBehaviour CoroutineRunner => gameLoopManager;

    List<Vector3> cardPositions = new List<Vector3>();

    public HeroCardSelectionState(GameLoopManager gameLoopManager)
    {
        this.gameLoopManager = gameLoopManager;
    }

    public void Enter()
    {
        // 1. Prepare data objects
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

        // 2. Flip all cards face down 
        yield return CoroutineRunner.StartCoroutine(FlipAllCardsDown(0.5f));

        // 3. Shuffle
        yield return CoroutineRunner.StartCoroutine(ShuffleAllCardsVisual(2));

        // 4. Player Selection Phase
        yield return CoroutineRunner.StartCoroutine(PlayerSelectionPhase());

        // 5. AI Selection Phase (NOW UNCOMMENTED)
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
                cardGO.TryGetComponent<CardInstance>(out CardInstance cardInstance);
                cardInstance.SetFrontFace(cardSO.cardSprite);

                HeroCard heroComponent = cardGO.GetComponent<HeroCard>() ?? cardGO.AddComponent<HeroCard>();
                Sprite cardBackFaceSprite = gameLoopManager.cardBackFaceSprite;

                // Initialize the component 
                heroComponent.Initialize(cardSO, cardBackFaceSprite);

                spawnedHeroCards.Add(heroComponent);
            }
        }
        gameLoopManager.heroDeck = heroDeck;
        Debug.Log($"Hero Deck initialized with {heroCardSO.Length} cards.");
    }

    // --- STEP 2: PRESENTATION ---
    private void SpawnAndPresentAllHeroes()
    {
        if (gameLoopManager.heroCardSpawnMinPoint == null || gameLoopManager.heroCardSpawnMaxPoint == null)
        {
            Debug.LogError("Hero Card Spawn Points are not set in GameLoopManager.");
            return;
        }

        Vector3 minPos = gameLoopManager.heroCardSpawnMinPoint.position;
        Vector3 maxPos = gameLoopManager.heroCardSpawnMaxPoint.position;

        Vector3 distanceBetweenPoints = Vector3.zero;
        if (spawnedHeroCards.Count > 1)
        {
            distanceBetweenPoints = (maxPos - minPos) / (spawnedHeroCards.Count - 1);
        }

        cardPositions.Clear();

        for (int i = 0; i < spawnedHeroCards.Count; i++)
        {
            cardPositions.Add(minPos + (distanceBetweenPoints * i));
            spawnedHeroCards[i].MoveToThePoint(cardPositions[i], Quaternion.identity);

            // Cards start face up to show them to the player initially
            spawnedHeroCards[i].IsFacingUp = true;
        }
    }

    private IEnumerator FlipAllCardsDown(float duration)
    {
        foreach (var heroCard in spawnedHeroCards)
        {
            heroCard.IsFacingUp = false;
        }
        yield return new WaitForSeconds(duration);
    }

    // --- STEP 3: SHUFFLING ---
    private IEnumerator ShuffleAllCardsVisual(int iterations)
    {
        int swapsPerIteration = spawnedHeroCards.Count;

        for (int iteration = 0; iteration < iterations; iteration++)
        {
            Debug.Log($"Shuffle Iteration: {iteration + 1}");

            for (int s = 0; s < swapsPerIteration; s++)
            {
                int indexA = UnityEngine.Random.Range(0, cardPositions.Count);
                int indexB = UnityEngine.Random.Range(0, cardPositions.Count);

                if (indexA == indexB) continue;

                // 2. SWAP THE POSITIONS IN THE POSITION LIST
                Vector3 temp = cardPositions[indexA];
                cardPositions[indexA] = cardPositions[indexB];
                cardPositions[indexB] = temp;

                // 3. MOVE CARDS VISUALLY TO THEIR NEW POSITIONS
                spawnedHeroCards[indexA].MoveToThePoint(cardPositions[indexA], Quaternion.Euler(0, 0, 180));
                spawnedHeroCards[indexB].MoveToThePoint(cardPositions[indexB], Quaternion.Euler(0, 0, 180));

                yield return new WaitForSeconds(0.4f);
            }
            yield return new WaitForSeconds(0.4f);
        }
    }


    // --- STEP 4: PLAYER SELECTION ---
    private IEnumerator PlayerSelectionPhase()
    {
        if (spawnedHeroCards.Count < 2)
        {
            Debug.LogError("Not enough cards for selection.");
            yield break;
        }

        // 1. Select two random indices from the entire remaining deck
        int randomIndex1 = UnityEngine.Random.Range(0, spawnedHeroCards.Count);
        int randomIndex2 = UnityEngine.Random.Range(0, spawnedHeroCards.Count);
        while (randomIndex1 == randomIndex2)
        {
            randomIndex2 = UnityEngine.Random.Range(0, spawnedHeroCards.Count);
        }

        playerCardOne = spawnedHeroCards[randomIndex1];
        playerCardTwo = spawnedHeroCards[randomIndex2];

        // 2. Temporarily position them for selection
        Vector3 middlePoint = (gameLoopManager.heroCardSpawnMinPoint.position + gameLoopManager.heroCardSpawnMaxPoint.position) / 2f;
        Vector3 cardOneNewPositon = new Vector3(middlePoint.x - 1f, middlePoint.y + 2.5f, middlePoint.z);
        Vector3 cardTwoNewPositon = new Vector3(middlePoint.x + 1f, middlePoint.y + 2.5f, middlePoint.z);

        // Move the cards to new positions above their spawn points
        playerCardOne.MoveToThePoint(cardOneNewPositon, Quaternion.Euler(-10, 0, 180));
        playerCardTwo.MoveToThePoint(cardTwoNewPositon, Quaternion.Euler(-10, 0, 180));

        // 3. FIX: Ensure both cards are FACE DOWN for blind selection after movement
        playerCardOne.IsFacingUp = false;
        playerCardTwo.IsFacingUp = false;

        gameLoopManager.choseHeroText.gameObject.SetActive(true);

        chosenPlayerCard = null;

        // 4. Subscribe and Wait for player input
        HeroCard.OnCardSelected += HandlePlayerCardSelection;
        Debug.Log("Waiting for player to select a Hero Card...");
        yield return new WaitUntil(() => chosenPlayerCard != null);

        // --- SELECTION IS COMPLETE ---

        HeroCard.OnCardSelected -= HandlePlayerCardSelection;

        // 5. Determine which card was chosen and which was not
        HeroCard unchosenPlayerCard = null;
        if (chosenPlayerCard == playerCardOne)
        {
            unchosenPlayerCard = playerCardTwo;
        }
        else if (chosenPlayerCard == playerCardTwo)
        {
            unchosenPlayerCard = playerCardOne;
        }

        // 6. Flip the CHOSEN card face up
        chosenPlayerCard.IsFacingUp = true;
        // Wait for the flip animation to finish
        // NOTE: Ensure your HeroCard has a public 'flipDuration' property defined.
        yield return new WaitForSeconds(chosenPlayerCard.flipDuration + 2f);

        // 7. Finalize assignment
        gameLoopManager.playerHeroCard = chosenPlayerCard;
        gameLoopManager.player.playerHeroCard = chosenPlayerCard;

        gameLoopManager.choseHeroText.gameObject.SetActive(false);

        // 8. Cleanup and move: Remove the unchosen card from the scene and the list
        GameObject.Destroy(unchosenPlayerCard.gameObject);

        // Only remove the two cards used in the selection from the master list. 
        // The remaining cards are for the AI phase.
        spawnedHeroCards.Remove(chosenPlayerCard);
        spawnedHeroCards.Remove(unchosenPlayerCard);

        // 9. Move the chosen card to its final placement area
        chosenPlayerCard.MoveToThePoint(gameLoopManager.playerHeroCardPlacePoint.position, Quaternion.identity);

        yield return new WaitForSeconds(1f);
    }

    // --- SELECTION HANDLER ---
    private void HandlePlayerCardSelection(HeroCard selectedHeroCard)
    {
        if (chosenPlayerCard == null)
        {
            chosenPlayerCard = selectedHeroCard;
        }
    }

    // --- STEP 5: AI SELECTION ---
    private IEnumerator AISelectionPhase()
    {
        if (spawnedHeroCards.Count < 2)
        {
            Debug.LogError("Not enough cards for AI selection.");
            yield break;
        }

        // --- 1. Select two random cards ---
        int randomIndex1 = UnityEngine.Random.Range(0, spawnedHeroCards.Count);
        int randomIndex2 = UnityEngine.Random.Range(0, spawnedHeroCards.Count);
        while (randomIndex1 == randomIndex2)
            randomIndex2 = UnityEngine.Random.Range(0, spawnedHeroCards.Count);

        aiCardOne = spawnedHeroCards[randomIndex1];
        aiCardTwo = spawnedHeroCards[randomIndex2];

        // --- 2. Move both to middle (face down) ---
        Vector3 middlePoint = (gameLoopManager.heroCardSpawnMinPoint.position +
                               gameLoopManager.heroCardSpawnMaxPoint.position) / 2f;

        Vector3 pos1 = new Vector3(middlePoint.x - 1f, middlePoint.y + 2.5f, middlePoint.z);
        Vector3 pos2 = new Vector3(middlePoint.x + 1f, middlePoint.y + 2.5f, middlePoint.z);

        aiCardOne.IsFacingUp = false;
        aiCardTwo.IsFacingUp = false;

        aiCardOne.MoveToThePoint(pos1, Quaternion.Euler(-10, 0, 180));
        aiCardTwo.MoveToThePoint(pos2, Quaternion.Euler(-10, 0, 180));

        yield return new WaitForSeconds(1.2f);

        // --- 3. AI picks one ---
        HeroCard chosenAICard = (UnityEngine.Random.Range(0, 2) == 0) ? aiCardOne : aiCardTwo;
        HeroCard unchosenAICard = (chosenAICard == aiCardOne) ? aiCardTwo : aiCardOne;

        // --- IMPORTANT: DO NOT FACE DOWN ANYMORE! ---
        // Only face up the chosen card
        chosenAICard.IsFacingUp = true;

        yield return new WaitForSeconds(chosenAICard.flipDuration + 0.5f);

        // --- 4. Assign selection ---
        gameLoopManager.AIHeroCard = chosenAICard;
        gameLoopManager.ai.aiHeroCard = chosenAICard;

        // --- 5. Destroy ALL remaining spawned hero cards except chosen ---
        List<HeroCard> cardsToDestroy = new List<HeroCard>(spawnedHeroCards);
        cardsToDestroy.Remove(chosenAICard);

        foreach (var card in cardsToDestroy)
        {
            if (card != null)
                GameObject.Destroy(card.gameObject);
        }

        // Remove destroyed cards from main list
        spawnedHeroCards.Clear();
        spawnedHeroCards.Add(chosenAICard);

        // --- 6. Move chosen card to final position ---
        chosenAICard.MoveToThePoint(
            gameLoopManager.AIHeroCardPlacePoint.position,
            Quaternion.identity
        );

        yield return new WaitForSeconds(1f);
    }

}