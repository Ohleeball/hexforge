# UI Principles

## Core Philosophy

The UI should communicate game state at a glance during combat, and provide clear planning affordances when paused. No tooltips should be required to make a decision mid-fight.

---

## Two UI Modes

### Combat Mode (game running)
Minimal HUD. Player needs to see:
- Wall health (all rings, visual representation — cracks, color degradation)
- Current gold
- Income tick countdown
- Player level
- Incoming enemy indicators

Everything else is out of the way.

### Planning Mode (game paused, player inside base)
Full building interface. Player sees:
- Current base grid with placed buildings highlighted
- Available blueprints in hand (bottom bar)
- Gold cost per blueprint
- Adjacency indicators when hovering a placement
- Potential tier upgrade indicator if 2 of the same type already exist
- Sell option per building (50% return shown)
- No player movement — camera is fixed on base

---

## Planning Mode Entry / Exit

- **Enter:** Player character crosses the base boundary → game immediately pauses → planning mode opens
- **Exit:** Player confirms they are done (button or hotkey) → planning mode closes → game unpauses → player resumes at base boundary

There is no penalty for entering planning mode mid-wave. This is an intentional design choice — the pause is the reward for returning to base under pressure.

---

## Adjacency Feedback

When a player hovers a blueprint over a placement slot in planning mode:
- Highlight which adjacent buildings will form synergies (green glow)
- Highlight which adjacent buildings would be destroyed on tier upgrade (orange warning)
- Show a ghost preview of what the tier 2 would look like if the third of a type is placed here

---

## Wall Health Visualization

Walls should communicate health without a number bar:
- Full health: solid stone texture
- 50–75% health: minor cracks
- 25–50% health: heavy cracks, discoloration
- Under 25%: crumbling, visual alarm indicator
- Destroyed segment: gap in wall, enemies can enter

Inner walls use the same system. Multiple rings should be visually distinct (different materials or ages).

---

## Income Tick Countdown

A small visible countdown to the next income tick. Players use this to time base re-entry (do I go in now and plan before the tick, or wait for the tick first?).

---

## Blueprint Hand

The player holds a hand of blueprints (max size TBD — prototype: 5). Blueprints are shown as cards or icons at the bottom of the screen in planning mode. Each shows:
- Building name and school icon
- Gold cost
- Tier (all found blueprints start at tier 1)

---

## What We Are NOT Building in UI (Prototype)

- Damage numbers
- Detailed enemy HP bars
- Minimap
- Tutorial overlays
- Tooltip descriptions in combat mode
