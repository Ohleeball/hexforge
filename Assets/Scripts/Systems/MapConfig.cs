using UnityEngine;

[CreateAssetMenu(fileName = "MapConfig", menuName = "HexForge/MapConfig")]
public class MapConfig : ScriptableObject
{
    [Header("Island Shape")]
    public int islandRadius = 200;
    public int borderWidth = 5;

    [Header("Water Placement")]
    [Range(0f, 1f)] public float waterPercent = 0.30f;
    public int waterClusterCount = 8;
    public int waterClusterMaxSize = 0;

    [Header("Base Protection")]
    public int minBaseClearRadius = 10;

    [Header("Seed")]
    [Tooltip("0 = random seed each run")]
    public int seed = 0;
}
