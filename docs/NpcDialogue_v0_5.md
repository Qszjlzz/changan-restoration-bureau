# NPC Dialogue v0.5

## Purpose

Define the first shippable dialogue entry points for the three required NPCs in the `v0.5` commission slice:

- `Han Niangzi`
- `Apprentice Dou`
- `Stele Rubbing Du`

This document stays intentionally small. It is meant to give Unity a stable dialogue gate and phase-transition contract for the first playable day around `The Lotus Roof Tile Of The Night Market`.

## Phase Reference

Current runtime enum:

`FreeRoamStart -> CommissionAccepted -> InvestigationOpen -> ArtifactCollected -> ConsultationOpen -> RestorationReady -> OutcomeResolved -> DaySummary`

Dialogue should only do two kinds of work in `v0.5`:

1. gate what the player is allowed to do next
2. move `ProofDayPhase` forward when the player completes a key conversation

Dialogue should not introduce side branches, inventory choices, or relationship meters in this pass.

## Shared Dialogue Rules

- Each NPC has one `first talk` node and one `repeat talk` node as the minimum requirement.
- `First talk` should only fire once per relevant phase window.
- `Repeat talk` should be short and should not re-teach the whole slice.
- Only one dialogue node per interaction should be eligible at runtime.
- If a talk does not advance state, it should still reinforce the current objective in one sentence.
- Placeholder text stays in English so the current UI can ship with neutral temporary copy.

## NPC 1: Han Niangzi

### Location

- World role: commissioner and outcome receiver
- Landmark: `landmark_market`
- Suggested anchor: near the night-market stall front
- Suggested open-map position: `(-3.50, -2.35)` with a small local offset for sprite feet alignment

### Phase Design

| Node Id | Use | Preconditions | On Trigger | Resulting `ProofDayPhase` |
| --- | --- | --- | --- | --- |
| `han_intro_request` | first talk | current phase is `FreeRoamStart` | accept the commission and set the first objective | `CommissionAccepted`, then immediately `InvestigationOpen` after the dialogue closes |
| `han_repeat_before_tile` | repeat talk | current phase is `CommissionAccepted` or `InvestigationOpen` | no state change; remind player to inspect the relic yard | no change |
| `han_repeat_after_tile` | repeat talk | current phase is `ArtifactCollected` or `ConsultationOpen` or `RestorationReady` | no state change; remind player to return after repair is decided | no change |
| `han_resolution_reuse` | outcome talk | restoration branch is `quick_reuse` and current phase is `RestorationReady` | resolve return path rewards | `OutcomeResolved` |
| `han_resolution_exhibit` | outcome talk | restoration branch is `careful_exhibit` and current phase is `RestorationReady` | resolve exhibit path rewards | `OutcomeResolved` |
| `han_post_resolution` | repeat talk | current phase is `OutcomeResolved` or `DaySummary` | no state change; short gratitude line | no change |

### First Dialogue Placeholder

Node: `han_intro_request`

Player-facing placeholder text:

1. `"This tile was part of my family's old stall front."`
2. `"After the fire, we kept what we could. I do not know whether to use it again or keep it safe."`
3. `"Will you take a look for me?"`

### Repeat Dialogue Placeholder

Node: `han_repeat_before_tile`

1. `"The lotus tile should still be near the old salvage heap."`
2. `"Please tell me what kind of future it can still have."`

Node: `han_repeat_after_tile`

1. `"You found it? Good. The soot always made the pattern hard to read."`
2. `"When you decide, I will trust your judgment."`

Node: `han_post_resolution`

1. `"Thank you. It feels lighter now, whatever shape its future takes."`

### Unity Minimum Field Suggestion

Required NPC-level fields:

- `npcId`: `han_niangzi`
- `displayName`: `Han Niangzi`
- `homeLandmarkId`: `landmark_market`
- `worldAnchorId`: `npc_han_market`
- `defaultFacing`: `right`

Required dialogue-node fields:

- `nodeId`
- `speakerNpcId`
- `phaseWhitelist`
- `requiredFlags`
- `forbiddenFlags`
- `lines`
- `nextPhase`
- `grantFlags`
- `objectiveText`
- `oneShot`

