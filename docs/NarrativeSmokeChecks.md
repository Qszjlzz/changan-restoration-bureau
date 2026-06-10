# Narrative Smoke Checks

Use this checklist for the first narrative-facing smoke pass on the current `v0.5` skeleton. This is narrower than a full playtest report: it is a fast gate to confirm that NPC anchors, dialogue entry, commission state, objective text, and official build startup all still work together after each Unity/content iteration.

This checklist assumes the current target slice from [TaskSlice_v0_5_LotusTile.md](D:/游戏工作流/new-sim-management-game/docs/TaskSlice_v0_5_LotusTile.md):

`take commission -> investigate -> collect evidence -> choose restoration path -> return or exhibit -> day summary`

---

## 1. Session Header

| Field | Fill |
| --- | --- |
| Test date |  |
| Tester | Player Test Agent |
| Branch / commit |  |
| Build target | Official gameplay executable |
| Build path | `Build/ChanganRestorationBureau/ChanganRestorationBureau.exe` |
| Smoke script used | `tools/smoke_play_proof.ps1` |
| Verdict | `PASS` / `RETURN_TO_UNITY` / `RETURN_TO_DESIGN` / `RETURN_TO_NARRATIVE_DATA` |
| Highest issue seen |  |

## 2. Gate Rule

Stop and return immediately if any of the following happens:

- Official executable fails either launch attempt.
- Smoke-play script fails, crashes, times out, or misses expected runtime markers.
- Any required NPC anchor is missing, invisible, unreachable, or unreadable from normal play distance.
- The player cannot start the commission, cannot advance to the next day phase, or objective text stops matching the real state.

## 3. Combined Build And Smoke Gate

Run this section before any manual narrative walk.

### Startup Check

| Check | Pass? | Notes |
| --- | --- | --- |
| Official executable launches from a clean start |  |  |
| Official executable launches a second time from a clean start |  |  |
| No native crash, freeze, or black screen on either launch |  |  |
| `Player.log` stays free of scene corruption or missing-script spam |  |  |
| Title screen / first controllable frame appears in reasonable time |  |  |

### Smoke-Play Script Check

| Check | Pass? | Notes |
| --- | --- | --- |
| `tools/smoke_play_proof.ps1 -Branch careful_exhibit` completes without failure |  |  |
| `tools/smoke_play_proof.ps1 -Branch quick_reuse` completes without failure |  |  |
| Script confirms expected startup markers |  |  |
| Script confirms expected interaction markers for both branches |  |  |
| Script exit code is `0` |  |  |
| Manual launch result and smoke-play result agree |  |  |

### Evidence

- Build used:
- Log checked:
- Crash folder checked:
- Smoke-play output summary:

## 4. NPC Anchor Visibility

Current v0.5 narrative anchors should at minimum cover:

- `Han Niangzi`
- `Apprentice Dou`
- `Stele Rubbing Du`

For each anchor, test from normal player spawn and normal walking route, not only from editor knowledge.

| NPC Anchor | Visible? | Reachable? | Readable at play distance? | Notes |
| --- | --- | --- | --- | --- |
| Han Niangzi |  |  |  |  |
| Apprentice Dou |  |  |  |  |
| Stele Rubbing Du |  |  |  |  |

Check these details while walking the map:

- The anchor is not hidden behind foreground art in a misleading way.
- The anchor does not blend into the background so much that the player misses it.
- The anchor feels intentionally placed in the world, not dropped as a debug marker.
- The anchor remains readable while the camera follows the player naturally.

## 5. Dialogue Entry Trigger

This section verifies the first layer of narrative interaction, even if the full branching dialogue is still placeholder.

| Check | Pass? | Notes |
| --- | --- | --- |
| Han Niangzi interaction prompt appears when the player is close enough |  |  |
| Talking to Han Niangzi triggers the intended commission-opening dialogue |  |  |
| Apprentice Dou can be spoken to at the expected time |  |  |
| Stele Rubbing Du stays locked until the intended phase, if phase gating exists |  |  |
| Dialogue does not trigger from too far away or through blocking geometry |  |  |
| Dialogue does not retrigger incorrectly after the state has changed |  |  |

Short notes:

- Best entry point:
- Most confusing entry point:
- Any NPC that feels easy to miss:

## 6. Commission Acceptance

This section is only about getting the player from free roam into a real owned task.

| Check | Pass? | Notes |
| --- | --- | --- |
| First commissioner conversation clearly communicates the request |  |  |
| Player can accept the commission without debug fallback |  |  |
| Accepting the commission changes the active day state |  |  |
| A fresh objective appears immediately after acceptance |  |  |
| Player can tell where to go next without external explanation |  |  |
| Player can restate what the tile is and why it matters after the first talk |  |  |

Decision note:

- Did the commission feel personal enough to care about?
- Did the object and NPC relationship feel clear yet?

## 7. Phase Transition Checks

Target flow for the current narrative skeleton:

`FreeRoamStart -> CommissionAccepted -> InvestigationOpen -> ArtifactCollected -> OptionalConsult -> RestorationReady -> RestorationResolved -> OutcomeResolved -> DaySummary`

Mark each phase only when the game gives a visible or inspectable sign that the phase really changed.

| Phase Change | Pass? | Evidence Seen |
| --- | --- | --- |
| Free roam to commission accepted |  |  |
| Commission accepted to investigation open |  |  |
| Investigation open to artifact collected |  |  |
| Artifact collected to optional consult available |  |  |
| Artifact collected or consult path to restoration ready |  |  |
| Restoration ready to restoration resolved |  |  |
| Restoration resolved to outcome resolved |  |  |
| Outcome resolved to day summary |  |  |

Failure notes:

- Any phase that changed silently:
- Any phase that advanced too early:
- Any phase that failed to advance:
- Any phase that could be broken by talking to the wrong NPC or revisiting an old point:

## 8. Objective Copy Change Check

Objective text is part of the narrative contract. The player should be able to track the story state from the objective copy alone.

| Moment | Objective updates? | Copy feels correct? | Notes |
| --- | --- | --- | --- |
| Initial spawn |  |  |  |
| After first talk with Han Niangzi |  |  |  |
| After commission acceptance |  |  |  |
| After clue point or artifact discovery |  |  |  |
| After artifact pickup |  |  |  |
| After optional consult |  |  |  |
| After repair choice unlock |  |  |  |
| After quick reuse branch resolution |  |  |  |
| After careful exhibit branch resolution |  |  |  |
| After return/exhibit outcome |  |  |  |
| At day summary |  |  |  |

Objective quality notes:

- Any objective text that is too vague:
- Any objective text that lags behind actual state:
- Any objective text that spoils a choice too early:
- Any objective text that still sounds like a debug placeholder:

## 9. Combined Narrative Smoke Verdict

Use this to decide who gets the return.

### PASS

- Official build launches twice.
- Smoke-play script passes.
- All required NPC anchors are visible and reachable.
- Commission starts successfully.
- Core day phases advance in the expected order.
- Objective text stays aligned with the active narrative state.

### RETURN_TO_UNITY

- Trigger volumes, state transitions, anchors, prompts, or launch stability are broken.

### RETURN_TO_DESIGN

- Flow technically works, but the player does not understand why they are helping, where to go, or what changed after each phase.

### RETURN_TO_NARRATIVE_DATA

- Dialogue entry ids, objective text, commission metadata, or phase copy are present but mismatched or incomplete.

## 10. Handback Notes

- Highest-priority issue:
- Suggested owner:
- Repro steps:
- What still feels good:
- What most needs the next pass:
