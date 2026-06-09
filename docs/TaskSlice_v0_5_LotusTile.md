# Task Slice v0.5: The Lotus Roof Tile Of The Night Market

## Purpose

Turn the current open-map prototype from a movement and placement proof into one complete, playable day that a player can understand without designer explanation.

This slice must stay small enough for 1-2 iterations and must build on the current loop:

`open-map movement -> cleanup -> discover relic -> repair -> display`

The v0.5 upgrade is:

`take commission -> investigate -> collect evidence -> choose restoration path -> return or exhibit -> day summary`

## Scope Boundary

In scope:

- One day, one commission, one artifact.
- Three NPCs with short but functional dialogue.
- One meaningful restoration choice with different rewards.
- One official playable executable path.

Out of scope:

- Multi-day save/load.
- Full shop economy.
- Multiple artifact families in the same day.
- Walking animation frames.
- Final production art for all props on the map.

## Player Fantasy

The player is not a looter and not a generic shopkeeper. The player is a careful restorer in Chang'an who listens to people, reads physical clues, and decides what kind of future an old object should have.

The fantasy for this slice is:

- I walk through a lived-in district, not a menu.
- People trust me with an object because it matters to their memory.
- Repair is not just clicking a button. I choose what to preserve and what to sacrifice.
- My result changes how the neighborhood sees me.

## Target Experience

At the end of 8-12 minutes, the player should feel:

- "I completed a full commission from request to outcome."
- "The night market, rubbing yard, and bureau each had a clear role."
- "My choice between quick reuse and careful conservation actually mattered."
- "This can grow into a deeper management game with NPCs, reputation, and upgrades."

## Slice Pillar

### Commission Theme

`The Lotus Roof Tile Of The Night Market`

Han Niangzi, a night-market stall owner, brings a lotus-pattern roof tile fragment that survived an old market fire and was later reused in a family stall front. She wants help deciding whether it should be made presentable for continued use or conserved as a documented local object.

## Core Loop

1. Start the day inside the bureau courtyard.
2. Walk to the night market stall and talk to Han Niangzi.
3. Accept the lotus tile commission and receive the first objective.
4. Go to the relic yard and inspect the tile clue point.
5. Clear one obstacle and collect the lotus tile fragment.
6. Optionally visit Stele Rubbing Du to spend time and reveal the fire/reuse backstory.
7. Return to the bureau workbench.
8. Perform two restoration actions:
   - clean crack and soot
   - choose either quick fill or careful conservation
9. Go to the display/return point and resolve the commission:
   - return for reuse
   - exhibit with explanation
10. Read a day summary showing coins, trust, reputation, and next-step hint.

## NPCs And Dialogue Nodes

Only three NPCs are required in v0.5. Each NPC must serve gameplay, not decoration.

### 1. Han Niangzi

Role:

- commissioner
- emotional anchor for the slice
- outcome receiver

Required dialogue nodes:

1. `intro_request`
   - trigger: first talk at market stall
   - delivers: object context, family attachment, request for help
   - unlocks: `commission_active`
2. `midpoint_check`
   - trigger: talk again before restoration is completed
   - delivers: urgency reminder
   - no new state required
3. `reuse_resolution`
   - trigger: quick repair path chosen and item returned
   - delivers: practical gratitude
   - grants: coins + neighborhood trust
4. `exhibit_resolution`
   - trigger: careful conservation path chosen and item displayed
   - delivers: mixed emotion but deeper respect
   - grants: coins + scholarly reputation

### 2. Apprentice Dou

Role:

- tutorial voice
- workshop guide
- future progression bridge

Required dialogue nodes:

1. `tutorial_hint_start`
   - trigger: first time player regains control near bureau
   - delivers: where to find the commissioner
2. `repair_hint`
   - trigger: first arrival at workbench with artifact collected
   - delivers: explain the two restoration actions
3. `day_end_prompt`
   - trigger: after commission resolution
   - delivers: summary framing and upgrade tease

### 3. Stele Rubbing Du

Role:

- optional consultant
- time-cost tradeoff
- source of historical interpretation

Required dialogue nodes:

