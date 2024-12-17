using System;
using System.Collections.Generic;
using UnityEngine;

public class HexBaseManager : MonoBehaviour
{
    [SerializeField] private MergeHexTileController _mergeHexTileController;
    [SerializeField] private List<HexBase> _hexTiles;
    [SerializeField] private HexTileGroup _hexTileGroup;
    public int NumberHexTileToClear { get; private set; }
    public List<HexBase> HexBases => _hexTiles;
    private Dictionary<Vector3Int, HexBase> _hexTileDictionary;

    public void InitialSetup()
    {
        GetNeighbours();
        _mergeHexTileController.SetHexBases(_hexTiles);
    }

    public void GetNeighbours()
    {
        _hexTileDictionary = new Dictionary<Vector3Int, HexBase>();
        foreach (var hexTile in _hexTiles)
        {
            RegisterTile(hexTile);
        }

        foreach (var hexTile in _hexTiles)
        {
            var neighborsWithDirections = GetNeighborsWithDirections(hexTile);

            List<HexBase> neighbors = new List<HexBase>();
            List<float> directions = new List<float>();

            foreach (var item in neighborsWithDirections)
            {
                neighbors.Add(item.Key);
                directions.Add(item.Value);
            }

            hexTile.HexTile.Neighbors = neighbors;
            hexTile.HexTile.Rotation = directions;
        }
    }


    public void SetHexBases(List<HexBase> hexTiles)
    {
        _hexTiles = new List<HexBase>();
        foreach (var hexTile in hexTiles)
        {
            _hexTiles.Add(hexTile);
        }

        GetNeighbours();
    }

    private void RegisterTile(HexBase hexBase)
    {
        _hexTileDictionary.Add(hexBase.CubeCoordinate, hexBase);
    }

    private List<KeyValuePair<HexBase, float>> GetNeighborsWithDirections(HexBase hexBase)
    {
        List<KeyValuePair<HexBase, float>> neighborsWithDirections = new List<KeyValuePair<HexBase, float>>();

        // Cube neighbor offsets (using Cube coordinates)
        Vector3Int[] neighborOffsets = new Vector3Int[]
        {
            new Vector3Int(1, -1, 0), // +1 x, -1 y, 0 z
            new Vector3Int(1, 0, -1), // +1 x, 0 y, -1 z    
            new Vector3Int(0, -1, 1), // 0 x, -1 y, +1 z         
            new Vector3Int(0, 1, -1), // 0 x, +1 y, -1 z       
            new Vector3Int(-1, 0, 1), // -1 x, 0 y, +1 z        
            new Vector3Int(-1, 1, 0), // -1 x, +1 y, 0 z         
        };

        float[] neighborRotations = new float[]
        {
            240, // +1 x, -1 y, 0 z
            300, // +1 x, 0 y, -1 z    
            180, // 0 x, -1 y, +1 z        
            0,   // 0 x, +1 y, -1 z       
            120, // -1 x, 0 y, +1 z        
            60   // -1 x, +1 y, 0 z     
        };

        for (int i = 0; i < neighborOffsets.Length; i++)
        {
            Vector3Int neighborCoord = hexBase.CubeCoordinate + neighborOffsets[i];

            if (_hexTileDictionary.TryGetValue(neighborCoord, out HexBase neighborTile))
            {
                neighborsWithDirections.Add(new KeyValuePair<HexBase, float>(neighborTile, neighborRotations[i]));
            }
        }

        return neighborsWithDirections;
    }
}