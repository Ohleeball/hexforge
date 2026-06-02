using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// Places the starting base on the hex Tilemap at scene start.
/// Execution order is higher than MapGenerator so the map is fully painted first.
/// </summary>
[DefaultExecutionOrder(10)]
[RequireComponent(typeof(Tilemap))]
public class BaseSpawner : MonoBehaviour
{
    [SerializeField] HexTile castleTile;
    [SerializeField] HexTile stoneTile;
    [SerializeField] HexTile wallTile;
    [SerializeField] BaseConfig config;
    [SerializeField] TileRegistry tileRegistry;

    [Header("Spawn Animation")]
    [SerializeField] float tileStagger = 0.08f;
    [SerializeField] float expandDuration = 0.15f;
    [SerializeField] float contractDuration = 0.10f;

    Tilemap _tilemap;
    static readonly Vector3Int Center = Vector3Int.zero;

    // HexUtils.GetNeighbors returns the 6 true ring-1 neighbours of a cell using
    // Unity's Hexagon (odd-r) layout. They come back in CubeDirections order;
    // CwFromTopRight just reorders that set so we pick a contiguous arc rather than
    // a scattered subset when initialTileCount < 6. Every entry is still a direct
    // neighbour, so any subset is guaranteed to be exactly 1 cell from center.
    // NOTE: the exact "clockwise from top-right" visual ordering depends on world
    // handedness (Cell Swizzle XZY can mirror winding) — confirm visually if the
    // arc looks reversed. Adjacency (distance == 1) does not depend on it.
    static readonly int[] CwFromTopRight = { 1, 0, 5, 4, 3, 2 };

    void Awake()
    {
        _tilemap = GetComponent<Tilemap>();
    }

    void Start()
    {
        StartCoroutine(SpawnBase());
    }

