using System;
using System.Collections;
using System.Collections.Generic;
using Storage;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NextLevelState : UIState
{
    [SerializeField] private Button _closeButton;
    public NextLevelState(UIStateMachine stateMachine) : base(stateMachine)
    {
        StateMachine = stateMachine;
    }

    private void OnEnable()
    {
        _closeButton.onClick.AddListener(OnChangeNextLevel);
    }

    private void OnDisable()
    {
        _closeButton.onClick.RemoveListener(OnChangeNextLevel);
    }

    private void OnChangeNextLevel()
    {
        AudioManager.Instance.PlaySfxSource(EAudioSfxTracking.Sfx_NextLevel);
        var userInfo = Db.storage.USER_INFO;
        userInfo.level++;
        Db.storage.USER_INFO = userInfo;
        SceneManager.LoadScene("HomeMenuScene"); //Testing (Need refactor)
    }

    public override void Enter()
    {
        AudioManager.Instance.PlaySfxSource(EAudioSfxTracking.Sfx_Popup_Win);
        StatePanel.gameObject.SetActive(true);
        BlockerPanel.SetActive(true);
        Debug.Log($"Entering {this.GetType().Name}");
    }
}