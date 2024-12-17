using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Storage;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class LevelController : MonoBehaviour
{
    [Header("Event")] [SerializeField] private RaiseHexTileToClearEventSO _raiseHexTileToClear;
    [SerializeField] private RaiseStateEventSO _raiseStateEvent;
    [SerializeField] private OnDroppedHexTileWithMergeTileSO _onDroppedHexTileWithMerge;

    [Header("State")] [SerializeField] private NextLevelState _nextLevelState;
    [SerializeField] private GameOverState _gameOverState;

    [Header("Scripts")] [SerializeField] private HexBaseManager _hexBaseManager;
    [SerializeField] private List<HexBaseSpawner> _hexBaseSpawners;
    [SerializeField] private Transform _transformParent;
    [SerializeField] private int _hexTileClearedCounter = 0;
    private int _hexBaseToClear = 0;
    private int _currentLevel = 0;

    // Using MVP to implement UI
    [SerializeField] private Text _hexTileClearedCounterText;
    [SerializeField] private UIProgressBar _uiProgressBar;

    private void OnEnable()
    {
        _raiseHexTileToClear.OnEventRaised += OnHexTileCleared;
        _onDroppedHexTileWithMerge.OnEventRaised += OnDroppedHexTile;
    }

    private void OnDisable()
    {
        _raiseHexTileToClear.OnEventRaised -= OnHexTileCleared;
        _onDroppedHexTileWithMerge.OnEventRaised -= OnDroppedHexTile;
    }

    private void Start()
    {
        InitialSetup();
    }

    private void InitialSetup()
    {
        GetCurrentLevel();
        SpawnLevelMap();
        _uiProgressBar.SetupProgressBar(_hexBaseToClear);
    }

    private void OnHexTileCleared(int value)
    {
        _hexTileClearedCounter += value;
        _uiProgressBar.UpdateProgress((float)value);
    }

    private void OnDroppedHexTile(bool isMerged)
    {
        if (!isMerged)
        {
            var listHexBase = _hexBaseManager.HexBases;
            bool isBlocked = listHexBase.All(tile => tile.HexTileGroup != null);
            if (isBlocked)
            {
                _raiseStateEvent?.RaiseEvent(EGameState.OutOfSpace);
            }
        }
    }


    private void GetCurrentLevel()
    {
        var userInfo = Db.storage.USER_INFO;
        _currentLevel = userInfo.level;
        _hexTileClearedCounter = 0;
    }

    private void SpawnLevelMap()
    {
        HexBaseSpawner levelSpawner;
        if (_currentLevel >= _hexBaseSpawners.Count)
        {
            int randomLevel = Random.Range(0, _hexBaseSpawners.Count); // Set random level to testing prototype
            levelSpawner = Instantiate(_hexBaseSpawners[randomLevel], _transformParent);
        }
        else
        {
            levelSpawner = Instantiate(_hexBaseSpawners[_currentLevel - 1], _transformParent);
        }

        _hexBaseToClear = levelSpawner.NumberToClearHexTiles;
        _hexBaseManager.SetHexBases(levelSpawner.GetHexTiles());
        _hexBaseManager.InitialSetup();
    }
}