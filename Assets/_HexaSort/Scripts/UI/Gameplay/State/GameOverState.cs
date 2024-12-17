using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverState : UIState
{
    [SerializeField] private Button _closeButton;
    public GameOverState(UIStateMachine stateMachine) : base(stateMachine)
    {
        StateMachine = stateMachine;
    }

    private void OnEnable()
    {
        _closeButton.onClick.AddListener(OnReturnMainMenu);
    }

    private void OnDisable()
    {
        _closeButton.onClick.RemoveListener(OnReturnMainMenu);
    }

    private void OnReturnMainMenu()
    {
        AudioManager.Instance.PlaySfxSource(EAudioSfxTracking.Sfx_ButtonClick);
        SceneManager.LoadScene("HomeMenuScene"); //Testing (Need refactor)
    }

    public override void Enter()
    {
        AudioManager.Instance.PlaySfxSource(EAudioSfxTracking.Sfx_Popup_Fail);
        StatePanel.gameObject.SetActive(true);
        BlockerPanel.SetActive(true);
        Debug.Log($"Entering {this.GetType().Name}");
    }
}