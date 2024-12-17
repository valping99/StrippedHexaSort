using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class HexTile
{
    public List<float> Rotation;
    public List<HexBase> Neighbors;
}
public class HexBase : MonoBehaviour
{
    [SerializeField] private Material _defaultMaterial;
    [SerializeField] private Material _selectedMaterial;
    [SerializeField] private MeshRenderer _meshRenderer;
    [SerializeField] private SelectedEventSO _selectedEvent;
    [SerializeField] private HexTileGroup _hexTileGroup;

    public HexTileGroup HexTileGroup => _hexTileGroup;

    public Vector2Int OffsetCoordinate;
    public Vector3Int CubeCoordinate;
    public HexTile HexTile;

    private bool _isDropped;

    private void OnEnable()
    {
        _selectedEvent.OnEventRaised += OnHexBaseSelected;
    }

    private void OnDisable()
    {
        _selectedEvent.OnEventRaised -= OnHexBaseSelected;
    }

    private void OnHexBaseSelected(HexBase hexBase)
    {
        bool isSelected = hexBase == this;
        _meshRenderer.material = isSelected ? _selectedMaterial : _defaultMaterial;
    }

    public bool IsDropped()
    {
        return _isDropped;
    }

    public void SetDropped(HexTileGroup hexTileGroup, bool dropped)
    {
        _hexTileGroup = hexTileGroup;
        _isDropped = dropped;
    }

    public void RemoveHexTile(HexTileGroup hexTileGroup)
    {
        _meshRenderer.material = _defaultMaterial;
        _isDropped = false;
        if (hexTileGroup != null)
        {
            _hexTileGroup = null;
            Destroy(hexTileGroup.gameObject);
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        foreach (var neighbor in HexTile.Neighbors)
        {
            if (neighbor == null) return;
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(transform.position, .1f);

            Gizmos.color = Color.white;
            Gizmos.DrawLine(transform.position, neighbor.transform.position);
        }
    }
#endif
}

public enum ETileColor
{
    Default = 0,
    Red = 1,
    Blue = 2,
    Green = 3,
    Brown = 4,
    Pink = 5,
    Purple = 6,
    White = 7
}