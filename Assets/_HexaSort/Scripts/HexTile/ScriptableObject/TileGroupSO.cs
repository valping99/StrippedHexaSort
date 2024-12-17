using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewGroup", menuName = "HexTile/Group")]
public class TileGroupSO : ScriptableObject
{
    public ETileColor TileColor;
    public Material TileMaterial;
    [Range(1, 5)] public int Min;
    [Range(5, 20)] public int Max;
}
