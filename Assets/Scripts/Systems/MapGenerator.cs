using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[DefaultExecutionOrder(-10)]
[RequireComponent(typeof(Tilemap))]
public class MapGenerator : MonoBehaviour
{
    [SerializeField] MapConfig config;
    [SerializeField] HexTile grassTile;
    [SerializeField] HexTile waterTile;

    Tilemap _tilemap;

    static readonly Vector3Int Center = Vector3Int.zero;

    void Awake()
    {
        _tilemap = GetComponent<Tilemap>();
    }

    void Start()
    {
        Generate();
    }

    public void Generate()
    {
        _tilemap.ClearAllTiles();

        int seed = config.seed == 0 ? Random.Range(1, int.MaxValue) : config.seed;
        Random.InitState(seed);

        // --- Step 1: classify every cell in the bounding scan area ---
        var islandCells = new List<Vector3Int>();
        var borderCells = new List<Vector3Int>();
        var clearZone = new HashSet<Vector3Int>();

        int scanRadius = config.islandRadius + config.borderWidth;
        for (int x = -scanRadius; x <= scanRadius; x++)
        {
            for (int y = -scanRadius; y <= scanRadius; y++)
            {
                var cell = new Vector3Int(x, y, 0);
                int dist = HexUtils.HexDistance(cell, Center);
                if (dist <= config.islandRadius)
                {
                    islandCells.Add(cell);
                    if (dist <= config.minBaseClearRadius)
                        clearZone.Add(cell);
                }
                else if (dist <= scanRadius)
                {
                    borderCells.Add(cell);
                }
            }
        }

        // --- Step 2: flood-fill cluster water placement ---
        int targetWater = Mathf.RoundToInt(islandCells.Count * config.waterPercent);
        var available = new List<Vector3Int>(islandCells.Count - clearZone.Count);
        foreach (var cell in islandCells)
        {
            if (!clearZone.Contains(cell))
                available.Add(cell);
        }

        Shuffle(available);

        var waterSet = new HashSet<Vector3Int>();
        var frontiers = new List<Queue<Vector3Int>>(config.waterClusterCount);
        var clusterSizes = new List<int>(config.waterClusterCount);

        int clusterCount = Mathf.Min(config.waterClusterCount, available.Count);
        int step = available.Count / clusterCount;
        for (int i = 0; i < clusterCount; i++)
        {
            var seedCell = available[i * step];
            if (waterSet.Add(seedCell))
            {
                var q = new Queue<Vector3Int>();
                q.Enqueue(seedCell);
                frontiers.Add(q);
                clusterSizes.Add(1);
            }
        }

        // Round-robin BFS expansion across clusters until water target is reached
        while (waterSet.Count < targetWater)
        {
            bool anyProgress = false;
            for (int i = 0; i < frontiers.Count; i++)
            {
                if (waterSet.Count >= targetWater) break;
                if (frontiers[i].Count == 0) continue;
                if (config.waterClusterMaxSize > 0 && clusterSizes[i] >= config.waterClusterMaxSize) continue;

                var current = frontiers[i].Dequeue();
                foreach (var neighbor in HexUtils.GetNeighbors(current))
                {
                    if (waterSet.Count >= targetWater) break;
                    if (clearZone.Contains(neighbor)) continue;
                    if (waterSet.Contains(neighbor)) continue;
                    if (HexUtils.HexDistance(neighbor, Center) > config.islandRadius) continue;

                    waterSet.Add(neighbor);
                    frontiers[i].Enqueue(neighbor);
                    clusterSizes[i]++;
                    anyProgress = true;
                }
            }
            if (!anyProgress) break;
        }

        // --- Step 3: batch-paint all tiles ---
        var positions = new Vector3Int[islandCells.Count + borderCells.Count];
        var tiles = new TileBase[positions.Length];

        for (int i = 0; i < islandCells.Count; i++)
        {
            positions[i] = islandCells[i];
            tiles[i] = waterSet.Contains(islandCells[i]) ? waterTile : grassTile;
        }

        int offset = islandCells.Count;
        for (int i = 0; i < borderCells.Count; i++)
        {
            positions[offset + i] = borderCells[i];
            tiles[offset + i] = waterTile;
        }

        _tilemap.SetTiles(positions, tiles);
    }

    static void Shuffle<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }
}
