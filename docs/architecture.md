# Architecture

## Engine

**Unity 3D** (LTS version recommended — 2022.3 or later)
- Familiar tooling for the team
- 3D pipeline with isometric camera, using Tilemap for hex grid management
- Asset pack import is straightforward for 3D assets
- Use the **Universal Render Pipeline (URP)** — better performance and shader support than Built-in

## Camera

Fixed **isometric camera** — 3D scene, orthographic projection, camera locked at a 45° horizontal rotation and approximately 30° vertical angle. Camera does not rotate during play. Zoom may be adjusted but angle is fixed.

The game plays on a flat 3D plane (Y = 0 for the ground). All gameplay logic treats position as 2D (X/Z). Height (Y) is used only for visual layering — buildings and characters sit above the tile plane.

---

## Project Structure

```
Assets/
├── Scenes/
│   ├── World/          # Map, hex grid, zone nodes
│   ├── Base/           # Wall rings, building grid, planning mode
│   ├── Player/         # Player character, movement, combat
│   ├── Enemies/        # Enemy types, spawner
│   ├── Buildings/      # Individual building prefabs per type
│   └── UI/             # HUD, planning mode UI, blueprint hand
├── Scripts/
│   ├── Systems/        # Economy, income tick, leveling, interest
│   ├── Data/           # BuildingData ScriptableObjects, blueprint data
│   ├── StateMachine/   # GameState, combat/planning states
│   └── Utils/          # Hex math, grid helpers
├── Prefabs/
│   ├── Buildings/
│   ├── Enemies/
│   └── UI/
├── Art/
│   ├── Source/         # Unmodified originals from asset packs
│   ├── Tiles/
│   ├── Buildings/
│   ├── Characters/
│   └── UI/
└── Docs/               # Symlink or copy of design vault
```

---

## Key Architectural Decisions

### Hex Grid
Use Unity's **Tilemap with hex layout** (Grid component set to Hexagonal Point Top or Flat Top — decide before starting, do not change mid-prototype). The Tilemap sits on the ground plane (Y = 0) and handles cell coordinates, snapping, and neighbor logic.

3D prefabs (buildings, characters, enemies) are instantiated in world space at positions derived from `Grid.CellToWorld(cellCoord)`, with a Y offset to sit correctly on top of the tile. The Tilemap manages the grid; prefabs handle the visual and gameplay presence.

All gameplay logic uses **cell coordinates** (`Vector3Int`) internally. Use `Grid.WorldToCell` and `Grid.CellToWorld` for conversion. Never store world positions as the source of truth for grid state.

**Adjacency rule:** Two buildings are adjacent if they share a hex edge. Corner-only contact does not count. Implement a `HexUtils.GetNeighbors(Vector3Int cell)` helper returning the 6 edge-sharing neighbors.

### Game State Machine
The game has two top-level states: `COMBAT` and `PLANNING`. These are mutually exclusive. Implement as a simple enum-based state machine on a `GameManager` singleton.

```
COMBAT
  └─ Player moves, enemies move, income ticks, walls take damage
  └─ Time.timeScale = 1

PLANNING
  └─ Everything paused except UI
  └─ Player places/sells buildings
  └─ No enemy movement, no income ticks
  └─ Time.timeScale = 0 (pauses physics and Update loops)
  └─ UI runs on unscaled time (use Time.unscaledDeltaTime in UI code)
```

State transition is triggered by player entering/exiting a **trigger collider** on the base perimeter (`OnTriggerEnter` / `OnTriggerExit` — 3D colliders, not 2D).

### Singletons / Managers
Use a small set of persistent manager singletons. Implement with a simple `Instance` pattern. Do not use DontDestroyOnLoad unless needed for multi-scene setups — keep the prototype single-scene.

| Singleton | Responsibility |
|---|---|
| `GameManager` | Game state machine (COMBAT / PLANNING), run lifecycle |
| `EconomyManager` | Gold, interest, income ticks, player level |
| `BuildingManager` | Grid state, placement, tier upgrade logic, adjacency |
| `WaveManager` | Enemy spawning, wave escalation |

### Buildings as ScriptableObject + Prefab
Each building type is defined as a **ScriptableObject** (`BuildingData`) and has a corresponding prefab (visuals + `Building` MonoBehaviour). This separates balance tuning from code.

`BuildingData` fields:
- `BuildingType` (enum)
- `School` (enum: Arcane, Martial, Nature, Gold)
- `Tier` (int)
- `GoldCost` (int)
- `PassiveEffect` (description + value — applied to player on placement)
- `AdjacencyBonus` (given to neighbors)
- `AdjacencyReceived` (received from neighbors)
- `TierUpgradeResult` (reference to the tier 2 BuildingData)

### Tier Upgrade Logic
Lives entirely in `BuildingManager`. When a third building of the same type is placed:
1. Record the cell coordinate of the third placement
2. Find and destroy the two earlier buildings of that type
3. Instantiate the tier 2 prefab at the third cell position
4. Call `RecalculateAdjacency()` for all affected cells

### Wall System
A single wall ring surrounds the base. Wall tiles use the `tile-walls` asset. Walls are **never repaired**. Wall HP only decreases and does not reset or increase on expansion.

HP is a single shared pool across all wall tiles, tracked in a `WallManager` component and displayed in the HUD. At 0 HP the run ends.

Enemies path to and attack the **nearest wall tile**.

**Tile types and buildability:**
- `tile-castle` — center tile, permanently unbuildable
- `stone-tile` — base tiles, the only buildable area
- `tile-walls` — wall perimeter tiles, unbuildable

**Expansion sequence** (triggered by player level-up in planning mode):
1. Add N new `stone-tile` base tiles clockwise from the top-right neighbor of the center hex, in a fixed deterministic order, where N = `BaseConfig.tilesPerExpansion`.
2. Recalculate the wall perimeter around the full new base shape.
3. Any tile that was `tile-walls` but is now interior to the new perimeter becomes a buildable `stone-tile`.
4. New perimeter tiles become `tile-walls`.

**Tile spawn animation:** every tile placed (base or wall, at run start or on expansion) plays a coroutine scaling it from 0 → 1.3 → 1.

**BaseConfig ScriptableObject fields:**
- `initialTileCount` (int) — stone-tile count at run start, not counting the castle center (default: 3)
- `tilesPerExpansion` (int) — tiles added per level-up
- `maxExpansions` (int) — cap on total expansions per run

---

## What to Avoid

- **No spaghetti GetComponent chains.** Cache component references in `Awake`, not in `Update`.
- **No hardcoded balance values in MonoBehaviours.** All numbers live in `ScriptableObject` data assets so they can be tuned in the Inspector without touching code.
- **No premature abstraction.** Prototype first. Refactor when a second building type needs the same logic, not before.
- **No async scene loading in prototype.** Keep it single-scene. Load everything upfront.
- **No FindObjectOfType in Update.** Use cached references or singleton `Instance` accessors.

---

## Asset Pack Integration

The game uses pre-purchased fantasy hex asset packs containing 3D models. Import assets as-is into `Art/Source/` — do not modify source files. Apply modifications via Unity's import settings or material overrides in URP.

For isometric consistency, ensure all 3D models share the same scale convention and Y-axis ground alignment. Set a project-wide standard (e.g. 1 Unity unit = 1 hex tile width) before importing assets and enforce it on every model.
