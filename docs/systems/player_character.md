# Player Character System

**Status:** `specced`

---

## Purpose

The player is a physically present character on the hex map during COMBAT mode. The player moves freely across walkable tiles, aims with the mouse, and is the primary agent for engaging enemies, capturing hex zones, and entering the base to trigger planning mode.

The player has no auto-attack in Phase 1. Combat interaction is stubbed (click-to-kill for testing). Full attack implementation is Phase 2+.

---

## Components (on `character-keeper.prefab`)

| Component | Notes |
|---|---|
| `PlayerController` | MonoBehaviour — movement, facing, animation |
| `PlayerData` | ScriptableObject — all tunable values (move speed etc.) |
| `Rigidbody` | `isKinematic = false`, gravity disabled, rotation frozen on all axes |
| `CapsuleCollider` | Sized to character bounds — enables collision with enemies and walls |
| `Animator` | Drives `idle` and `walk` clips from `character-keeper.fbx` |

---

## Movement

- **Input:** WASD (or arrow keys). Produces a raw direction vector each frame, then rotated into **camera space** so the input axes line up with what the player sees: W moves "into" the screen, A moves screen-left, etc.
  - The camera is a fixed isometric rig yawed 45° (see `docs/systems/camera.md`). Applying raw world-space WASD under that yaw makes the character walk diagonally relative to the screen (pressing left moves toward the bottom-left), which feels broken. Mapping the input through the camera's flattened `right`/`forward` basis (`y = 0`, normalized) keeps movement screen-relative. If `mainCamera` is unavailable, fall back to the raw world-space vector.
- **Style:** Free movement — not snapped to hex cell centers. The player slides smoothly across the tile plane. Movement stays on the XZ plane (the camera basis is flattened to `y = 0`).
- **Physics:** Driven by `Rigidbody.MovePosition` in `FixedUpdate`. This keeps movement in sync with the physics step and allows the collider to interact correctly with enemies and wall geometry.
- **Speed:** Defined in `PlayerData.moveSpeed`. Never hardcoded in `PlayerController`.
- **Active only in COMBAT mode.** `PlayerController` checks `GameManager.Instance.State` each `FixedUpdate` and skips movement input during PLANNING.

---

## Walkability Constraint

Movement is restricted to tiles where `HexTile.isWalkable == true`. Unwalkable tile types include `tile-walls`, `tile-castle`, water, and any future tile type with `isWalkable = false`.

**Constraint method: axis-separated sliding.**

Each `FixedUpdate`, the intended movement vector is split into its X and Z components and tested independently:

1. Compute `candidatePosition = currentPosition + (inputDirection * speed * fixedDeltaTime)`
2. Convert `candidatePosition` to cell coordinates via `Grid.WorldToCell`
3. Fetch tile: `tilemap.GetTile<HexTile>(cellCoord)`
4. If `tile == null || !tile.isWalkable` → the full move is blocked. Then:
   - Test X component only: apply `(inputDirection.x, 0, 0) * speed * fixedDeltaTime`
   - If that cell is walkable, apply the X-only move (slide along Z wall)
   - Otherwise test Z component only and apply if walkable (slide along X wall)
   - If both axes are individually blocked, do not move

This allows the player to slide along wall boundaries naturally without stopping dead.

---

## Facing Direction

- The character always rotates to face the mouse cursor, **independent of movement direction**.
- Method: cast a ray from the camera through the mouse screen position onto the Y=0 world plane (`Physics.Raycast` against a `LayerMask` for the ground, or a manual plane intersection). The hit point gives the world position the cursor is over.
- Compute direction from player position to hit point (X/Z only, ignore Y). Apply as `transform.rotation` directly — no lerp, instant snap.
- If the ray misses (cursor off the map), retain the last valid facing direction.

---

## Animation

**Animator controller:** `PlayerAnimator` (created in Unity Editor, lives in `Assets/Animation/`).

**Clips used** (from `character-keeper.fbx`):

| Clip name | Used for |
|---|---|
| `idle` | Standing still |
| `walk` | Moving in any direction |

**Parameter:** single `bool` named `IsMoving`.
- `IsMoving = true` when input direction magnitude > 0
- `IsMoving = false` otherwise

