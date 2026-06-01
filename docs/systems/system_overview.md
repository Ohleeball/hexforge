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
Concentric walls define buildable area and act as base HP buffer. Wall 1 at start. Wall 2 unlocked at level 4. Wall 3 unlocked at level 8. Walls never repair. Enemies breach outer walls and attack buildings and inner walls. Each wall ring is a separate HP pool.

---

### Player Leveling System
**Status:** `specced`  
**Doc:** [[economy]]  
Player spends gold to level up. Higher level increases passive income and upgrade rarity rolls. Level 4 and 8 trigger wall expansion. Incentivizes holding out at lower power for long-term advantage.

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
