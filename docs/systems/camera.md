# Camera System Design Document
**System:** Camera Controller  
**Status:** Ready for implementation  
**Last updated:** 2026-06-05

---

## 1. Overview

The camera is a **true 3D isometric camera** in Unity — a physically angled 3D camera using orthographic projection. It follows the player character while also shifting toward the mouse cursor position to give the player more battlefield awareness in their aim direction. The system has three distinct states: normal follow, death/respawn, and planning/building mode.

---

## 2. Camera Setup (Unity)

- **Projection:** Orthographic
- **Rotation:** Fixed at classic isometric angles — `(30, 45, 0)` in euler degrees (X=30 gives the isometric elevation; Y=45 gives the diagonal orientation). These are locked and never change at runtime.
- **Parent object:** `CameraRig` — an empty GameObject that moves in world space. The Camera is a child of this rig, offset back along its local Z axis.
- **The rig's position** is what gets driven by the camera logic. The camera itself never moves independently.

---

## 3. States

### 3.1 Normal Follow (default)

The active state during gameplay. Two simultaneous behaviours compose the final camera rig position:

**A) Player follow**  
The rig snaps tightly to the player's world position. "Snappy" means a very high lerp speed (see parameters) — it should feel nearly instant, with just enough smoothing to prevent single-frame jitter.

**B) Aim offset**  
The rig is additionally displaced toward the mouse cursor. This gives the player more visual space in the direction they're aiming.

- The offset is calculated in **screen space**: measure the mouse position relative to the **screen center**, normalize it by half the screen dimensions to get a `[-1, 1]` range on each axis, then map that to a world-space offset applied to the rig.
- The screen-space direction is converted to a world-space direction by accounting for the camera's isometric angle — the offset must be projected onto the world XZ plane so it doesn't push the camera up or down.
- The offset magnitude scales linearly with mouse distance from screen center. At the screen edge (`magnitude = 1.0` in normalized space) the offset reaches `AimOffsetMaxDistance` (world units). At screen center the offset is zero.
- The aim offset is also smoothed (separate lerp speed from player follow — see parameters), so flicking the mouse across the screen doesn't snap the camera instantly but still feels responsive.

**Final rig target position each frame:**
```
targetPosition = playerWorldPosition + aimOffsetWorldSpace
rigPosition = Lerp(rigPosition, targetPosition, followSpeed * deltaTime)
```

> **Note:** No level boundary clamping. The camera follows freely even if it would show out-of-bounds areas. This is an intentional design choice.

> **Input System:** This component uses Unity's **new Input System**. Ensure the project has `com.unity.inputsystem` installed and `Mouse` is imported from `UnityEngine.InputSystem`.

---

### 3.2 Death / Respawn State

Triggered when the player dies. Transitions back to Normal Follow when respawn is complete.

- The camera **zooms out** — orthographic size increases to `DeathZoomSize`.
- The rig **stops following** the player's position; it stays at the position of death.
- Aim offset is **disabled** during this state.
- Both the zoom-out and zoom-in (on respawn) are **smoothly animated** using lerp (see parameters).
- On respawn: camera zooms back to `NormalZoomSize` and re-locks to the player before re-enabling aim offset.

**Transition sequence on death:**
1. State → Death
2. Lerp ortho size from `NormalZoomSize` → `DeathZoomSize`
3. Freeze rig position

**Transition sequence on respawn:**
1. Snap rig to new player spawn position (instant, happens while camera is still zoomed out)
2. Lerp ortho size from `DeathZoomSize` → `NormalZoomSize`
3. State → Normal Follow (re-enables aim offset)

---

### 3.3 Planning / Building Mode

Triggered when the player enters the base's planning or building interface. Transitions back to Normal Follow on exit.

- The camera re-centers on the **castle tile** (the permanent center of the base). Its world position is resolved once on `Awake` — see `BaseCenterPoint` in parameters.
- The camera **zooms out** to `PlanningZoomSize` to show the full base.
- Aim offset is **disabled**.
- Player follow is **disabled**.
- All transitions (position shift + zoom) are smoothly lerped.
- The rig does not move while in this state (no follow, no drift).

