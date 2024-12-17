public class HomeMenuModel
{
    public int Score;
    public int Level;
    public EGameState CurrentState;

    public HomeMenuModel()
    {
        Score = 0;
        Level = 0;
        CurrentState = EGameState.Playing; // Default state
    }

    public void IncreaseScore(int amount)
    {
        Score += amount;
    }

    public void IncreaseLevel(int amount)
    {
        Level += amount;
    }

    public void ChangeState(EGameState newState)
    {
        CurrentState = newState;
    }
}

public enum EGameState
{
    Playing = 0,
    Paused = 1,
    GameOver = 2,
    OutOfSpace = 3,
    NextLevel = 4
}