**Transitions:**
- `idle` → `walk`: `IsMoving == true`, no exit time, transition duration 0.1s
- `walk` → `idle`: `IsMoving == false`, no exit time, transition duration 0.1s

Other available clips (`sprint`, `attack-melee-left`, `attack-melee-right`, `die`, etc.) are present in the FBX and available for future phases.

---

## Script Structure

### `PlayerData.cs` (ScriptableObject)

```
Fields:
  float moveSpeed         // default: 4.0
```

Created as asset at `Assets/Data/Player/PlayerData.asset`.

### `PlayerController.cs` (MonoBehaviour)

```
Serialized fields:
  PlayerData data
  Tilemap tilemap         // reference to the hex Tilemap in the scene
  Grid grid               // reference to the Grid component

Cached in Awake:
  Rigidbody rb
  Animator animator
  Camera mainCamera

FixedUpdate:
  - Skip if GameManager.Instance.State == PLANNING
  - Read WASD input, rotate into camera space (flattened right/forward basis)
  - Run axis-separated walkability check
  - Call rb.MovePosition
  - Set animator IsMoving

Update:
  - Mouse-to-world raycast
  - Rotate transform to face hit point
```

---

## Decisions Log

### Rigidbody over CharacterController
**Decision:** Use `Rigidbody` with `MovePosition`, not `CharacterController` or Transform.
**Why:** Enemies will need to physically collide with the player and with walls. A Rigidbody participates in the physics simulation correctly. `CharacterController` uses its own collision resolution that doesn't interact well with Rigidbody-based enemies. Transform-based movement bypasses physics entirely and would require manual collision handling later.

### Axis-separated sliding over hard block
**Decision:** When the target cell is unwalkable, test X and Z movement components separately and apply whichever is walkable.
**Why:** Hard blocking feels bad — the player stops dead against wall edges. Sliding is the standard expectation for movement in any action game. The hex grid has irregular diagonal edges that would make hard blocking especially punishing. Axis separation is the simplest correct implementation of sliding that doesn't require a full physics solver.

### Always-face-mouse independent of movement
**Decision:** Character rotation is driven entirely by mouse cursor position, not by movement direction.
**Why:** The player will need to aim attacks at enemies that may not be in the direction of movement. Decoupling facing from movement allows strafing, which is essential for skill-based combat even in Phase 1 stubbing. Standard for twin-stick style games.

### Walkability queried per-frame from Tilemap
**Decision:** Walkability is checked by fetching `HexTile` from the Tilemap each `FixedUpdate`, not cached on the player.
**Why:** The tile grid can change during a run (base expansion changes wall positions). A cached walkability map would go stale. Per-frame lookup via `tilemap.GetTile<HexTile>` is cheap and always correct. If profiling shows it is a bottleneck it can be cached with a dirty flag on the grid.

### No movement in planning mode
**Decision:** `PlayerController` checks `GameManager.Instance.State` and skips all movement when `PLANNING`.
**Why:** Consistent with the architecture decision documented in the changelog. The base boundary trigger handles state transition; `PlayerController` just respects it. No special disabling of the component needed — the check is a single guard at the top of `FixedUpdate`.

### idle and walk only for Phase 1
**Decision:** Only wire `idle` and `walk` clips in the Animator for now. Do not configure attack, die, or other clips.
**Why:** Phase 1 scope is movement only. The FBX contains the full clip set (`attack-melee-left`, `attack-melee-right`, `die`, `sprint`, etc.) and these will be wired in their relevant phase. Setting them up now would add Animator complexity that would likely need reworking anyway.

### Camera-relative movement over raw world-space input
**Decision:** Rotate the raw WASD vector into camera space (the camera's `right`/`forward` flattened to the XZ plane) before applying it, rather than feeding raw world-space input to `Rigidbody.MovePosition`.
**Why:** The camera is a fixed isometric rig yawed 45° (`docs/systems/camera.md`). Raw world-space WASD under that yaw walks diagonally relative to the screen — pressing left moves the character toward the bottom-left — which reads as a control bug. Deriving movement from the camera basis makes the controls screen-relative and intuitive, and automatically stays correct if the camera's yaw is ever retuned. (Earlier revisions of this doc specified "raw direction vector in world space"; this decision supersedes that wording.)