1. `consult_offer`
   - trigger: first talk while `artifact_collected` is true and `consulted_du` is false
   - cost: 1 work hour
   - delivers: reused-after-fire clue
2. `consult_repeat`
   - trigger: later talks after consultation
   - delivers: short reminder of interpretation

## State Machine

The slice should run as a small deterministic state machine, not a loose collection of triggers.

### Day State

`Boot -> FreeRoamStart -> CommissionOffered -> CommissionAccepted -> InvestigationOpen -> ArtifactCollected -> OptionalConsult -> RestorationReady -> RestorationResolved -> OutcomeResolved -> DaySummary -> End`

### Restoration State

`Damaged -> Cleaned -> ChoicePending -> QuickReuseFixed`  
or  
`Damaged -> Cleaned -> ChoicePending -> ConservedForExhibit`

### Dialogue Gating Rules

- Han Niangzi `intro_request` is available only once.
- Stele Rubbing Du consult is locked until the artifact is collected.
- Workbench interaction is locked until the artifact is collected.
- Outcome point is locked until one restoration branch is complete.
- Day summary is locked until one valid outcome is resolved.

### Failure Or Soft-Fail Rules

- If work hours reach 0 before restoration, the player can still walk and talk, but cannot perform new workbench or consult actions.
- If the player has 0 work hours after collecting the artifact, Dou should point them back to the summary/end-of-day fallback.
- This slice should avoid hard fail screens. Use constrained outcomes instead.

## Resources And Numbers

These values are already aligned with the current roadmap and are small enough to test clearly.

### Starting Resources

- Coins: `12`
- Work Hours: `5`
- Paste: `2`
- Stone Powder: `1`
- Neighborhood Trust: `0`
- Scholarly Reputation: `0`

### Costs

- Cleanup action: `0` hours, interaction only
- Collect artifact: `0` hours, interaction only
- Consult Stele Rubbing Du: `1` work hour
- Clean soot/crack at workbench: `1` work hour
- Join/fix for reuse: `1` work hour + `1` paste
- Careful conservation fill: `1` work hour + `1` stone powder

### Outcomes

- Quick repair and return:
  - `+12` coins
  - `+2` neighborhood trust
  - `+0` scholarly reputation
- Careful conservation and exhibit:
  - `+8` coins
  - `+0` neighborhood trust
  - `+2` scholarly reputation
  - `+2` next-day visitor income placeholder

### First Upgrade Hook

The day summary should mention but not require:

- `Clear Window Lamp`
  - cost: `10` coins
  - future effect: `+1` work hour at day start

## Implementation Slice

This should be split into two small iterations.

### Iteration 1: Complete One Stable Day

Goal:

- The official gameplay build launches reliably and completes one commission from talk to day summary.

Required behaviors:

- Commission acceptance from Han Niangzi.
- Objective text updates across the loop.
- One collectable lotus tile artifact.
- One optional consultation with Du.
- One repair station with two branch choices.
- One outcome resolution point.
- One day summary panel.

Validation evidence:

- Fresh launch video or screenshot set showing start, collection, restoration choice, and summary.
- Build launches twice without P0 or P1 issue.
- `RunLog.md` updated with exact branch result and any blockers.

Rollback note:

- If dialogue branching destabilizes the build, keep NPC placement and only reduce line count. Do not remove the commission state machine.

### Iteration 2: Readability And Emotional Payoff

Goal:

- Make the slice legible and satisfying without adding new systems.

Required behaviors:

- Distinct UI copy for quick reuse vs careful conservation.
- NPC resolution lines that acknowledge the chosen path.
- Day summary shows resource deltas and one next-day tease.
- Scene readability pass for artifact, workbench, and outcome point.

Validation evidence:

- Player test notes showing the choice difference was understood.
- Screenshot of both outcome summaries or one branch plus debug proof of the other.
- No UI overlap with core play space.

Rollback note:

- If extra presentation work risks the stable build, preserve functional dialogue and state feedback first.

## Minimum Acceptance Standard

The slice is accepted only if all of the following are true:

1. The official gameplay executable launches twice from a fresh start.
2. The player can finish one full day without using debug-only controls.
3. At least two player choices affect outcome:
   - whether to consult Du
   - which restoration branch to choose
