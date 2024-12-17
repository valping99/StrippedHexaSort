using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OutOfSpaceState : UIState
{
    [SerializeField] private HexTileReviver _hexTileReviver;
    [SerializeField] private Button _reviveButton;
    [SerializeField] private Button _closeButton;
    [SerializeField] private int _numberOfSpawn; // Testing
    public OutOfSpaceState(UIStateMachine stateMachine) : base(stateMachine)
    {
        StateMachine = stateMachine;
    }

    private void OnEnable()
    {
        _closeButton.onClick.AddListener(OnClose);
        _reviveButton.onClick.AddListener(OnRevive);
    }

    private void OnDisable()
    {
        _closeButton.onClick.RemoveListener(OnClose);
        _reviveButton.onClick.RemoveListener(OnRevive);
    }

    private void OnClose()
    {
        StateMachine.ChangeState(EGameState.GameOver);
    }

    private void OnRevive()
    {
        StateMachine.ChangeState(EGameState.Playing);
        _hexTileReviver.ReviveBySlot(_numberOfSpawn);
    }
}