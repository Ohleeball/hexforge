# Cursor Guidelines

These rules apply to all AI-assisted development on this project. Read before writing any code.

---

## Always Read First

Before writing any code for a system, read the relevant design doc:
- Economy work → read `economy.md`
- Building work → read `building_roster.md`
- UI work → read `ui_principles.md`
- Architecture decisions → read `architecture.md`
- What to build right now → read `current_task.md`

Do not infer design intent from the code. The docs are the source of truth.

---

## Engine and Language

- **Engine:** Unity 3D (LTS, URP)
- **Language:** C#
- **Camera:** Fixed isometric, orthographic, do not rotate or change angle during play
- **Hex grid:** Use Unity's Tilemap with hex layout for grid management and coordinates. 3D prefabs are placed in world space derived from cell coordinates. Use `Vector3Int` for all grid logic. Never use world position as the source of truth for grid state.

---

## Code Style

- Use descriptive variable names. No single-letter variables except loop indices.
- All balance values (costs, damage, income amounts, tick intervals) must be defined as `@export` variables or in a data resource. Never hardcode them inline.
- Comments are required on any function longer than 10 lines.
- One class per file.

---

## Architecture Rules

- The economy system is a singleton. Access gold via `EconomyManager.Instance.Gold`, not via scene references.
- Game state (COMBAT / PLANNING) is managed by `GameManager.Instance`. No system should independently decide what state the game is in.
- Buildings are defined as ScriptableObjects (`BuildingData`). The prefab handles visuals. The ScriptableObject handles data.
- `BuildingManager` owns the grid state. No other system modifies the building grid directly.

---

## What Not to Do

- Do not add features not listed in `current_task.md` without confirmation.
- Do not repair walls in code. Wall HP only decreases. There is no repair method.
- Do not add tower defense mechanics (no auto-attacking buildings).
- Do not add player movement code to planning mode.
- Do not use `OnTriggerEnter2D` or any 2D physics components. This is a 3D project — use `OnTriggerEnter`, `Collider`, `Rigidbody`, etc.
- Do not use `FindObjectOfType` in `Update`. Cache references in `Awake` instead.

---

## Testing

- After implementing each phase, manually test the full loop described in `core_gameplay_loop.md`.
- If a system behaves differently from its design doc, flag it with a `# TODO: Design mismatch —` comment and describe the discrepancy. Do not silently change the design.

---

## Commit Messages

Format: `[phase] short description`

Examples:
- `[phase1] hex grid renders with player movement`
- `[phase2] income tick and interest calculation working`
- `[phase3] planning mode pause and building placement`
