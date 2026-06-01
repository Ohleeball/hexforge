# Economy System

## Gold Sources

| Source | Amount | Notes |
|---|---|---|
| Enemy kill | Small flat amount | Primary combat income |
| Hex zone capture | Medium lump sum | Scales with zone distance from base |
| Passive income tick | Varies by level | Every 5–10 seconds (TBD exact interval) |
| Interest | +1 per 10 gold held | Calculated at each income tick |
| Gold buildings | Passive per tick | No combat value, high-risk investment |

---

## Passive Income

Income ticks on a fixed interval (prototype default: every 10 seconds). At each tick the player receives:

```
base income (by level) + interest bonus + gold building output
```

The tick interval is visible to the player via a UI countdown so they can make timing decisions around it.

---

## Interest Mechanic (TFT-Style)

At each income tick:
- Every 10 gold the player is currently holding earns +1 bonus gold
- Capped at +5 (holding 50+ gold)

| Gold held | Interest bonus |
|---|---|
| 0–9 | +0 |
| 10–19 | +1 |
| 20–29 | +2 |
| 30–39 | +3 |
| 40–49 | +4 |
| 50+ | +5 (cap) |

This rewards disciplined saving and penalizes panic-spending. Combined with the leveling cost, it creates a meaningful decision each tick.

---

## Player Leveling

Leveling costs gold and has two effects:
1. **Increases base passive income** at each tick
2. **Increases rarity of upgrade options** offered when capturing zones

| Level | Level-up cost | Base income per tick | Notes |
|---|---|---|---|
| 1 | — | 2 | Starting level |
| 2 | 4 | 3 | |
| 3 | 6 | 4 | |
| 4 | 8 | 5 | Wall 2 unlocks |
| 5 | 10 | 6 | |
| 6 | 12 | 7 | |
| 7 | 14 | 8 | |
| 8 | 18 | 10 | Wall 3 unlocks |

> All values are prototype estimates. Balance pass required.

---

## Gold Sinks

| Sink | Notes |
|---|---|
| Place a building | Fixed cost per building type, paid on placement |
| Level up | Cost increases per level |
| (No wall repair) | Walls permanently damaged — intentional |

---

## Gold Buildings

A category of buildings that generate gold per income tick but provide no combat or upgrade value. They occupy a building slot on the hex grid.

- High-risk: they take up space that could be a combat upgrade
- High-reward: compound over a long run
- Can be sold for 50% gold if the risk becomes too high
- Blueprint lost on sale

Adjacency interactions with gold buildings are TBD — possible design: combat buildings adjacent to a gold building receive a small income bonus, incentivizing mixed placement rather than isolated gold farming.

---

## Design Notes

- The tension between spending on buildings, saving for interest, and leveling up is the primary strategic decision space of the economy
- "Hold out at low power to level faster" is a valid high-risk strategy, borrowed from TFT
- Gold should feel scarce enough that every decision matters but not so scarce that the player feels stuck
