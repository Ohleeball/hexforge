# HexForge Devlog

Append-only session log. Never edit or delete past entries. Add a new entry at the end of every session.

Format:
```
## YYYY-MM-DD — [Short session title]
**What was done:** Summary of work completed this session.
**Decisions made:** Any choices worth noting (with reasoning in the relevant system doc).
**Left off at:** Where to pick up next session.
```

---

## 2026-06-04 — Documentation process established

**What was done:** Reviewed and restructured the documentation process. Added a `## Documentation Process` section to `cursor_guidelines.md` defining rules for both Claude Chat and Claude Code. Created this `DEVLOG.md`. Absorbed `camera_task.md` into `current_task.md` and deleted the per-task file. Added a `## Decisions` section to `docs/systems/camera.md` as the first example of the decision-recording pattern.

**Decisions made:** Per-task `.md` files (e.g. `camera_task.md`) are eliminated. Decision reasoning lives in the relevant system design doc under a `## Decisions` heading. Cross-cutting decisions go in `architecture.md`. See `cursor_guidelines.md` → Documentation Process for the full rules.

**Left off at:** Phase 1 Foundation is in progress. Camera system design is complete and ready for implementation. Next: implement player character movement.
