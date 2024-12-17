using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HomeMenuView : MonoBehaviour, IHomeMenuView
{
    public Text _levelText;
    public Button _selectLevelButton;
    public GameObject _gameOverPanel;
    public GameObject _pausePanel;

    public void ShowLevelToSelect(int level)
    {
        _levelText.text = $"Level {level}";
    }

    public void ShowGameOverScreen()
    {
        _gameOverPanel.SetActive(true);
        _pausePanel.SetActive(false);
    }

    public void ShowPauseScreen()
    {
        _pausePanel.SetActive(true);
        _gameOverPanel.SetActive(false);
    }

    public void ShowGamePlayingScreen()
    {
        _gameOverPanel.SetActive(false);
        _pausePanel.SetActive(false);
    }

    public void SetSelectLevelButtonListener(UnityEngine.Events.UnityAction action)
    {
        _selectLevelButton.onClick.AddListener(action);
    }
}