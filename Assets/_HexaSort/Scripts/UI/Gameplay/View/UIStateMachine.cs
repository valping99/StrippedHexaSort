using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIStateMachine : MonoBehaviour
{
    [Header("Current State")] [SerializeField]
    private UIState _currentState;

    [SerializeField] private EGameState _currentGameState;

    [Header("State")] 
    [SerializeField] private UIState _gameplayState;
    [SerializeField] private UIState _nextLevelState;
    [SerializeField] private UIState _outOfSpaceState;
    [SerializeField] private UIState _gameOverState;

    [Header("Event Raised")] [SerializeField]
    private RaiseStateEventSO _gameplayStateEventSO;

    private void OnEnable()
    {
        _gameplayStateEventSO.OnEventRaised += UpdateStateRaised;
    }

    private void OnDisable()
    {
        _gameplayStateEventSO.OnEventRaised -= UpdateStateRaised;
    }

    void Start()
    {
        InitialSetup();
    }

    private void InitialSetup()
    {
        _gameplayState.GetState(this);
        _nextLevelState.GetState(this);
        _outOfSpaceState.GetState(this);
        _gameOverState.GetState(this);
        ChangeState(EGameState.Playing);
    }

    private void UpdateStateRaised(EGameState state)
    {
        ChangeState(state);
    }

    public void ChangeState(EGameState state)
    {
        var newState = GetState(state);
        if (_currentState == newState || newState == null)
        {
            Debug.Log($"Can't change state to {_currentState}");
            return;
        }

        _currentState?.Exit();
        _currentState = newState;
        _currentState.Enter();
    }

    private UIState GetState(EGameState state)
    {
        switch (state)
        {
            case EGameState.Playing:
                _currentGameState = EGameState.Playing;
                return _gameplayState;
            case EGameState.GameOver:
                _currentGameState = EGameState.GameOver;
                return _gameOverState;
            case EGameState.NextLevel:
                _currentGameState = EGameState.NextLevel;
                return _nextLevelState;
            case EGameState.OutOfSpace:
                _currentGameState = EGameState.OutOfSpace;
                return _outOfSpaceState;
            default:
                break;
        }

        return null;
    }
}