# Changelog

Design decisions log. Most recent first. Include the *why*, not just the *what*.

---

## Session 2 — Player Character Spec

### Rigidbody-based movement
**Decision:** Player uses `Rigidbody` with `MovePosition`, not `CharacterController` or Transform.
**Why:** Enemies and walls need real physics collision. `CharacterController` doesn't play well with Rigidbody-based enemies. Transform bypasses physics entirely and would require manual collision handling later.

### Axis-separated sliding on unwalkable tiles
**Decision:** When the target cell is unwalkable, X and Z movement components are tested independently. Whichever axis leads to a walkable cell is applied, allowing the player to slide along boundaries.
**Why:** Hard blocking stops the player dead at wall edges, which feels bad and is especially punishing on a hex grid with irregular diagonal boundaries. Axis-separation is the minimal correct implementation of sliding without a full physics solver.

### Always-face-mouse, independent of movement direction
**Decision:** Character rotation is determined solely by mouse cursor world position, not by the movement vector.
**Why:** Attacking enemies requires aiming independently of movement direction. Decoupling the two enables strafing, which is fundamental to the skill-based combat feel the design calls for.

### Walkability queried per-frame from Tilemap
**Decision:** `tilemap.GetTile<HexTile>(cellCoord)` is called each `FixedUpdate` rather than caching a walkability map on the player.
**Why:** The tile grid mutates during a run (base expansions change wall positions). A cached map would go stale. The per-frame lookup is cheap and always correct.

### idle and walk animations only for Phase 1
**Decision:** Only the `idle` and `walk` clips from `character-keeper.fbx` are wired in the Animator for Phase 1. The full clip set (`attack-melee-left`, `attack-melee-right`, `die`, `sprint`, etc.) is deferred to the phase that needs them.
**Why:** Phase 1 scope is movement only. Wiring unused clips now would create Animator complexity that would likely need reworking when the full combat system is designed.

---

## Session 1 — Design Documentation

### No wall repair
**Decision:** Walls never repair. Damage is permanent.
**Why:** Makes every moment of overextension meaningful. Consistent with the design pillar of permanent consequences. Borrowed from TFT where player HP rarely recovers. Keeps the game tense throughout a run rather than allowing recovery loops.

### Selling buildings loses the blueprint
**Decision:** Selling a building returns 50% gold but the blueprint is permanently lost and must be found again.
**Why:** Selling should be a costly decision, not a free undo. If blueprints were retained on sale, players would optimise by selling and rebuilding freely, undermining the spatial planning challenge. The 50% gold return ensures selling is never completely wasteful.

### No movement during planning mode
**Decision:** Player cannot move while in planning mode. Camera fixed on base.
**Why:** Planning mode is intended to feel like an auto-battler draft phase — deliberate, without action pressure. Allowing movement would blur the distinction between the two modes and create awkward control conflicts. May revisit post-prototype if playtest feedback suggests it.

### Entering base boundary triggers planning mode
**Decision:** Planning mode is triggered automatically when the player crosses the base boundary, not via a button press.
**Why:** More intuitive and spatially grounded. The base boundary is a meaningful physical threshold. It also creates interesting emergent decisions — do I cross the boundary (and pause) mid-wave to plan, or finish the wave first?

### Third building placement determines tier 2 location
**Decision:** When 3 of the same building are placed, the first two are destroyed and the third placement location becomes the tier 2 building.
**Why:** Creates a spatial planning puzzle. Experienced players deliberately place the first two in less valuable spots and save prime real estate for the tier 2. Rewards forward thinking.

### Concentric walls as both HP buffer and buildable area expansion
**Decision:** Wall rings define the buildable area. Wall 2 at level 4, wall 3 at level 8 expand the base.
**Why:** Solves three problems at once: spatial constraint, HP buffer, and visible progression milestone. Also gives the run a natural three-act structure without scripting it.

### Leveling costs gold (TFT-style)
**Decision:** Player spends gold to level up. Higher level increases passive income and upgrade rarity.
**Why:** Creates the classic TFT tension of "level now for long-term advantage vs spend on buildings for immediate power". The "hold out at low power to rush level 4" strategy is a valid high-risk line.

### Interest mechanic (TFT-style)
**Decision:** Every 10 gold held earns +1 bonus gold per income tick, capped at +5.
**Why:** Rewards disciplined saving. Penalises panic-spending. Creates an additional strategic consideration on top of the build vs level decision. Directly borrowed from TFT where it is a proven fun mechanic.

### Gold-generating buildings with no combat value
**Decision:** A Gold school of buildings exists that generates income but provides zero combat or defensive value.
**Why:** Creates a high-risk high-reward build path. Pure economy players must survive long enough for the income to compound. Adds diversity to build strategies without requiring complex new mechanics.

### Unity as engine
**Decision:** Prototype in Unity 3D (LTS) with C#, using URP.
**Why:** Team has existing Unity experience. Faster to prototype in a familiar environment than learn a new engine. Unity's Tilemap supports hex grids natively. Good asset pack import pipeline for 3D assets.

### Isometric camera with 3D assets on a 2D play plane
**Decision:** Fixed isometric orthographic camera. 3D assets. Gameplay logic is flat (X/Z plane, Y=0). Camera angle does not change during play.
**Why:** Asset packs are 3D fantasy assets. Isometric perspective suits the hex grid aesthetic and is standard for this genre. Keeping gameplay logic on a flat plane avoids 3D complexity while retaining the visual quality of 3D assets.
