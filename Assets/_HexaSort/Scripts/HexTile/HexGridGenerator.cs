#if UNITY_EDITOR
using System.Collections.Generic;
using Kilosoft.Tools;
using UnityEngine;

public class HexGridGenerator : MonoBehaviour
{
    public GameObject HexPrefab;
    public int GridWidth = 10;
    public int GridHeight = 10;
    public float HexRadius = 1f;
    public bool IsFlatTopped = true;
    public HexBaseManager BaseManager;
    public MergeHexTileController MergeController;

    private float _hexWidth;
    private float _hexHeight;
    private Vector3 _hexOffset;

    [EditorButton("Generate Hex Grid")]
    public void Generation()
    {
        Clear();
        _hexWidth = 2f * HexRadius;
        _hexHeight = Mathf.Sqrt(3f) * HexRadius;

        _hexOffset = new Vector3(_hexWidth * 0.75f, 0f, _hexHeight);

        GenerateHexGrid();
    }

    public void Clear()
    {
        List<UnityEngine.GameObject> hexTiles = new List<UnityEngine.GameObject>();
        for (int i = 0; i < transform.childCount; i++)
        {
            hexTiles.Add(transform.GetChild(i).gameObject);
        }

        foreach (var hex in hexTiles)
        {
            DestroyImmediate(hex, true);
        }
    }

    // Function to generate the grid of hexagons
    void GenerateHexGrid()
    {
        List<HexBase> hexTiles = new List<HexBase>();
        for (int row = 0; row < GridHeight; row++)
        {
            for (int col = 0; col < GridWidth; col++)
            {
                // Calculate the position for the hexagon
                float x = col * _hexWidth * 0.75f;
                float z = row * _hexHeight;

                // Stagger every other row
                if (col % 2 == 1)
                {
                    z -= _hexHeight / 2f;
                }

                // Instantiate hexagons at the calculated position
                Vector3 position = new Vector3(x, 0f, z);
                GameObject hex = Instantiate(HexPrefab, position, Quaternion.identity, transform);
                hex.name = $"HexTile_{col}_{row}";
                HexBase hexBase = hex.GetComponentInChildren<HexBase>();
                // HexBase hexBase = hex.GetComponent<HexBase>(); // Required HexTile Component in prefab
                hexBase.OffsetCoordinate = new Vector2Int(col, row);

                // Convert offset coordinate to cube coordinate
                hexBase.CubeCoordinate = Utilities.OffsetToCube(hexBase.OffsetCoordinate);

                hexTiles.Add(hexBase);

                // Rotate hexagons based on whether they are flat-topped or pointy-topped
                if (IsFlatTopped)
                {
                    hex.transform.rotation = Quaternion.Euler(0, 0f, 0f);
                }
                else
                {
                    hex.transform.rotation = Quaternion.Euler(0, 0f, 90f);
                }
            }
        }
        BaseManager.SetHexBases(hexTiles);
        MergeController.SetHexBases(hexTiles);
    }
}
#endif