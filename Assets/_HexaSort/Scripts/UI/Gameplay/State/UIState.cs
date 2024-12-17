using UnityEngine;

public abstract class UIState : MonoBehaviour
{
    protected UIStateMachine StateMachine;
    public GameObject StatePanel;
    public GameObject BlockerPanel;

    public UIState(UIStateMachine stateMachine)
    {
        this.StateMachine = stateMachine;
    }

    public void GetState(UIStateMachine stateMachine)
    {
        StateMachine = stateMachine;
    }

    public virtual void Enter()
    {
        BlockerPanel.SetActive(true);
        StatePanel.gameObject.SetActive(true);
        Debug.Log($"Entering {this.GetType().Name}");
    }

    public virtual void Exit()
    {
        StatePanel.gameObject.SetActive(false);
        Debug.Log($"Exiting {this.GetType().Name}");
    }
}