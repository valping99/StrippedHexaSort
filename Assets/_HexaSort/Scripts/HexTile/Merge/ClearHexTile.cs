using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class ClearHexTile : MonoBehaviour
{
    [Header("Event")]
    [SerializeField] private RaiseHexTileToClearEventSO _raiseHexTileToClear;
    [SerializeField] private ReCheckHexTileToMergeEventSO _reCheckHexTileToMerge;
    [SerializeField] private OnClearedHexTileEventSO _onClearHexTileEvent;
    [SerializeField] private OnDroppedHexTileWithMergeTileSO _onDroppedHexTileWithMerge;

    [Header("Scripts")] [SerializeField] private MoveObjectToProgressBar _moveObjectToProgress;
    [SerializeField] private MergeHexTileController _mergeHexTileController;
    [SerializeField] private List<HexTileGroup> _hexTileGroups;
    [SerializeField] private int _numberOfHexTileToClear = 10;
    private bool _isCleared;
    public bool AreClearingHexTiles { get; private set; }

    private void OnEnable()
    {
        _onClearHexTileEvent.OnEventRaised += Clear;
    }

    private void OnDisable()
    {
        _onClearHexTileEvent.OnEventRaised -= Clear;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            Clear();
        }
    }

    public async void Clear()
    {
        AreClearingHexTiles = false;
        _isCleared = false;
        _hexTileGroups = new();
        _hexTileGroups = _mergeHexTileController.HexTileGroups;

        var tasks = new List<UniTask>();

        foreach (var group in _hexTileGroups.ToList())
        {
            var lastColor = group.HexTileBehaviours.LastOrDefault()?.Color;

            var matchingElements = group.HexTileBehaviours
                                        .Where(item => item.Color == lastColor)
                                        .Reverse()
                                        .TakeWhile(x => x.Color == lastColor)
                                        .ToList();

            if (matchingElements.Count >= _numberOfHexTileToClear)
            {
                AreClearingHexTiles = true;
                tasks.Add(ClearAndRemoveHexTiles(group, matchingElements));
            }
        }

        await UniTask.WhenAll(tasks);
        AreClearingHexTiles = false;
        if (_isCleared)
        {
            _reCheckHexTileToMerge?.RaiseEvent();
        }
        else
        {
            _onDroppedHexTileWithMerge?.RaiseEvent(false);
        }
    }

    private async UniTask ClearAndRemoveHexTiles(HexTileGroup group, List<HexTileBehaviour> matchingElements)
    {
        var newHexTileBehaviour = matchingElements.Last();
        foreach (var tile in matchingElements)
        {
            await tile.ClearHexTile();
            group.RemoveHexTileConfiguration(tile);
        }

        _raiseHexTileToClear?.RaiseEvent(matchingElements.Count);
        _moveObjectToProgress.MoveToProgress(newHexTileBehaviour);

        if (!group.HexTileBehaviours.Any())
        {
            group.HexBase.RemoveHexTile(group);
            _hexTileGroups.Remove(group);
        }

        AudioManager.Instance.PlaySfxSource(EAudioSfxTracking.Sfx_GetScore);
        _isCleared = true;
    }
}