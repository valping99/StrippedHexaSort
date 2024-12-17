using UnityEngine;

public class GameplayState : UIState
{
    public GameplayState(UIStateMachine stateMachine) : base(stateMachine)
    {
        StateMachine = stateMachine;
    }

    public override void Enter()
    {
        BlockerPanel.SetActive(false);
        StatePanel.gameObject.SetActive(true);
        Debug.Log($"Entering {this.GetType().Name}");
    }
}