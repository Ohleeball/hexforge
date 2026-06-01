# Prototype Scope

## Goal

Build a playable prototype that validates the core loop: fight → push out → capture zone → return to base → build → fight. The prototype does not need to be polished or complete. It needs to be fun enough to identify what works and what doesn't.

---

## What Is In Scope for the Prototype

- Hex grid map with a base area and surrounding zones
- Player character with movement and basic attack
- Enemy spawning and basic pathing toward base
- Wall system: 1 starting ring, wall HP, visual degradation, no repair
- Planning mode: enter base to pause, place buildings, exit to resume
- 2–3 building types (one Martial, one Arcane, one Gold) at tier 1 only
- Tier upgrade mechanic (3 → tier 2) for at least one building type
- Basic adjacency bonus for one building pair
- Economy: enemy gold drops, income tick, interest mechanic, manual leveling
- Hex zone capture: player holds zone, receives blueprint + gold
- Blueprint hand: hold up to 5 blueprints, place from hand

## What Is Out of Scope for the Prototype

- Wall 2 and wall 3 (level 4 and 8 expansions) — include the hook, skip the art
- Tier 3 buildings
- Full building roster (8+ types)
- Full adjacency synergy matrix
- Enemy variety (one enemy type is fine)
- Meta-progression between runs
- Audio
- Polish, juice, particles
- Win condition beyond "survive as long as possible"
- Mobile or gamepad support

---

## Development Phases

### Phase 1 — Foundation
**Goal:** Hex grid, player movement, basic enemy spawning, wall takes damage.

Deliverables:
- [ ] Hex grid renders
- [ ] Player moves on grid
- [ ] Enemies spawn at edges and path to base
- [ ] Wall ring takes damage and shows visual degradation
- [ ] Run ends when wall reaches 0 HP

Relevant docs: [[architecture]], [[core_gameplay_loop]]

---

### Phase 2 — Economy
**Goal:** Gold flows correctly. Income tick works. Interest works. Leveling works.

Deliverables:
- [ ] Enemies drop gold on death
- [ ] Income tick fires every 10 seconds
- [ ] Interest calculated correctly at each tick
- [ ] Player can spend gold to level up
- [ ] Level increases income per tick
- [ ] Gold and level visible in HUD

Relevant docs: [[economy]]

---

### Phase 3 — Base Building
**Goal:** Planning mode works end to end. One building type placeable and functional.

Deliverables:
- [ ] Entering base boundary pauses game
- [ ] Planning mode UI renders building grid and blueprint hand
- [ ] Player can place a blueprint on the grid for gold cost
- [ ] Building applies its passive effect to player on placement
- [ ] Exiting base resumes game
- [ ] Selling a building returns 50% gold and removes blueprint

Relevant docs: [[ui_principles]], [[building_roster]]

---

### Phase 4 — Tier Upgrade + Adjacency
**Goal:** Tier upgrade mechanic works for one building type. One adjacency pair works.

Deliverables:
- [ ] Third placement of same building triggers tier upgrade
- [ ] Two earlier buildings destroyed, tier 2 placed at third location
- [ ] Tier 2 has improved effect
- [ ] One adjacency synergy is active and visible in planning mode

Relevant docs: [[building_roster]]

---

### Phase 5 — Hex Zone Capture
**Goal:** Zones exist on map. Capturing one gives a blueprint.

Deliverables:
- [ ] Neutral zones placed on hex map
- [ ] Player holds zone for X seconds to capture
- [ ] Capture rewards blueprint + gold
- [ ] Blueprint added to player's hand

Relevant docs: [[system_overview]]

---

### Phase 6 — Playtest Loop
**Goal:** Full loop is playable. Identify what feels good and what needs changing.

Deliverables:
- [ ] All phase 1–5 systems connected
- [ ] At least 2 building types available
- [ ] 20-minute run is survivable and interesting
- [ ] Design notes updated based on playtest

---

## Current Session Scope

**Session 1:** Design documentation. No code.

Deliverables:
- [x] readme.md
- [x] high_concept.md
- [x] design_pillars.md
- [x] core_gameplay_loop.md
- [x] system_overview.md
- [x] economy.md
- [x] building_roster.md
- [x] ui_principles.md
- [x] architecture.md
- [x] prototype_scope.md
- [x] cursor_guidelines.md
- [x] current_task.md
- [x] changelog.md

**Next session:** Phase 1 — Foundation. See [[current_task]].
