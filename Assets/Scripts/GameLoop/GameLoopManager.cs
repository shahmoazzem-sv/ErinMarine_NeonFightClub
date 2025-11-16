using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon.StructWrapping;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public enum GameState
{
    None,
    HeroCardSelection,
    InitialCardDistribution,
    GameRunning,
    Winner
}

public class GameLoopManager : MonoBehaviour
{
    private GameState currentState;

    // References to other components (Assuming Player/AI scripts exist)
    public Player player;
    public AI ai;

    // --- DECK REFERENCES ---
    public HeroDeck heroDeck { get; set; } // Reference to HeroDeck (stores ICard components now)
    public CardDeck cardDeck { get; set; } // Reference to the main card deck

    // --- HERO CARD ASSET REFERENCES ---
    [Header("Card Asset References")]
    // The base Card Prefab (must have the Card component attached)
    public GameObject cardPrefab;
    // The common Sprite for the back of all cards (Move/Special/Hero)
    public Sprite cardBackFaceSprite;
    // Shuffle Speed (used in HeroCardSelectionState)
    [Tooltip("Speed for visual shuffling (units/sec)")]
    public float shuffleVisualSpeed = 5f;


    // --- HERO CARD PLACEMENT TRANSFORMS ---
    [Header("Hero Card Selection Points")]
    // Spawn Min/Max points for random card spawning during selection
    public Transform heroCardSpawnMinPoint;
    public Transform heroCardSpawnMaxPoint;

    [Header("Player Hero Card Section")]
    [HideInInspector] public HeroCard playerHeroCard;
    // Removed playerHeroCardGameObject
    public Transform playerHeroCardPlacePoint; // Final location for player's chosen hero

    [Header("AI Hero Card Section")]
    [HideInInspector] public HeroCard AIHeroCard;
    // Removed AIHeroCardGameObject
    public Transform AIHeroCardPlacePoint; // Final location for AI's chosen hero


    // UI Panel (required for HeroCardSelectionState)
    [Header("Hero Selection Panel Elements (Legacy UI)")]
    public GameObject heroSelectionUIPanel;
    // Note: The new state ignores HeroSelectionImageOne/Two and uses 3D cards instead.

    [SerializeField] public TMP_Text choseHeroText;


    StateMachine stateMachine = new StateMachine();

    // Called when the game starts
    void Awake()
    {
        // Initialize game state as 'None'
        currentState = GameState.None;
        ChangeState(GameState.HeroCardSelection);
    }

    // Change state function
    public void ChangeState(GameState newState)
    {
        // Update state only if it's different
        if (currentState != newState)
        {
            currentState = newState;

            // Handle state transitions
            switch (currentState)
            {
                case GameState.HeroCardSelection:
                    stateMachine.SetState(new HeroCardSelectionState(this));
                    break;

                case GameState.InitialCardDistribution:
                    // Need to implement the InitialCardDistributionState later
                    // stateMachine.SetState(new InitialCardDistributionState(this));
                    Debug.Log("Entering Initial Card Distribution State (Placeholder)");
                    break;

                default:
                    break;
            }
        }
    }


    // Get the current game state
    public GameState GetCurrentState()
    {
        return currentState;
    }
    void Update()
    {
        stateMachine.Update();
    }




}