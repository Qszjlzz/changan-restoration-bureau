# Player Test Report Template

Use this template for player-facing test returns after a playable build, launch attempt, or art-placement pass. The tone should read like a real playtest handoff: what happened, what blocked play, what felt good, what broke immersion, and which agent owns the next fix.

---

## 1. Session Header

| Field | Fill |
| --- | --- |
| Test date |  |
| Tester | Player Test Agent |
| Branch / commit |  |
| Build target | Official gameplay executable / editor play mode / preview build |
| Build path | `Build/ChanganRestorationBureau/ChanganRestorationBureau.exe` |
| Focus of this pass | launch stability / movement / art placement / v0.5 day loop / other |
| Verdict | `PASS` / `RETURN_TO_UNITY` / `RETURN_TO_ART` / `RETURN_TO_DESIGN` |
| Highest issue seen |  |
| Next return target | Unity / Art / Design / Orchestrator |

## 2. Severity Rules

### P0

Ship blocker. Stop the pass and return immediately.

- Build fails, executable does not launch, native crash, hard freeze, black screen, or repeated scene-load corruption.
- Player cannot move, cannot interact, or cannot finish the current target flow.
- Camera shows unusable black void or map breaks in a way that destroys navigation.
- Critical references or runtime state are missing, causing the playable loop to fail.

### P1

Core experience works only partially. Must be fixed before calling the slice stable.

- Movement, collision, edge clamp, or camera clamp feels unreliable.
- Occlusion, sorting, or blocking volumes make navigation or reading the scene confusing.
- Cleanup, sampling, repair, or display works inconsistently or only through debug fallback.
- UI covers the player, hides the main play area, or gives unclear state feedback.
- Art placement is visibly off in a way that harms readability: wrong scale, pivot drift, floating props, severe alpha fringe, wrong slot fit.

### P2

Noticeable quality issue that does not stop flow completion.

- Minor art mismatch, mild position drift, small sorting pop, copy issue, mojibake text, weak feedback, or uneven polish.
- A mechanic technically works but feels flat, repetitive, under-explained, or not rewarding enough.

### P3

Nice-to-have improvement or future enhancement.

- Additional flavor, NPC lines, story texture, pacing ideas, reward tuning, optional interactions, or comfort improvements.

## 3. Launch And Startup Check

### Result

- Launch attempt count:
- Did the official executable open twice from a clean start?
- Time to first visible frame:
- Did a game window appear?
- Did `Player.log` stay free of `level0 corrupted`, `Position out of bounds`, missing script spam, null-reference spam, or native crash tail?

### Evidence

- Build used:
- Log checked:
- Crash folder checked:
- Screenshot or clip path:

### Startup Notes

- First visible impression:
- Any delay, hitch, blank frame, or immediate crash:
- Any difference between official build and preview build:

## 4. Current Playable Loop Check

Target chain for the current proof:

`move -> clear obstacle -> uncover relic -> sample relic -> return to bureau -> repair -> display`

| Step | Pass? | Notes |
| --- | --- | --- |
| Spawn in a sensible place |  |  |
| `WASD` movement responds immediately |  |  |
| Arrow keys also work if expected |  |  |
| Facing sprite changes correctly by direction |  |  |
| Can reach cleanup obstacle without clipping |  |  |
| `E` clears grass or rubble |  |  |
| Hidden relic becomes available after cleanup |  |  |
| `E` samples the relic |  |  |
| Sampled relic visibly follows or transfers state correctly |  |  |
| Workbench accepts repair action |  |  |
| Repaired state is visible |  |  |
| Display case accepts repaired relic |  |  |
| Objective text updates after each step |  |  |
| Detail panel updates after each step |  |  |

## 5. Movement, Boundary, And Black Edge Check

### Boundary Pass

- Push against left map edge for 5 seconds:
- Push against right map edge for 5 seconds:
- Push against top map edge for 5 seconds:
- Push against bottom map edge for 5 seconds:
- Any case where the player exits the intended play area:
- Any case where the camera reveals black space beyond the map:
- Any case where invisible walls feel too far inside the artwork:

### Feel Notes

- Movement feel:
- Camera follow feel:
- Does the world feel like an open map or a boxed proof:

## 6. Occlusion, Sorting, And Readability Check

Test around props, relics, foreground eaves, canopies, awnings, wall edges, workbench, and display case.

| Check | Pass? | Notes |
| --- | --- | --- |
| Player sorts correctly against small props by Y position |  |  |
| Player readability stays clear near foreground occluders |  |  |
| Occlusion hides enough to feel spatial, but not so much that interaction becomes blind |  |  |
| Relics do not disappear behind the wrong layer |  |  |
| Workbench and display case feel grounded in the scene |  |  |
| No obvious sorting flicker or popping |  |  |

## 7. Interaction And Input Check

### Proximity Interaction

- Is the nearest `E` prompt reliable?
- Does the prompt change correctly by context: cleanup / sample / repair / display?
- Do interactions trigger only when they should?
- Is there any case where an old target stays active after state changed?

### Mouse And Debug Selection

- Click-to-select relic works:
- `1-6` selection works:
- Debug selection ever conflicts with normal play:

### Interaction Notes

- Best-feeling interaction:
- Most confusing interaction:
- Any place where the player does not know what to do next:

## 8. UI Check

### Core UI

| Element | Pass? | Notes |
| --- | --- | --- |
| Objective panel is readable |  |  |
| Interaction hint appears only when useful |  |  |
| Detail panel does not cover critical play space |  |  |
| Repair button state is correct |  |  |
| Display button state is correct |  |  |
| Icon, title, era, and state text match the selected relic |  |  |
| No overlapping, clipped, or unreadable text |  |  |
| No mojibake or broken font rendering |  |  |

### UI Feel

- Does the UI support the world, or feel pasted on top of it?
- Is the current panel size appropriate for an open-map game?
- Which panel or prompt feels most in need of redesign:

## 9. Art Placement And Asset Fit Check

This section is for the art agent and Unity implementation agent together. Report what the player actually sees, not just whether the file exists.

### Map And Landmark Fit

| Check | Pass? | Notes |
| --- | --- | --- |
| Background reads as Chang'an / Guanzhong at first glance |  |  |
| Map scale feels coherent with camera size |  |  |
| Landmark silhouettes are readable from play distance |  |  |
| No major empty zones or accidental dead space |  |  |
| No obvious mismatch between background perspective and sprite props |  |  |

### Character Fit

| Check | Pass? | Notes |
| --- | --- | --- |
| Female keeper scale feels natural in the world |  |  |
| Four-direction sprite set feels visually consistent |  |  |
| Feet contact feels correct; no floating |  |  |
| Pivot feels correct during movement and occlusion |  |  |

### Prop And Relic Fit

| Check | Pass? | Notes |
| --- | --- | --- |
| Grass, rubble, relic pile, and relic sprites sit on the ground correctly |  |  |
| Damaged/repaired state swap reads clearly |  |  |
| Workbench and display slots accept assets without looking misaligned |  |  |
| Transparency edges look clean |  |  |
| Style match is coherent across map, props, relics, and UI |  |  |

### Asset Contract Notes

- Suspected pivot issue:
- Suspected scale or PPU issue:
- Suspected crop or padding issue:
- Suspected slot mismatch:
- Any asset that should return to staging before reuse:

## 10. Fun And Design Feedback

Write this like a real player note, not a sterile bug list.

### First Impression

- What grabbed attention in the first 30 seconds:
- What felt cheap, placeholder-like, or immersion-breaking:

### Strongest Moment

- The one moment that felt most like the intended game:

### Weakest Moment

- The one moment that made the loop feel flat or repetitive:

### Clarity

- Did I understand my goal?
- Did I understand why I was repairing this object?
- Did the game communicate consequence, value, or meaning?

### Management Pressure

- Did I feel any time, money, reputation, material, or choice pressure?
- If not, where should pressure enter first:

### Story And NPC Pull

- Did I want to meet anyone, help anyone, or learn anything about the object?
- Which future NPC or dialogue beat feels most necessary:

### Design Return Notes

- Suggested new choice:
- Suggested stronger reward:
- Suggested better narrative hook:
- Suggested cut, merge, or simplification:

## 11. v0.5 Day Slice Check

Use this section once the narrative commission loop exists.

Target chain:

`talk -> investigate -> collect evidence -> restoration choice -> return or exhibit -> reward -> day summary`

| Step | Pass? | Notes |
| --- | --- | --- |
| Talk to Han Niangzi |  |  |
| Ask at least one optional question |  |  |
| Identify dust, crack, and missing corner |  |  |
| Optional consult with Stele Rubbing Du works |  |  |
| Time or work-hour cost is readable |  |  |
| Restoration choice changes outcome |  |  |
| Closing response reflects the choice |  |  |
| Day summary appears and feels complete |  |  |

## 12. Issue Reproduction Cards

Duplicate this block once per issue.

### Issue [ID]

- Title:
- Severity: `P0` / `P1` / `P2` / `P3`
- Area: launch / movement / camera / occlusion / interaction / UI / art / design / narrative / economy
- Owner: Unity / Art / Design / Shared
- Build or scene:
- Frequency: always / often / sometimes / once
- Preconditions:

#### Repro Steps

1. 
2. 
3. 

#### Expected

-

#### Actual

-

#### Evidence

- Screenshot:
- Clip:
- Log line:

#### Return Note

- Why this should go to that owner:
- What the next agent should verify after fixing it:

## 13. Ownership Summary

### Return To Unity

Put issues here when the fix is mainly code, scene generation, runtime hookup, collision, camera, state flow, build, or crash related.

- 

### Return To Art

Put issues here when the fix is mainly sprite generation, crop, alpha cleanup, size, pivot source, padding, style mismatch, slot fit, or visual readability.

- 

### Return To Design

Put issues here when the fix is mainly goal clarity, pacing, reward, story, NPC use, economy pressure, or decision quality.

- 

### Shared Follow-Up

Use when the problem crosses boundaries and must be handed off in order.

- Example format: `Design clarifies target -> Art updates asset prompt/spec -> Unity re-imports and validates slot fit`
- 

## 14. Final Tester Verdict

### What Is Ready

- 

### What Is Not Ready

- 

### Single Highest-Impact Next Step

- 

### Release Call

- `PASS`
- `RETURN_TO_UNITY`
- `RETURN_TO_ART`
- `RETURN_TO_DESIGN`
