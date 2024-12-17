using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Animations;

public class MergeHexTileController : MonoBehaviour
{
    [SerializeField] private OnDroppedHexTileWithMergeTileSO _onDroppedHexTileWithMerge;
    [SerializeField] private OnClearedHexTileEventSO _onClearHexTileEvent;
    [SerializeField] private ReCheckHexTileToMergeEventSO _reCheckHexTileToMergeEvent;
    [SerializeField] private AxisBasement _axisBasement;
    [SerializeField] private SelectedHexTileGroupSO _selectedHexTileGroup;
    [SerializeField] private ClearHexTile _clearHexTile;
    [SerializeField] private int _milisecondsToDelay = 450;
    private List<HexBase> _baseList = new List<HexBase>();
    private List<HexTileGroup> _hexTileGroups = new List<HexTileGroup>();
    public List<HexTileGroup> HexTileGroups => _hexTileGroups;
    private bool _areMergingHexTileGroups;
    private bool _isMergingHexTile;

    private void OnEnable()
    {
        _selectedHexTileGroup.OnEventRaised += OnDropHexTile;
        _reCheckHexTileToMergeEvent.OnEventRaised += async () => await RecheckHexTileGroups();
    }

    private void OnDisable()
    {
        _selectedHexTileGroup.OnEventRaised -= OnDropHexTile;
        _reCheckHexTileToMergeEvent.OnEventRaised -= async () => await RecheckHexTileGroups();
    }

    private void Start()
    {
        InitializeSetup();
    }

