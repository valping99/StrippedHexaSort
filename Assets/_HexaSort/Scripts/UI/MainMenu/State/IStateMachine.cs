public interface IStateMachine
{
    void SwitchState(EGameState newState);
    EGameState CurrentState { get; }
}