**Transition sequence into planning mode:**
1. State → Planning
2. Simultaneously lerp rig position → `BaseCenterPoint` and ortho size → `PlanningZoomSize`

**Transition sequence out of planning mode:**
1. Snap rig to player position (instant)
2. Simultaneously lerp ortho size → `NormalZoomSize`
3. State → Normal Follow

---

## 4. Parameters (all exposed in Inspector)

| Parameter | Type | Suggested Default | Description |
|---|---|---|---|
| `NormalZoomSize` | float | `8.0` | Orthographic size during normal gameplay |
| `DeathZoomSize` | float | `14.0` | Orthographic size during death state |
| `PlanningZoomSize` | float | `18.0` | Orthographic size during planning/building mode |
| `AimOffsetMaxDistance` | float | `4.0` | Max world-unit offset when mouse is at screen edge |
| `PlayerFollowSpeed` | float | `25.0` | Lerp speed for rig tracking player (high = snappy) |
| `AimOffsetSpeed` | float | `12.0` | Lerp speed for aim offset smoothing |
| `ZoomSpeed` | float | `6.0` | Lerp speed for orthographic size transitions |
| `PlanningTransitionSpeed` | float | `5.0` | Lerp speed for rig position shift into planning mode |
| `BaseCenterPoint` | Vector3 | (derived at runtime) | World position of the castle tile — resolved once on `Awake` via `BuildingManager` or a tagged GameObject, not set manually |
| `CameraElevationAngle` | float | `30.0` | X rotation of the camera rig (isometric elevation) |
| `CameraYawAngle` | float | `45.0` | Y rotation of the camera rig (isometric diagonal) |

---

## 5. Input Handling

- Mouse position is read each frame via `Mouse.current.position.ReadValue()` (Unity new Input System).
- Mouse position is in screen pixels. Convert to normalized `[-1, 1]` range:
  ```
  normalizedMouse.x = (mousePos.x - Screen.width  * 0.5f) / (Screen.width  * 0.5f)
  normalizedMouse.y = (mousePos.y - Screen.height * 0.5f) / (Screen.height * 0.5f)
  ```
- Clamp the normalized value's magnitude to `1.0` before scaling, to handle multi-monitor edge cases.

---

## 6. Coordinate Mapping (Screen → World Offset)

Because the camera is isometric (angled), a screen-space mouse direction cannot be applied directly as a world XZ offset — it needs to be projected so the screen axes line up with what the player sees: mouse-right shifts the view right, mouse-up shifts the view "into" the screen.

**Approach:** map the normalized screen vector onto the rig's own basis vectors, flattened to the XZ plane. Screen-right maps to the rig's `right`, screen-up maps to the rig's `forward`:

```csharp
// rig == the CameraRig transform (rotation fixed at CameraElevationAngle, CameraYawAngle, 0)
Vector3 right   = rig.right;   right.y   = 0f; right.Normalize();
Vector3 forward = rig.forward; forward.y = 0f; forward.Normalize();

Vector3 worldOffset = right * nx + forward * ny;          // nx, ny = normalized mouse axes
worldOffset = worldOffset.normalized * (normalizedMagnitude * AimOffsetMaxDistance);
```

Flattening `right`/`forward` to `y = 0` keeps the offset purely horizontal regardless of camera elevation.

> **Why not a hand-rolled `cos/sin` yaw rotation?** An earlier version of this doc rotated the screen vector with an explicit `cos/sin` matrix built from `CameraYawAngle`. For this rig's orientation that mapped the axes with swapped/inverted signs (mouse left↔right shifted the view up↔down and vice-versa). Deriving the offset from the rig's actual `right`/`forward` vectors is orientation-correct regardless of yaw sign conventions or hex-grid swizzle handedness, so it is the canonical approach.

---

## 7. State Machine

