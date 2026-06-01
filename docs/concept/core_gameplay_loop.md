# Core Gameplay Loop

## Run Structure

A run begins with a small walled base, no buildings, and enemies spawning from the edges of the map. The run ends when the innermost wall is destroyed.

---

## The Moment-to-Moment Loop

```
Fight enemies near base
    → enemies drop gold
    → income ticks every 5-10 seconds passively
    
Push out to neutral hex zones
    → hold zone briefly to capture it
    → receive building blueprint + gold reward
    → risk: base takes wall damage while you're away
    
Re-enter base
    → game pauses (planning mode)
    → spend gold to place blueprints
    → consider adjacency, tier upgrade paths, synergies
    → optionally sell a building for 50% gold (blueprint lost)
    → leave base to resume

Decide gold allocation each income tick
    → spend on buildings
    → save for interest (every 10 gold = 1 bonus gold per tick)
    → invest in player level (costs gold, increases passive income + upgrade rarity)
```

---

## The Run Progression Arc

### Act 1 — Inner Wall (Levels 1–3)
- Small buildable area inside wall 1
- Limited blueprint slots, tight economy
- Player stays close to base, learns systems
- Wall 1 is the only buffer against destruction

### Act 2 — Middle Ring (Level 4)
- Wall 2 added at level 4, expanding buildable area
- Wall 1 becomes inner ring — enemies must breach wall 2 first
- More building slots, more complex adjacency decisions
- Player can push further out safely

### Act 3 — Outer Ring (Level 8)
- Wall 3 added at level 8
- Three concentric rings of wall
- Full base complexity — tier 2 and tier 3 buildings possible
- Late-game economic and synergy decisions dominate

---

## Key Tensions

| Tension | Choice A | Choice B |
|---|---|---|
| Economy | Spend gold on buildings now | Save for interest + level |
| Risk | Push far for better blueprints | Stay close to defend |
| Space | Cluster same buildings to merge | Spread different buildings for cross-synergy |
| Timing | Re-enter base to build mid-wave | Finish wave first |
| Selling | Free up a bad slot | Lose blueprint permanently |

---

## Win / Loss

**Loss:** Innermost wall is destroyed. All walls are permanent — they never repair.

**Win condition:** TBD for prototype. Survive N waves, or reach level 8, or defeat a boss enemy.
