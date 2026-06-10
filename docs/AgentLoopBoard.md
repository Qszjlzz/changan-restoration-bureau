# Agent Loop Board

## Current Release Target

Ship `v0.5` as one stable, replayable in-game day for `The Lotus Roof Tile Of The Night Market` through the official Windows executable.

## Current Player-Test Problem

The playable is stable enough to launch and finish, but it still feels more like a guided proof than a real management day. The biggest missing player-facing ingredient is a visible restoration decision that changes route, rewards, and NPC response.

## Current Approved Slice

### Slice A: Workbench Branch Choice

- Owner: Unity Implementation + Game Design
- Goal: turn the current repair step into a real choice between `Quick Reuse` and `Careful Exhibit`
- Acceptance:
  - quick reuse resolves directly with Han Niangzi
  - careful exhibit still requires the display case
  - branch choice changes summary text and rewards
  - official build still passes the two-launch smoke gate

### Slice B: Visible Day Budget

- Owner: Unity Implementation + Player Test
- Goal: make `WorkHours`, `Paste`, and `StonePowder` visible and understandable during play
- Acceptance:
  - consultation and restoration costs are visible before the player commits
  - blocked actions explain why they are blocked
  - players can explain the tradeoff they made after one run

## Agent Assignments

### Orchestrator

- Keep the branch `codex/art-v0-4` healthy.
- Protect user edits, especially generated scenes and art.
- Only commit after asset QA and build/launch QA pass.

### Game Design Agent

- Keep scope locked to the one-day loop.
- Prefer choices that change route, reward, or NPC reaction.
- Reject new content that does not improve decision quality.

### 2D Art Agent

Current priority batch:

1. `npc_han_niangzi_idle` and `portrait_han_niangzi`
2. `npc_apprentice_dou_idle` and `portrait_apprentice_dou`
3. `npc_stele_du_idle` and `portrait_stele_du`
4. `furniture_workbench` production replacement
5. `furniture_display_case` production replacement
6. `artifact_roof_tile_quickfix` and `artifact_roof_tile_conserved`

Placement invariants:

- keep slot ids, pivots, scale, and world positions stable
- fix source crop/padding instead of dragging scene objects
- validate staging -> production -> Unity Production before handoff

### Unity Implementation Agent

Current responsibilities:

- keep the official playable build as the only truth path
- wire restoration branch choice into interaction, UI, dialogue, and summary
- preserve map edge clamp, camera clamp, y-sort, and occlusion behavior

### Player Test Agent

Current must-check items:

- two consecutive official launches
- branch choice readability
- quick reuse route without display-case dependency
- careful exhibit route with display-case dependency
- objective text, dialogue hints, and summary clarity
- art placement remains coherent after any sprite replacement

## Return Rules

- Return to `Unity` for crashes, broken progression, blocked interactions, or missing runtime references.
- Return to `Art` for bad crop, alpha fringe, scale drift, broken slot fit, or style mismatch.
- Return to `Design` for weak pacing, unclear stakes, uninteresting rewards, or meaningless choices.

## Next Queue After Current Slice

1. visible day budget HUD
2. dedicated NPC production art
3. production workbench and display case replacement
4. one extra optional narrative beat or ambient NPC reaction after outcome resolution