```
         [Game Start]
               │
               ▼
        ┌─────────────┐
        │ NormalFollow │◄──────────────────┐
        └─────────────┘                    │
          │         │                      │
      [death]   [enter build]         [respawn /
          │         │                  exit build]
          ▼         ▼                      │
      ┌───────┐  ┌──────────┐             │
      │ Death │  │ Planning │─────────────┘
      └───────┘  └──────────┘
```

States are mutually exclusive. Transitions between Death and Planning are not expected during normal gameplay and do not need to be handled.

---

## 8. Component Structure

**Recommended Unity component layout:**

```
CameraRig (GameObject)
  └── IsometricCameraController.cs   ← all logic lives here
  └── Main Camera (child GameObject)
        └── Camera component
        └── AudioListener
```

`IsometricCameraController.cs` responsibilities:
- Holds all exposed parameters (Section 4)
- Owns the state machine (Section 7)
- Reads mouse input each frame (Section 5)
- Computes aim offset (Section 6)
- Drives `CameraRig` transform position and `Camera.orthographicSize`
- Exposes public methods for state transitions:
  - `EnterDeathState()`
  - `EnterRespawnState()`
  - `EnterPlanningState()`
  - `ExitPlanningState()`

External systems (player health, UI manager) call these methods — the camera controller does not poll for player state itself.

---

## 9. Open Questions / Future Considerations

- **Screenshake:** Not in scope for this pass, but the `CameraRig` / Camera child separation makes it easy to add later — screenshake can be applied as a local position offset on the Camera child without disturbing the rig's world-space logic.
- **Multiplayer / split screen:** Not considered. Single player only.
- **Zoom on ability use:** Could be a fourth state or a transient additive effect — defer until abilities are designed.
- **Controller support:** Aim offset using right thumbstick would use the same normalized `[-1,1]` input range — a second input path can be wired into the same offset logic with minimal changes.

---

## 11. Decisions

### CameraRig parent / Camera child separation — 2026-06-04
**Choice:** All camera logic drives a `CameraRig` empty GameObject. The Camera is a child of the rig and never moves independently.
**Why:** Keeps world-space positional logic (follow, aim offset, state transitions) cleanly separated from camera-local effects. Screenshake, FOV pulses, or any transient visual effect can be applied as local offsets on the Camera child without corrupting the rig's position state. Mixing both into a single transform makes those effects much harder to isolate and undo cleanly.
**Consequences:** Any system that needs to know "where the camera is looking from" should reference the rig position, not the Camera child transform.

### No level boundary clamping — 2026-06-04
**Choice:** The camera follows freely and does not clamp to level bounds.
**Why:** At prototype stage the map size and camera zoom levels are not finalised. Clamping requires knowing the playable boundary, which would create a coupling between the camera system and map generation before either is stable. Easier to add clamping later than to remove incorrect clamping logic.
**Consequences:** Players can briefly see out-of-bounds areas. Acceptable for prototype; revisit before shipping.

### Aim offset projected via yaw-only rotation — 2026-06-04
**Choice:** Screen-space mouse direction is rotated into world XZ using only the camera's Y (yaw) angle, not the full isometric rotation.
**Why:** The goal is to shift the camera horizontally toward where the player is aiming. Applying the full 3D rotation would introduce a vertical (Y) component to the offset, pushing the camera up or down. Yaw-only keeps the offset purely horizontal and predictable regardless of elevation angle.
**Consequences:** If the elevation angle changes significantly in future, this logic remains correct. Only a yaw change would require updating the offset calculation.

---

## 10. Acceptance Criteria

- [ ] Camera maintains isometric angle at all times; rotation is never modified at runtime
- [ ] Camera follows player with effectively zero perceptible lag during normal movement
- [ ] Aim offset visibly shifts the view toward the mouse; the shift is proportional to mouse distance from screen center
- [ ] `AimOffsetMaxDistance` tunable in Inspector; changes take effect in Play mode
- [ ] Death state triggers zoom-out; camera freezes at death position; zoom reverses cleanly on respawn
- [ ] Planning state centers on `BaseCenterPoint` with zoom-out; aim offset and follow disabled
- [ ] All state transitions are smooth (no position or size pops)
- [ ] No level boundary clamping — camera follows freely
