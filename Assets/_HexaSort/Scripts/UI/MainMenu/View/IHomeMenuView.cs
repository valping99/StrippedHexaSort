using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHomeMenuView
{
    void ShowLevelToSelect(int level);
    void ShowGameOverScreen();
    void ShowPauseScreen();
    void ShowGamePlayingScreen();
    void SetSelectLevelButtonListener(UnityEngine.Events.UnityAction action);
}