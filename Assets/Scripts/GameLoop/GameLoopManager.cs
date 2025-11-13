using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon.StructWrapping;
using JetBrains.Annotations;
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

    public HeroDeck heroDeck { get; set; } // Reference to HeroDeck

    public List<Card> heroCards = new List<Card>();

    StateMachine stateMachine = new StateMachine();

    [Header("Hero Selection Panel Enelemts")]


    public GameObject heroSelectionUIPanel;
    public Image HeroSelectionImageOne;
    public Image HeroSelectionImageTwo;

    [Header("Player Hero Card Section")]
    [HideInInspector] public HeroCard playerHeroCard;
    public GameObject playerHeroCardGameObject;
    public Transform playerHeroCardPlacePoint;

    [Header("AI Hero Card Section")]
    [HideInInspector] public HeroCard AIHeroCard;
    public GameObject AIHeroCardGameObject;
    public Transform AIHeroCardPlacePoint;
    [SerializeField] public Player player;
    [SerializeField] public AI ai;
    // Called when the game starts





    // Add this line to GameLoopManager.cs, alongside 'heroDeck':
    public CardDeck cardDeck { get; set; }
    void Awake()
    {
        // Initialize game state as 'None'
        currentState = GameState.None;
        ChangeState(GameState.HeroCardSelection);
    }
    void Start()
    {

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
                    stateMachine.SetState(new InitialCardDistributionState(this));
                    break;

                // Additional cases for other states
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