    IEnumerator SpawnBase()
    {
        // ── 1. Stone tile cells ─────────────────────────────────────────────
        // Bug 2 fix: algorithm is strictly "get 6 direct neighbors of center,
        // reorder clockwise from top-right via the axial lookup, take the first
        // initialTileCount". No walking outward from previously placed tiles.
        // Max stone tiles is 6 (the number of direct neighbors of center).

        var baseCells = new HashSet<Vector3Int> { Center };
        Vector3Int[] rawNeighbors = HexUtils.GetNeighbors(Center);

        // Log the castle center and its 6 computed neighbours (cell + world pos)
        // so the neighbour vectors can be checked against the Tilemap's actual
        // grid geometry — HexDistance alone can't verify this (see note below).
        Debug.Log($"[BaseSpawner] Castle center cell={Center}  world={_tilemap.CellToWorld(Center)}");
        for (int i = 0; i < 6; i++)
            Debug.Log($"[BaseSpawner] Neighbor[{i}] cell={rawNeighbors[i]}  world={_tilemap.CellToWorld(rawNeighbors[i])}  dist={HexUtils.HexDistance(rawNeighbors[i], Center)}");

        var clockwiseNeighbors = new Vector3Int[6];
        for (int i = 0; i < 6; i++)
            clockwiseNeighbors[i] = rawNeighbors[CwFromTopRight[i]];

        var stoneCells = new List<Vector3Int>();
        int stoneCount = Mathf.Clamp(config.initialTileCount, 0, 6);
        for (int i = 0; i < stoneCount; i++)
        {
            stoneCells.Add(clockwiseNeighbors[i]);
            baseCells.Add(clockwiseNeighbors[i]);
            // dist must be 1 for every stone cell. NOTE: HexDistance runs through the
            // same odd-r conversion as GetNeighbors, so this confirms internal
            // consistency, not the match to Unity's grid — for that, compare the
            // world positions logged above against the Tilemap's grid overlay.
            Debug.Log($"[BaseSpawner] Stone[{i}] cell={clockwiseNeighbors[i]}  world={_tilemap.CellToWorld(clockwiseNeighbors[i])}  dist={HexUtils.HexDistance(clockwiseNeighbors[i], Center)}");
        }

        // ── 2. Wall cells ───────────────────────────────────────────────────
        // Collect unique non-base neighbors of every base cell into a HashSet,
        // so cells bordering multiple base tiles are deduplicated before placement.
        var wallSet = new HashSet<Vector3Int>();
        foreach (Vector3Int cell in baseCells)
        {
            foreach (Vector3Int nb in HexUtils.GetNeighbors(cell))
            {
                if (!baseCells.Contains(nb))
                    wallSet.Add(nb);
            }
        }

        // Sort wall cells clockwise. For the wall ring the axial lookup doesn't
        // apply directly (cells are at varying ring distances), so use Atan2 on
        // world offsets. But the vertical axis depends on the Tilemap plane:
        //   XZ-plane tilemap: offsets have z varying, y≈0 → use Atan2(z, x)
        //   XY-plane tilemap: offsets have y varying, z≈0 → use Atan2(y, x)
        // Detect once by comparing max absolute y vs z across the ring-1 neighbors.
        Vector3 worldCenter = _tilemap.CellToWorld(Center);
        float maxY = 0f, maxZ = 0f;
        foreach (Vector3Int nb in rawNeighbors)
        {
            Vector3 off = _tilemap.CellToWorld(nb) - worldCenter;
            maxY = Mathf.Max(maxY, Mathf.Abs(off.y));
            maxZ = Mathf.Max(maxZ, Mathf.Abs(off.z));
        }
        bool isXYPlane = maxY > maxZ;

        var wallCells = new List<Vector3Int>(wallSet);
        wallCells.Sort((a, b) =>
        {
            Vector3 wa = _tilemap.CellToWorld(a) - worldCenter;
            Vector3 wb = _tilemap.CellToWorld(b) - worldCenter;
            float angleA = isXYPlane ? Mathf.Atan2(wa.y, wa.x) : Mathf.Atan2(wa.z, wa.x);
            float angleB = isXYPlane ? Mathf.Atan2(wb.y, wb.x) : Mathf.Atan2(wb.z, wb.x);
            return angleB.CompareTo(angleA);
        });

        // ── 3. Place tiles on the Tilemap ──────────────────────────────────
        // Build cell→tile map here; needed by the Bug 1 scale fix in step 5.
        var cellToTile = new Dictionary<Vector3Int, HexTile>();

        cellToTile[Center] = castleTile;
        _tilemap.SetTile(Center, castleTile);
        tileRegistry.Register(Center, false);

        foreach (Vector3Int cell in stoneCells)
        {
            cellToTile[cell] = stoneTile;
            _tilemap.SetTile(cell, stoneTile);
            tileRegistry.Register(cell, true);
        }

        foreach (Vector3Int cell in wallCells)
        {
            cellToTile[cell] = wallTile;
            _tilemap.SetTile(cell, wallTile);
            tileRegistry.Register(cell, false);
        }

        // ── 4. Animation order ─────────────────────────────────────────────
        var animOrder = new List<Vector3Int>(1 + stoneCells.Count + wallCells.Count);
        animOrder.Add(Center);
        animOrder.AddRange(stoneCells);
        animOrder.AddRange(wallCells);

        // Wait one frame for Tilemap to instantiate prefab GameObjects
        yield return null;

        // ── 5. Collect objects and capture intended scales ──────────────────
        // Bug 1 fix: HexTile.GetTileData sets TileFlags.LockTransform without
        // setting tileData.transform, so it defaults to Matrix4x4.identity.
        // The Tilemap therefore forces every instantiated object to scale (1,1,1)
        // regardless of what the prefab asset specifies.
        // go.transform.localScale after instantiation is always (1,1,1) — not the
        // prefab's intended scale.
        // Fix: read originalScale from tile.prefab.transform.localScale (the prefab
        // asset's own configured scale), then animate back to that value.
        var objects = new List<(GameObject go, Vector3 originalScale)>(animOrder.Count);
        foreach (Vector3Int cell in animOrder)
        {
            GameObject go = _tilemap.GetInstantiatedObject(cell);
            if (go == null) continue;

            // Log the Tilemap-overridden scale so the forced value is visible in Console.
            Debug.Log($"[BaseSpawner] Instantiated {cell}: tilemap-forced localScale={go.transform.localScale}");

            HexTile tile = cellToTile.TryGetValue(cell, out var t) ? t : null;
            Vector3 originalScale = (tile != null && tile.prefab != null)
                ? tile.prefab.transform.localScale
                : go.transform.localScale;

            go.transform.localScale = Vector3.zero;
            objects.Add((go, originalScale));
        }

        // ── 6. Animate in clockwise order with stagger ─────────────────────
        foreach (var (go, originalScale) in objects)
        {
            StartCoroutine(AnimateTile(go, originalScale));
            yield return new WaitForSeconds(tileStagger);
        }
    }

    /// <summary>Scales a tile from 0 → originalScale*1.3 → originalScale.</summary>
    IEnumerator AnimateTile(GameObject go, Vector3 originalScale)
    {
        Vector3 peakScale = originalScale * 1.3f;

        float elapsed = 0f;
        while (elapsed < expandDuration)
        {
            elapsed += Time.deltaTime;
            go.transform.localScale = Vector3.Lerp(Vector3.zero, peakScale, Mathf.Clamp01(elapsed / expandDuration));
            yield return null;
        }

        elapsed = 0f;
        while (elapsed < contractDuration)
        {
            elapsed += Time.deltaTime;
            go.transform.localScale = Vector3.Lerp(peakScale, originalScale, Mathf.Clamp01(elapsed / contractDuration));
            yield return null;
        }

        go.transform.localScale = originalScale;
    }
}