Suggested flags for Han:

- `commission_offered`
- `lotus_tile_collected`
- `lotus_tile_restored`
- `outcome_reuse`
- `outcome_exhibit`

## NPC 2: Apprentice Dou

### Location

- World role: guide, tutorial anchor, workshop bridge
- Landmark: `landmark_bureau`
- Suggested anchor: bureau courtyard or near the workbench path
- Suggested open-map position: `(-5.10, 1.15)` if placed outdoors, or slightly beside the repair table if the bureau interior edge is used

### Phase Design

| Node Id | Use | Preconditions | On Trigger | Resulting `ProofDayPhase` |
| --- | --- | --- | --- | --- |
| `dou_start_hint` | first talk | current phase is `FreeRoamStart` and `han_intro_request` has not fired | no hard phase change; point player toward Han Niangzi | no change |
| `dou_repeat_pre_accept` | repeat talk | current phase is `FreeRoamStart` and `han_intro_request` has not fired | no state change; shorter route reminder | no change |
| `dou_repair_hint` | first talk for mid slice | current phase is `ArtifactCollected` and player is near bureau/workbench | unlock workbench framing and nudge next step | `RestorationReady` |
| `dou_repeat_repair` | repeat talk | current phase is `RestorationReady` and outcome not yet resolved | no state change; explain the two restoration intentions in one line | no change |
| `dou_day_end_prompt` | first talk for closing | current phase is `OutcomeResolved` | send player to summary state | `DaySummary` |
| `dou_post_summary` | repeat talk | current phase is `DaySummary` | no state change; teaser for the next work day | no change |

### First Dialogue Placeholder

Node: `dou_start_hint`

1. `"Master, Han Niangzi is looking for you at the night market."`
2. `"She brought something from an old stall front. It sounds important."`

Node: `dou_repair_hint`

1. `"The tile can be cleaned now."`
2. `"After that, you must decide whether to ready it for use or conserve it properly."`

### Repeat Dialogue Placeholder

Node: `dou_repeat_pre_accept`

1. `"The market awning with the red trim. She is waiting there."`

Node: `dou_repeat_repair`

1. `"Quick reuse will help the stall sooner. Careful conservation will preserve more of the story."`

Node: `dou_day_end_prompt`

1. `"That settles the commission for today."`
2. `"Come, let us write the result into the day ledger."`

Node: `dou_post_summary`

1. `"Tomorrow's work will be easier if we keep both the tools and the stories in order."`

### Unity Minimum Field Suggestion

Required NPC-level fields:

- `npcId`: `apprentice_dou`
- `displayName`: `Apprentice Dou`
- `homeLandmarkId`: `landmark_bureau`
- `worldAnchorId`: `npc_dou_bureau`
- `defaultFacing`: `down`

Required dialogue-node fields:

- `nodeId`
- `speakerNpcId`
- `phaseWhitelist`
- `requiredFlags`
- `forbiddenFlags`
- `lines`
- `nextPhase`
- `grantFlags`
- `objectiveText`
- `oneShot`

Suggested flags for Dou:

- `dou_start_seen`
- `dou_repair_seen`
- `dou_summary_seen`

## NPC 3: Stele Rubbing Du

### Location

- World role: optional consultant and history interpreter
- Landmark: `landmark_stele_yard`
- Suggested anchor: front edge of the rubbing yard where the player can reach him without walking behind heavy occluders
- Suggested open-map position: `(4.35, 2.35)` with slight downward offset so his feet read clearly against the ground plane

### Phase Design

| Node Id | Use | Preconditions | On Trigger | Resulting `ProofDayPhase` |
| --- | --- | --- | --- | --- |
| `du_locked_hint` | repeat talk | current phase is before `ArtifactCollected` | no state change; lightly deflect until the player has the tile | no change |
| `du_consult_offer` | first talk | current phase is `ArtifactCollected`, `consulted_du` is false, and player still has at least `1` work hour | spend one work hour, reveal reuse-after-fire clue, update objective | `ConsultationOpen` |
| `du_repeat_after_consult` | repeat talk | current phase is `ConsultationOpen` or `RestorationReady` and `consulted_du` is true | no state change; shorten the historical reading into one reminder | no change |
| `du_no_time` | repeat talk | current phase is `ArtifactCollected` and remaining work hours are `0` | no state change; direct player back to the bureau | no change |

