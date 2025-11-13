using System;
using UnityEngine;

public class StateMachine
{
    private IState currentState;

    // Set the initial state
    public void SetState(IState newState)
    {
        // Exit current state if it exists
        if (currentState != null)
        {
            currentState.Exit();
        }

        // Enter the new state
        currentState = newState;
        currentState.Enter();
    }

    // Update the current state (called every frame)
    void Update()
    {
        if (currentState != null)
        {
            currentState.Update();
        }
    }
}