    private async void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            await RecheckHexTileGroups();
        }
    }

    private void InitializeSetup()
    {
        _areMergingHexTileGroups = false;
    }

    private async void OnDropHexTile(HexTileGroup hexTileGroup)
    {
        _hexTileGroups.Add(hexTileGroup);
        if (_areMergingHexTileGroups || _clearHexTile.AreClearingHexTiles) return;
        _areMergingHexTileGroups = true;
        await UniTask.WhenAny(MergeHexTile(hexTileGroup));
    }

    private async UniTask MergeHexTile(HexTileGroup hexTileGroup)
    {
        var hexBase = _baseList.FirstOrDefault(x => x.HexTileGroup == hexTileGroup);
        if (hexBase == null) return;

        await MergeTiles(hexTileGroup);

        _onDroppedHexTileWithMerge?.RaiseEvent(_isMergingHexTile);
        _reCheckHexTileToMergeEvent?.RaiseEvent();
    }

    private async UniTask MergeTiles(HexTileGroup selectedHexTileGroup)
    {
        _isMergingHexTile = false;
        if (selectedHexTileGroup == null) return;
        bool hasUniformColor = selectedHexTileGroup.HasUniformColor();

        await UniTask.WhenAll(RearrangeHexTilesByColor(selectedHexTileGroup.HexBase));

        switch (CountAdjacentMatchingColors(selectedHexTileGroup))
        {
            case 1:
                await UniTask.WhenAll(MergeAdjacentTiles(selectedHexTileGroup, hasUniformColor));
                break;
            case >= 2:
                await UniTask.WhenAll(MergeTilesWithTwoAdjacentGroups(selectedHexTileGroup, hasUniformColor));
                break;
            // case >= 3:
            //     await UniTask.WhenAll(MergeTilesWithThreeAdjacentGroups(selectedHexTileGroup));
            //     break;
            default:
                Debug.Log($"Invalid hex tile group: {selectedHexTileGroup}");
                break;
        }

        await UniTask.Delay(_milisecondsToDelay);
    }

    private async UniTask MergeAdjacentTiles(HexTileGroup targetHexTileGroup, bool hasUniformColor)
    {
        var currentColor = targetHexTileGroup.LastColorOfHexTile();
        bool isMerged = false;
        foreach (var neighbor in targetHexTileGroup.HexBase.HexTile.Neighbors)
        {
            if (neighbor.HexTileGroup != null && neighbor.HexTileGroup.LastColorOfHexTile() == currentColor)
            {
                await MergeTileGroups(neighbor.HexTileGroup, targetHexTileGroup, hasUniformColor);
                break;
            }
        }
    }

    private async UniTask MergeTilesWithTwoAdjacentGroups(HexTileGroup targetHexTileGroup, bool hasUniformColor)
    {
        var currentColor = targetHexTileGroup.LastColorOfHexTile();
        foreach (var neighbor in targetHexTileGroup.HexBase.HexTile.Neighbors)
        {
            if (neighbor.HexTileGroup != null && neighbor.HexTileGroup.LastColorOfHexTile() == currentColor)
            {
                await MergeTileGroupFromOutsideIn(targetHexTileGroup, neighbor.HexTileGroup);
                break;
            }
        }
    }

    private async UniTask RecheckHexTileGroups()
    {
        _isMergingHexTile = false;
        var tileGroupToMerge = _hexTileGroups
                               .Select(tileGroup => new
                               {
                                   TileGroup = tileGroup,
                                   SameColorNeighbours = tileGroup.GetNeighbourColor()
                               })
                               .Where(x => x.SameColorNeighbours > 0)
                               .OrderByDescending(x => x.SameColorNeighbours)
                               .FirstOrDefault();
        if (tileGroupToMerge != null)
        {
            await MergeTiles(tileGroupToMerge.TileGroup);
        }
        else
        {
            foreach (var tileGroup in _hexTileGroups)
            {
                if (tileGroup.HasSameColorOfNeighbour())
                {
                    await MergeTiles(tileGroup);
                    break;
                }
            }
        }

        if (_isMergingHexTile)
        {
            _reCheckHexTileToMergeEvent?.RaiseEvent();
        }
        else
        {
            _onClearHexTileEvent?.RaiseEvent();
            _areMergingHexTileGroups = false;
        }
    }

    private async UniTask MergeTilesWithThreeAdjacentGroups(HexTileGroup targetHexTileGroup)
    {
        var currentColor = targetHexTileGroup.LastColorOfHexTile();
        int caseIndex = 0;
        foreach (var neighbor in targetHexTileGroup.HexBase.HexTile.Neighbors)
        {
            if (neighbor.HexTileGroup != null && neighbor.HexTileGroup.LastColorOfHexTile() == currentColor)
            {
                await MergeHexTileGroups(targetHexTileGroup, neighbor.HexTileGroup, caseIndex);
                caseIndex++;
            }
        }
    }

    private async UniTask RearrangeHexTilesByColor(HexBase hexBase)
    {
        await MoveUniformColorElementsToEnd(hexBase);
    }

    private async UniTask MergeTileGroups(HexTileGroup selectedHexTileGroup, HexTileGroup otherHexTileGroup,
        bool hasUniformColor)
    {
        var newHexTileGroup = selectedHexTileGroup.HasUniformColor() ? selectedHexTileGroup : otherHexTileGroup;
        var targetHexTile = selectedHexTileGroup.HasUniformColor() ? otherHexTileGroup : selectedHexTileGroup;

        if (CanMergeHexTileColor(selectedHexTileGroup, otherHexTileGroup))
        {
            for (int i = targetHexTile.HexTileBehaviours.Count - 1; i >= 0; i--)
            {
                var mergeTask = MergeHexagons(targetHexTile, newHexTileGroup);
                await UniTask.WhenAll(mergeTask);
            }
        }
    }

    private async UniTask MergeHexTileGroups(HexTileGroup insideHexTileGroup, HexTileGroup outsideHexTileGroup,
        int caseIndex)
    {
        if (CanMergeHexTileColor(insideHexTileGroup, outsideHexTileGroup))
        {
            if (insideHexTileGroup.HasUniformColor())
            {
                await MergeHexagonsForGroup(outsideHexTileGroup, insideHexTileGroup);
            }
            else
            {
                if (caseIndex <= 1)
                {
                    await MergeHexagonsForGroup(outsideHexTileGroup, insideHexTileGroup);
                }
                else
                {
                    await MergeHexagonsForGroup(insideHexTileGroup, outsideHexTileGroup);
                }
            }
        }
    }

    private async UniTask MergeTileGroupFromOutsideIn(HexTileGroup insideHexTileGroup, HexTileGroup outsideHexTileGroup)
    {
        if (CanMergeHexTileColor(insideHexTileGroup, outsideHexTileGroup))
        {
            for (int i = outsideHexTileGroup.HexTileBehaviours.Count - 1; i >= 0; i--)
            {
                var mergeTask = MergeHexagons(outsideHexTileGroup, insideHexTileGroup);
                await UniTask.WhenAll(mergeTask);
            }
        }
    }

    private async UniTask MergeHexagonsForGroup(HexTileGroup sourceGroup, HexTileGroup targetGroup)
    {
        for (int i = sourceGroup.HexTileBehaviours.Count - 1; i >= 0; i--)
        {
            var mergeTask = MergeHexagons(sourceGroup, targetGroup);
            await UniTask.WhenAll(mergeTask);
        }
    }

    private async UniTask MoveUniformColorElementsToEnd(HexBase hexBaseList)
    {
        List<HexBase> nonUniformColorList = new List<HexBase>();
        List<HexBase> uniformColorList = new List<HexBase>();

        foreach (var hexBase in hexBaseList.HexTile.Neighbors)
        {
            if (hexBase.HexTileGroup != null && hexBase.HexTileGroup.HasUniformColor())
            {
                uniformColorList.Add(hexBase);
            }
            else
            {
                nonUniformColorList.Add(hexBase);
            }
        }

        hexBaseList.HexTile.Neighbors = new List<HexBase>(nonUniformColorList);
        hexBaseList.HexTile.Neighbors.AddRange(uniformColorList);
    }

    private int CountAdjacentMatchingColors(HexTileGroup hexTileGroup)
    {
        int matchingColorCount = 0;
        ETileColor targetColor = hexTileGroup.HexTileBehaviours.LastOrDefault().Color;

        foreach (var neighbor in hexTileGroup.HexBase.HexTile.Neighbors)
        {
            var neighborGroup = neighbor.HexTileGroup;
            if (neighborGroup != null)
            {
                var neighborColor = neighborGroup.HexTileBehaviours.LastOrDefault()?.Color;
                if (neighborColor == targetColor)
                {
                    matchingColorCount++;
                }
            }
        }

        return matchingColorCount;
    }

    private bool CanMergeHexTileColor(HexTileGroup currentTileGroup, HexTileGroup targetTileGroup)
    {
        if (currentTileGroup == null || targetTileGroup == null) return false;
        var lastObjectInFirstGroup = currentTileGroup.HexTileBehaviours.LastOrDefault();
        var lastObjectInTargetGroup = targetTileGroup.HexTileBehaviours.LastOrDefault();
        return lastObjectInFirstGroup.Color == lastObjectInTargetGroup.Color;
    }

    private async UniTask MergeHexagons(HexTileGroup firstHexTileGroup, HexTileGroup targetHexTileGroup)
    {
        if (ValidToMerge(firstHexTileGroup, targetHexTileGroup)) return;

        if (CanMergeHexTileColor(firstHexTileGroup, targetHexTileGroup))
        {
            var lastObjectInFirstGroup = firstHexTileGroup.HexTileBehaviours.LastOrDefault();
            var lastObjectInTargetGroup = targetHexTileGroup.HexTileBehaviours.LastOrDefault();

            var higherObjectPosition = GetHigherObjectPosition(firstHexTileGroup, targetHexTileGroup,
                lastObjectInFirstGroup, lastObjectInTargetGroup);

            await RotateHexTileGroups(firstHexTileGroup, targetHexTileGroup);
            await TransferTileToTargetGroup(firstHexTileGroup, targetHexTileGroup, lastObjectInFirstGroup);
            await PerformTileJumpAndFlip(lastObjectInFirstGroup, lastObjectInTargetGroup, higherObjectPosition);
            await CleanupAfterMerge(firstHexTileGroup);
        }
    }

    private bool ValidToMerge(HexTileGroup firstHexTileGroup, HexTileGroup targetHexTileGroup)
    {
        return firstHexTileGroup.HexTileBehaviours.Count == 0 || targetHexTileGroup.HexTileBehaviours.Count == 0;
    }

    private Vector3 GetHigherObjectPosition(HexTileGroup firstHexTileGroup, HexTileGroup targetHexTileGroup,
        HexTileBehaviour lastObjectInFirstGroup, HexTileBehaviour lastObjectInTargetGroup)
    {
        return firstHexTileGroup.HexTileBehaviours.Count >= targetHexTileGroup.HexTileBehaviours.Count
            ? lastObjectInFirstGroup.OriginalPosition
            : lastObjectInTargetGroup.OriginalPosition;
    }

    private async UniTask RotateHexTileGroups(HexTileGroup firstHexTileGroup, HexTileGroup targetHexTileGroup)
    {
        await RotateBase(firstHexTileGroup, targetHexTileGroup);
        await RotateBase(targetHexTileGroup, firstHexTileGroup);
    }

    private async UniTask RotateBase(HexTileGroup sourceGroup, HexTileGroup targetGroup)
    {
        HexBase sourceBase = sourceGroup.HexBase;
        HexBase targetBase = targetGroup.HexBase;

        Vector3 direction = targetBase.transform.position - sourceBase.transform.position;
        float angle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
        angle = Mathf.Round(angle / 60) * 60;

        Quaternion rotation = Quaternion.Euler(0, 90, angle - _axisBasement.AxisOffset);
        sourceBase.transform.localRotation = rotation;
    }

    private async UniTask TransferTileToTargetGroup(HexTileGroup firstHexTileGroup, HexTileGroup targetHexTileGroup,
        HexTileBehaviour tile)
    {
        firstHexTileGroup.RemoveHexTileConfiguration(tile);
        targetHexTileGroup.AddHexTileConfiguration(tile);

        tile.transform.SetParent(targetHexTileGroup.transform);
    }

    private async UniTask PerformTileJumpAndFlip(HexTileBehaviour lastObjectInFirstGroup,
        HexTileBehaviour lastObjectInTargetGroup, Vector3 higherObjectPosition)
    {
        var jumpAndFlipToTarget =
            lastObjectInFirstGroup.JumpAndFlipToTarget(lastObjectInTargetGroup, higherObjectPosition);
        await UniTask.WhenAll(jumpAndFlipToTarget);
    }

    private async UniTask CleanupAfterMerge(HexTileGroup firstHexTileGroup)
    {
        if (firstHexTileGroup.HexTileBehaviours.Count == 0)
        {
            firstHexTileGroup.HexBase.RemoveHexTile(firstHexTileGroup);
            _hexTileGroups.Remove(firstHexTileGroup);
        }

        _isMergingHexTile = true;
    }


    public void SetHexBases(List<HexBase> hexTiles)
    {
        _baseList = new List<HexBase>();
        foreach (var hexTile in hexTiles)
        {
            _baseList.Add(hexTile);
        }
    }
}