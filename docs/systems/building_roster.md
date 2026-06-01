# Building Roster

## System Rules (Recap)

- Buildings are placed inside the walled base on a hex grid
- Each building requires a blueprint (found by capturing hex zones)
- Placing costs gold
- 3 of the same building type merge into a tier 2 — the first two placements are destroyed, the third location becomes the tier 2
- Tier 2 → tier 3 follows the same rule
- Buildings influence adjacent buildings (adjacency synergies)
- Selling returns 50% gold, blueprint is permanently lost
- Tier 2+ buildings cannot be sold (TBD — under review)

---

## Building Schools

Buildings belong to a school. Same-school adjacency unlocks synergy bonuses. Cross-school adjacency can also trigger specific bonuses (listed per building).

| School | Theme | Color Code |
|---|---|---|
| Arcane | Magic, mana, spell power | Purple |
| Martial | Physical power, speed, area | Red |
| Nature | Healing (wall), sustain, area growth | Green |
| Gold | Economy, income | Yellow |

---

## Building Roster (Draft)

> Status: `concept`. Numbers are placeholders for prototype.

---

### Arcane School

#### Mage Tower
- **Tier 1:** Player gains +1 spell damage. Adjacency bonus: +5% spell damage per adjacent Arcane building.
- **Tier 2:** Player gains +3 spell damage, spells apply slow. Inherits and doubles tier 1 adjacency bonus.
- **Tier 3:** Player gains +6 spell damage, spells pierce one enemy.

#### Runestone
- **Tier 1:** Every 30 seconds, player's next attack is empowered (double damage).
- **Tier 2:** Cooldown reduced to 20 seconds, empowered attack also stuns briefly.
- **Tier 3:** Cooldown 10 seconds, empowered attack chains to nearby enemies.

---

### Martial School

#### Barracks
- **Tier 1:** Player gains +10% attack speed.
- **Tier 2:** +25% attack speed, player gains a short dash ability.
- **Tier 3:** +40% attack speed, dash resets on kill.

#### Forge
- **Tier 1:** Player gains +2 physical damage.
- **Tier 2:** +5 physical damage, attacks have a chance to cause bleed.
- **Tier 3:** +10 physical damage, bleed stacks.

---

### Nature School

#### Grove
- **Tier 1:** Slows enemies within a radius of the building (passive aura, building must be intact).
- **Tier 2:** Larger slow radius, slow is stronger.
- **Tier 3:** Enemies in radius take damage over time.

#### Shrine
- **Tier 1:** Wall HP degradation rate reduced by 10% (enemies deal slightly less wall damage).
- **Tier 2:** 20% reduction, wall segments adjacent to shrine take reduced damage.
- **Tier 3:** 30% reduction, occasionally repels a single enemy strike (absorbed).

> Note: Shrine does not repair walls — it reduces incoming damage. Consistent with no-repair design pillar.

---

### Gold School

#### Market
- **Tier 1:** +1 gold per income tick.
- **Tier 2:** +3 gold per income tick.
- **Tier 3:** +6 gold per income tick, interest cap raised by +1.

#### Treasury
- **Tier 1:** Interest threshold lowered (earn interest at 8 gold instead of 10).
- **Tier 2:** Threshold lowered to 6 gold, interest still capped at +5.
- **Tier 3:** Threshold 4 gold, interest cap raised to +7.

---

## Adjacency Synergy Matrix (Draft)

| Building A | Adjacent to | Effect |
|---|---|---|
| Any Arcane | Any Arcane | +5% spell damage each |
| Any Martial | Any Martial | +5% attack speed each |
| Market | Any Gold | +0.5 gold per tick extra |
| Mage Tower | Forge | Attacks deal both physical and spell damage type |
| Barracks | Grove | Dash also applies slow |

> Cross-school synergies to be expanded during spec phase.

---

## Design Notes

- The roster should have enough variety that multiple viable build paths exist (full Arcane, full Martial, mixed economy, economy-into-combat)
- Tier 3 buildings are rare and should feel like a run-defining moment
- Gold school buildings are intentionally weaker in combat to maintain risk/reward tension
- Synergy matrix should be expanded before prototype is built
