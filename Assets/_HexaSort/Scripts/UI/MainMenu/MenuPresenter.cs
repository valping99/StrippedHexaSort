using System.Collections;
using Storage;
using UnityEngine.SceneManagement;

public class MenuPresenter
{
    private IHomeMenuView _homeMenuView;
    private HomeMenuModel _homeMenuModel;
    private IStateMachine _stateMachine;

    public MenuPresenter(IHomeMenuView homeMenuView, HomeMenuModel homeMenuModel, IStateMachine stateMachine)
    {
        this._homeMenuView = homeMenuView;
        this._homeMenuModel = homeMenuModel;
        this._stateMachine = stateMachine;
    }

    public void Initialize()
    {
        ShowLevel();
        UpdateView();
        _homeMenuView.SetSelectLevelButtonListener(LoadSceneAsync);
    }

    private void ShowLevel()
    {
        var userInfo = Db.storage.USER_INFO;
        _homeMenuView.ShowLevelToSelect(userInfo.level);
    }

    private void OnPauseButtonClicked()
    {
        if (_homeMenuModel.CurrentState == EGameState.Playing)
        {
            _homeMenuModel.ChangeState(EGameState.Paused);
        }
        else if (_homeMenuModel.CurrentState == EGameState.Paused)
        {
            _homeMenuModel.ChangeState(EGameState.Playing);
        }

        UpdateView();
    }

    private void LoadSceneAsync()
    {
        //Test
        AudioManager.Instance.PlaySfxSource(EAudioSfxTracking.Sfx_ButtonClick);
        SceneManager.LoadScene("MainScene");
    }

    private void UpdateView()
    {
        switch (_homeMenuModel.CurrentState)
        {
            case EGameState.Playing:
                _homeMenuView.ShowGamePlayingScreen();
                break;
            case EGameState.Paused:
                _homeMenuView.ShowPauseScreen();
                break;
            case EGameState.GameOver:
                _homeMenuView.ShowGameOverScreen();
                break;
        }
    }
}