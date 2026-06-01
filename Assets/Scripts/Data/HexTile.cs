using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "HexTile", menuName = "HexForge/HexTile")]
public class HexTile : TileBase
{
    public bool isWalkable;
    public GameObject prefab;

    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
    {
        tileData.gameObject = prefab;
        tileData.flags = TileFlags.LockTransform;
    }
}
