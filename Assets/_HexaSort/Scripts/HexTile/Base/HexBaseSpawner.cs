using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexBaseSpawner : MonoBehaviour
{
    [SerializeField] private List<HexBase> _hexTiles;
    [field: SerializeField] public int NumberToClearHexTiles { get; private set; }

    public List<HexBase> GetHexTiles()
    {
        return _hexTiles;
    }
}
