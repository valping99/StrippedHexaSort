using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine : IStateMachine
{
    private EGameState _currentState;

    public EGameState CurrentState => _currentState;

    public void SwitchState(EGameState newState)
    {
        _currentState = newState;
    }
}