### First Dialogue Placeholder

Node: `du_consult_offer`

1. `"The soot line is wrong for simple weathering."`
2. `"This tile was scorched, then set back into use later. Someone chose survival over symmetry."`
3. `"If you restore it, decide whether you are preserving a roof tile or a neighborhood memory."`

### Repeat Dialogue Placeholder

Node: `du_locked_hint`

1. `"Bring me the object itself, and I may tell you more."`

Node: `du_repeat_after_consult`

1. `"Fire broke it. Reuse gave it a second life. Your repair should respect that choice."`

Node: `du_no_time`

1. `"You already know enough to act. Spend the rest of the day at the bench, not here."`

### Unity Minimum Field Suggestion

Required NPC-level fields:

- `npcId`: `stele_du`
- `displayName`: `Stele Rubbing Du`
- `homeLandmarkId`: `landmark_stele_yard`
- `worldAnchorId`: `npc_du_stele_yard`
- `defaultFacing`: `left`

Required dialogue-node fields:

- `nodeId`
- `speakerNpcId`
- `phaseWhitelist`
- `requiredFlags`
- `forbiddenFlags`
- `lines`
- `nextPhase`
- `grantFlags`
- `objectiveText`
- `resourceDelta`
- `oneShot`

Suggested flags for Du:

- `consulted_du`
- `du_consult_seen`

Suggested resource delta for `du_consult_offer`:

- `workHours: -1`

## Cross-NPC Transition Notes

These are the intended first-pass phase moves:

1. `Han Niangzi` first talk:
   - `FreeRoamStart -> CommissionAccepted -> InvestigationOpen`
2. `Apprentice Dou` repair hint:
   - `ArtifactCollected -> RestorationReady`
3. `Stele Rubbing Du` consult:
   - `ArtifactCollected -> ConsultationOpen`
   - returning to the bureau and opening repair flow should still end in `RestorationReady`
4. `Han Niangzi` outcome talk:
   - `RestorationReady -> OutcomeResolved`
5. `Apprentice Dou` end-of-day talk:
   - `OutcomeResolved -> DaySummary`

`ConsultationOpen` should be treated as an informational branch state, not a dead end. The player must still be able to proceed into `RestorationReady` after speaking with Du.

## Minimal Unity Data Shape

The implementation layer does not need a full dialogue tree system for this pass. A flat, gated node list is enough.

Suggested minimum data shape per node:

```json
{
  "nodeId": "han_intro_request",
  "speakerNpcId": "han_niangzi",
  "phaseWhitelist": ["FreeRoamStart"],
  "requiredFlags": [],
  "forbiddenFlags": ["commission_offered"],
  "lines": [
    "This tile was part of my family's old stall front.",
    "After the fire, we kept what we could.",
    "Will you take a look for me?"
  ],
  "nextPhase": "InvestigationOpen",
  "grantFlags": ["commission_offered"],
  "objectiveText": "Inspect the relic yard for the lotus roof tile.",
  "resourceDelta": null,
  "oneShot": true
}
```

Suggested minimum data shape per NPC anchor:

```json
{
  "npcId": "han_niangzi",
  "displayName": "Han Niangzi",
  "homeLandmarkId": "landmark_market",
  "worldAnchorId": "npc_han_market",
  "positionHint": { "x": -3.5, "y": -2.35 },
  "idleAssetId": "npc_han_niangzi_idle",
  "portraitAssetId": "portrait_han_niangzi"
}
```

## Acceptance Check For This Document

This dialogue spec is ready for implementation when:

- each NPC has a stable world anchor
- each NPC has a `first talk` and a `repeat talk` path
- every state-changing conversation points to one explicit `ProofDayPhase`
- placeholder UI copy is already written
- Unity can implement it with flat node data instead of a full conversation editor
