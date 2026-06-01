# Assets

## Scale Convention

- 1 metre = 1 Unity unit
- 1 Unity unit = 1 hex tile width
- All imported models must conform to this scale before use. Apply corrections via Unity import settings (scale factor), not by modifying source files.

## Hex Orientation

Flat Top. This must match the TileMap Grid component setting in Unity. Do not use Point Top assets or rotate tiles to compensate.

## Asset Packs

All packs are from [Kenney](https://kenney.nl). Kenney assets are public domain (CC0) — free to use in commercial projects, no attribution required. Verify at https://kenney.nl/assets if needed.

| Pack | Usage |
|---|---|
| kenney_hexagon-kit | Hex tiles, terrain |
| kenney_castle-kit | Walls, base structures, enemies (catapults, battering rams) |
| kenney_survival-kit | Decoration, props |
| kenney_nature-kit | Environmental decoration |
| kenney_mini-arena | Enemy character (soldier) |
| kenney_graveyard-kit_5.0 | Enemy-related props, atmosphere |
| kenney_fantasy-town-kit_2.0 | Buildings, town structures |

## Import Conventions

- Original unmodified files live in `Art/Source/`. Do not edit anything under `Art/Source/`.
- Apply scale corrections, material swaps, and URP shader assignments via Unity import settings or material overrides — never by modifying source files.
- Organised subdirectories under `Art/Source/` mirror pack names (e.g. `Art/Source/kenney_hexagon-kit/`).
- Processed or modified assets (e.g. prefabs, material overrides) live in the appropriate subfolder under `Art/` (`Tiles/`, `Buildings/`, `Characters/`, etc.).
