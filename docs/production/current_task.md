# Current Task

> This file should be updated at the start of every session. It is the single source of truth for what is being built right now.

---

## Current Phase

**Phase 1 — Foundation**

## Objective

Get a hex grid on screen with a player that can move, enemies that spawn and path to the base, and a wall that takes damage and ends the run when destroyed.

## Tasks

- [ ] Create hex grid scene using Godot TileMap in hex mode
- [ ] Place a base area in the center of the grid (marked tiles)
- [ ] Add a wall ring around the base area (Wall 1)
- [ ] Implement player character: 8-directional movement, stays on grid
- [ ] Implement basic enemy: spawns at map edge, paths toward base wall
- [ ] Implement wall HP: enemies deal damage on contact with wall
- [ ] Implement visual degradation on wall (3 stages: healthy, damaged, critical)
- [ ] Implement run-end condition: wall HP reaches 0 → show game over screen
- [ ] Enemy drops gold on death (gold stored in Economy singleton, not spent yet)

## Out of Scope This Phase

- Planning mode
- Building placement
- Income ticks
- Hex zone capture
- Player attacking (can stub with a simple click-to-kill for testing)

## Definition of Done

A player can move around a hex grid, enemies spawn and walk toward the base, the wall takes damage over time as enemies reach it, and the game ends when the wall is destroyed.

## Relevant Docs

- [[architecture]] — project structure, game state machine, hex grid decisions
- [[core_gameplay_loop]] — what the full loop looks like
- [[system_overview]] — wall system, enemy spawning

---

## Next Phase

**Phase 2 — Economy**
See [[prototype_scope]] for details.
