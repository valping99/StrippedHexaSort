using UnityEngine;

public class Utilities
{
    public static Vector3Int OffsetToCube(Vector2Int offset)
    {
        int x = offset.x;
        int z = offset.y - (offset.x + (offset.x & 1)) / 2; // this handles the staggered rows
        int y = -x - z;                                     // x + y + z = 0 for Cube coordinates

        return new Vector3Int(x, y, z);
    }
}