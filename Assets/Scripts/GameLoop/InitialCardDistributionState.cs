using System.Collections;
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
        gameLoopManager.StartCoroutine(Run());
    }

    public void Exit() { }
    public void Update() { }

    IEnumerator Run()
    {
        // Build deck
        gameLoopManager.cardDeck = new CardDeck();
        gameLoopManager.cardDeck.deckBase = gameLoopManager.DeckPosition;
        gameLoopManager.cardDeck.maxDeckHeight = 0.35f;

        gameLoopManager.cardDeck.BuildDeck(
            gameLoopManager.cardPrefab,
            gameLoopManager.cardBackFaceSprite
        );

        // Visual shuffle
        yield return gameLoopManager.StartCoroutine(
            gameLoopManager.cardDeck.ShuffleVisual()
        );

        // Logical shuffle
        gameLoopManager.cardDeck.Shuffle();

        // Restack again
        gameLoopManager.cardDeck.Restack();

        Debug.Log("Deck Built & Shuffled Successfully.");
    }
}
