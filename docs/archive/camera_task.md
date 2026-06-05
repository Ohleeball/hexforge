# Task: Implement Camera System

## Before you start

Read the following docs in order:
1. `docs/cursor_guidelines.md` — project rules, code style, architecture constraints
2. `docs/systems/camera.md` — full design spec for this task (source of truth)

Do not proceed until you have read both.

---

## What to build

Implement `IsometricCameraController.cs` as specified in `docs/systems/camera.md`.

**Output file:** `Assets/Scripts/Systems/IsometricCameraController.cs`

---

## Step-by-step

### Step 1 — Understand the existing codebase

Before writing any code, read the following to understand what already exists:

- `Assets/Scripts/Systems/` — scan all files, understand what managers exist
- `Assets/Scripts/Player/` — find the player GameObject/component to understand how to get the player's world position
- `Assets/InputSystem_Actions.inputactions` — confirm the Input System is already set up
- Find how the castle tile is referenced in the codebase. The castle tile is the permanent center of the base and its world position is needed for planning mode. Look in `BuildingManager` or similar — do not add a new tag or field if a reference already exists.

Report what you find before writing any code.

### Step 2 — Plan

Write out your implementation plan:
- How you will get the player's world position (direct reference, singleton, event?)
- How you will resolve the castle tile world position on `Awake`
- Any assumptions you are making

Wait for confirmation if anything is ambiguous.

### Step 3 — Implement

Create `Assets/Scripts/Systems/IsometricCameraController.cs` strictly following the design doc:

- `CameraRig` parent / Camera child structure (doc section 2 and 8)
- Three states: `NormalFollow`, `Death`, `Planning` (doc section 3 and 7)
- All parameters exposed in Inspector with suggested defaults (doc section 4)
- New Input System only — `Mouse.current.position.ReadValue()`, import from `UnityEngine.InputSystem` (doc section 5)
- Screen-space aim offset projected onto world XZ plane using yaw rotation (doc section 6)
- Public state transition methods: `EnterDeathState()`, `EnterRespawnState()`, `EnterPlanningState()`, `ExitPlanningState()`
- One class per file
- Comments on all functions longer than 10 lines

### Step 4 — Scene setup instructions

After writing the script, provide exact instructions for setting up the scene:
- What GameObjects to create and how to parent them
- What components to attach where
- What Inspector fields need to be wired up (if any)
- How to hook the public methods into the player health and planning mode systems

---

## Constraints

- Do not rotate or change the camera angle at runtime — the isometric rotation is fixed
- Do not use `FindObjectOfType` in `Update` — cache all references in `Awake`
- Do not use legacy `Input.mousePosition` — new Input System only
- Do not add boundary clamping — the camera follows freely
- Do not poll for player death or planning mode state — external systems call the public methods
- Do not add features beyond what is in `docs/systems/camera.md`
- If anything in the existing code contradicts this design doc, flag it with a `# TODO: Design mismatch —` comment and describe the issue. Do not silently change the design.
