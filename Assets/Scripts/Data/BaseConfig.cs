using UnityEngine;

[CreateAssetMenu(fileName = "BaseConfig", menuName = "HexForge/BaseConfig")]
public class BaseConfig : ScriptableObject
{
    public int initialTileCount = 3;
    public int tilesPerExpansion = 6;
    public int maxExpansions = 5;
}
