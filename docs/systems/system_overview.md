# System Overview

Status stages: `concept` → `specced` → `implemented`

---

## Systems

### Economy System
**Status:** `specced`  
**Doc:** [[economy]]  
Gold drops from enemies, passive income ticks, interest mechanic (TFT-style), player leveling costs gold and increases income. Gold-generating buildings available as a high-risk investment.

---

### Base Building System
**Status:** `specced`  
**Doc:** [[building_roster]]  
Player enters base to pause game and enter planning mode. Buildings are placed on a hex grid within walled area. Buildings cost gold and require a blueprint. Selling returns 50% gold but blueprint is permanently lost.

---

### Tier Upgrade System
**Status:** `specced`  
**Doc:** [[building_roster]]  
Placing 3 of the same building merges them into a tier 2. The two earlier buildings are destroyed. The third placement location becomes the tier 2 building. Tier 2 can merge into tier 3 the same way.

---

### Adjacency Synergy System
**Status:** `concept`  
**Doc:** [[building_roster]]  
Buildings of the same school placed adjacent to each other gain synergy bonuses. Different building types can also have cross-synergies. Tier 2 buildings inherit and improve the adjacency bonuses of the two they consumed.

---

### Wall System
**Status:** `specced`  
A single wall ring surrounds the base and acts as the base HP buffer. The wall is represented by `tile-walls` tiles placed on every hex that borders the base perimeter.

Wall HP is a single shared pool across all wall tiles. It is displayed in the HUD, never repairs, and does not reset or increase when the base expands. At 0 HP the run ends.

Enemies path toward and attack the nearest wall tile.

**Expansion:** When the player levels up (via gold purchase in planning mode), the base expands:
1. New base tiles (`stone-tile`) are added clockwise starting from the top-right neighbor of the center hex, following the same fixed order every time, quantity defined in `BaseConfig`.
2. The wall perimeter is recalculated. Any tile that was a wall tile but is now interior to the new perimeter becomes a buildable stone base tile.
3. New perimeter tiles become wall tiles.

All newly placed tiles (base or wall) play a spawn animation: scale 0 → 1.3 → 1.

Expansion cap is configurable in `BaseConfig`. Wall tiles are unbuildable. The castle center tile (`tile-castle`) is permanently unbuildable.

---

### Player Leveling System
**Status:** `specced`  
**Doc:** [[economy]]  
Player spends gold to level up. Higher level increases passive income and upgrade rarity rolls. Each level up triggers a base wall expansion. Leveling is done during planning mode. Incentivizes holding out at lower power for long-term advantage.

---

### Hex Zone Capture System
**Status:** `specced`  
Neutral hex zones exist outside the base on the map. Player physically holds a zone briefly to capture it. Reward is a building blueprint plus gold. Zones are the only source of blueprints. Distance from base correlates with reward quality and risk.

---

### Planning Mode / Pause System
**Status:** `specced`  
**Doc:** [[ui_principles]]  
Entering the base boundary pauses the game. Player can place buildings, view adjacency, and sell buildings. Leaving the base unpauses. No player movement during planning mode.

---

### Enemy Spawning System
**Status:** `concept`  
Enemies spawn continuously from map edges. Waves escalate over time. Enemy kills drop gold. Enemy pathing targets base walls. Details TBD.

---

### Blueprint System
**Status:** `specced`  
Blueprints are obtained from captured hex zones. Each blueprint is for a specific building type. Selling a building destroys its blueprint — it must be found again in the world. Player holds a hand of available blueprints to place.

---

## Future Systems (Post-Prototype)

- Player movement inside base during planning mode (currently excluded to preserve planning focus)
- Boss enemies
- Meta-progression between runs
- Multiplayer
