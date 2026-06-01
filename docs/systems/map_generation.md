# Map Generation

**Status:** `specced`

## Overview

The map is a procedurally generated island on a hex grid. Generation runs once at the start of each run. The result is a circle of playable tiles surrounded by non-playable water border tiles.

---

## Map Shape

Circular island. All tiles outside the island radius are water and are excluded from the land/water ratio calculation. The island is visually surrounded by water but that border is cosmetic, not part of the playable area.

---

## Tile Types (Phase 1)

| Tile | Walkable | Notes |
|---|---|---|
| Grass | Yes | Default land tile |
| Water | No | Generated within island, plus border surround |

Additional tile types to be added in later phases.

---

## Generation Parameters (MapConfig ScriptableObject)

| Parameter | Default | Notes |
|---|---|---|
| `islandRadius` | 200 | Radius in tiles from center to island edge |
| `waterPercent` | 0.30 | Fraction of island tiles that are water (border excluded) |
| `minBaseClearRadius` | 10 | No water within this radius of the center |
| `seed` | 0 | 0 = random seed each run |

All parameters are exposed in the Inspector via a `MapConfig` ScriptableObject. No generation values are hardcoded in MonoBehaviours.

---

## Generation Algorithm

1. Determine all cell coordinates within `islandRadius` of the center — these are the candidate island tiles.
2. Mark the center zone (`minBaseClearRadius`) as guaranteed grass — skip these in water placement.
3. Run flood-fill clustering on the remaining candidate tiles to place coherent bodies of water totalling `waterPercent` of island tile count.
4. All remaining candidate tiles become grass.
5. All tiles outside `islandRadius` are placed as water border tiles (cosmetic, non-playable).

Flood-fill clustering: seed N random water origin points, then expand each cluster outward to neighbouring tiles until the target water tile count is reached. Cluster count and max cluster size are tunable via `MapConfig`.

| Parameter | Default | Notes |
|---|---|---|
| `waterClusterCount` | 8 | Number of independent water bodies seeded |
| `waterClusterMaxSize` | 0 | 0 = no cap, clusters grow until total water% is hit |

---

## Base Placement

Fixed at map center (cell 0,0). The generator guarantees all tiles within `minBaseClearRadius` are grass before water placement runs. The wall ring and base area are placed on top of these tiles by the scene setup, not by the map generator.

---

## Enemy Spawn Points

Enemies do not spawn at fixed points. Each wave picks a random direction from the island center. The spawner finds the nearest walkable (grass) tile to the island edge along that direction at runtime. Spawn point selection is handled by `WaveManager`, not the map generator.

---

## Walkability

Each tile stores a `isWalkable` bool flag directly on the tile data. Water tiles are always non-walkable. Grass tiles are walkable by default. Buildings and walls may override walkability on their tile at placement time.

---

## Future: Chunk-Based Generation

The current algorithm is fully procedural. A later pass will introduce hand-authored map chunks (e.g. a river crossing, a dense forest clearing, a ruined outpost) that can be sampled and placed during generation for more intentional layouts. The procedural pass runs first; chunks are stamped on top.
