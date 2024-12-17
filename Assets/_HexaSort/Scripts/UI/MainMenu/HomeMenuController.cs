using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomeMenuController : MonoBehaviour
{
    public HomeMenuView HomeMenuView;
    private HomeMenuModel _homeMenuModel;
    private MenuPresenter _homeMenuPresenter;
    private IStateMachine _stateMachine;

    void Start()
    {
        _homeMenuModel = new HomeMenuModel();
        _stateMachine = new StateMachine();
        _homeMenuPresenter = new MenuPresenter(HomeMenuView, _homeMenuModel, _stateMachine);

        _homeMenuPresenter.Initialize();
    }
}
