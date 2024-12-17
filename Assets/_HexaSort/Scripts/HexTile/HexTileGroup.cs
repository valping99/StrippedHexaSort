using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class HexTileGroup : MonoBehaviour
{
    [SerializeField] private List<HexTileBehaviour> _hexTileBehaviours;
    [SerializeField] private DragAndDrop _inputDragAndDrop;
    public List<HexTileBehaviour> HexTileBehaviours => _hexTileBehaviours;
    public HexBase HexBase { get; private set; }
    public bool IsDropped { get; private set; }

    private void Start()
    {
        IsDropped = false;
    }

    public void ToggleInputAction(bool isToggle)
    {
        _inputDragAndDrop.enabled = isToggle;
    }

    public void ClearHexTileConfigurations()
    {
        _hexTileBehaviours.Clear();
        ToggleInputAction(false);
    }

    public void AddHexTileConfiguration(HexTileBehaviour hexTileConfiguration)
    {
        _hexTileBehaviours.Add(hexTileConfiguration);
    }

    public void RemoveHexTileConfiguration(HexTileBehaviour hexTileConfiguration)
    {
        _hexTileBehaviours.Remove(hexTileConfiguration);
    }

    public void DropHexTile(HexBase newBase, bool isAvailable)
    {
        IsDropped = true;
        if (isAvailable)
        {
            HexBase = newBase;
        }
        else
        {
            HexBase = null;
        }
    }

    public bool HasUniformColor()
    {
        return _hexTileBehaviours.Any() &&
               _hexTileBehaviours.All(obj => obj.Color == _hexTileBehaviours[0].Color);
    }

    public bool HasSameColorOfNeighbour()
    {
        if (_hexTileBehaviours.Count <= 0) return false;
        var currentColor = _hexTileBehaviours.Last().Color;
        foreach (var neighbour in HexBase.HexTile.Neighbors)
        {
            if (neighbour.HexTileGroup != null)
            {
                var neighbourColor = neighbour.HexTileGroup._hexTileBehaviours.Last().Color;
                if (neighbourColor == currentColor) return true;
            }
        }

        return false;
    }

    public int GetNeighbourColor()
    {
        var currentColor = LastColorOfHexTile();
        int neighbourColor = 0;
        foreach (var neighbour in HexBase.HexTile.Neighbors)
        {
            if (neighbour.HexTileGroup != null)
            {
                if (currentColor == neighbour.HexTileGroup.LastColorOfHexTile())
                {
                    neighbourColor++;
                }
            }
        }

        return neighbourColor;
    }

    public ETileColor LastColorOfHexTile()
    {
        if (_hexTileBehaviours.Count > 0)
            return _hexTileBehaviours.Last().Color;
        else
            return ETileColor.Default;
    }
}