4. Han Niangzi, Apprentice Dou, and Stele Rubbing Du all appear in the flow with gated dialogue.
5. The day ends with a readable summary of coins, work hours spent, and either trust or reputation gain.
6. The same slice works with the existing open map layout and does not rely on a separate preview-only build.

## Unity Deliverables

The Unity implementation agent should deliver the following, and each item must be observable in build or Play Mode.

### Scene And Flow

- One stable official scene or runtime-built scene path for the full v0.5 loop.
- NPC interaction points for Han Niangzi, Apprentice Dou, and Stele Rubbing Du.
- One commission state controller for the lotus tile day.
- One day summary UI state.

### Systems

- Dialogue trigger system that can gate lines by simple state flags.
- Objective tracker that updates across:
  - talk to Han
  - investigate relic yard
  - collect tile
  - optional consult
  - repair
  - resolve outcome
- Restoration branch choice UI with two explicit buttons or equivalent inputs.
- Resource tracker for coins, hours, paste, stone powder, trust, and reputation.

### Placement And Readability

- No black map edge visible during the slice.
- NPCs and interactables must respect y-sort and foreground occlusion rules already established in v0.4.
- Artifact pickup point, workbench, and outcome point must stay readable at gameplay camera scale.

### Validation

- Unity compile clean.
- Windows build path remains:
  `Build/ChanganRestorationBureau/ChanganRestorationBureau.exe`
- `RunLog.md` updated with build result and flow result.

## Art Deliverables

The art agent should deliver only the assets needed for this slice. Missing assets may use controlled fallback, but only temporarily and only if slot contract stays stable.

### Required New Or Updated Assets

1. `npc_han_niangzi_idle`
   - role: commissioner at market
   - format: transparent PNG
   - target size: `512x512`
   - pivot: bottom-center
2. `npc_apprentice_dou_idle`
   - role: bureau guide
   - format: transparent PNG
   - target size: `512x512`
   - pivot: bottom-center
3. `npc_stele_du_idle`
   - role: consultant at rubbing yard
   - format: transparent PNG
   - target size: `512x512`
   - pivot: bottom-center
4. `artifact_lotus_tile_damaged`
   - role: field pickup and pre-repair view
   - format: transparent PNG
   - target size: `512x512`
   - pivot: center or bottom-center, must be declared
5. `artifact_lotus_tile_quickfix`
   - role: reuse outcome
   - format: transparent PNG
   - target size: `512x512`
6. `artifact_lotus_tile_conserved`
   - role: exhibit outcome
   - format: transparent PNG
   - target size: `512x512`
7. `ui_day_summary_panel`
   - role: day-end result panel
   - target size declared in spec before import
8. `ui_restoration_choice_panel`
   - role: quick repair vs careful conservation
   - target size declared in spec before import

### Required Art Constraints

- Keep the hand-painted Guanzhong direction already chosen for the open map.
- NPC costumes should read as district-specific and grounded, not fantasy combat attire.
- The lotus tile must remain readable at gameplay camera distance.
- The two restored tile states must look meaningfully different in silhouette or surface treatment, not just color tint.
- Do not shift world positions in Unity to compensate for bad crop, padding, or pivot. Fix the asset contract first.

### Asset QA Requirements

- Add each new asset to the catalog before production import.
- Stage first, then promote to production.
- Keep slot id, pivot, PPU, and scene scale consistent across damaged and restored states.
- Resolve the existing background size warning before final art lock if it affects camera clamp or framing.

## Player Test Focus

The player test agent should specifically judge:

- Is the commission understandable on first read?
- Does consulting Du feel optional but valuable?
- Does the restoration choice feel different enough to justify replay?
- Does the day summary make the consequences obvious?
- Does any interaction point feel visually misplaced or hard to read on the map?

## Definition Of Done For This Slice

This document is complete when it guides one or two implementation passes without requiring design reinterpretation mid-build.

That means:

- the player fantasy is clear
- the loop is finite
- the state machine is explicit
- the numbers are testable
- Unity knows what to build
- art knows what to draw
- QA knows what to reject
