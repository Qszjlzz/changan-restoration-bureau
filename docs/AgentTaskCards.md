# Agent Task Cards

## Game Design Agent

Purpose: make the game deeper, clearer, and more culturally specific.

Per run:

- Read the latest Player Test problem first.
- Choose one player-visible improvement.
- Define the player action, state change, feedback, reward, failure/reset, and acceptance test.
- Keep cultural detail functional: it must change a choice, result, NPC response, or clue.
- Reject pure content expansion when the current loop is unstable or shallow.

Return format:

```text
Problem:
Design Slice:
Player Choice:
State/Reward:
Files Expected:
Acceptance:
```

## 2D Art Agent

Purpose: make generated art attractive, consistent, and correctly placed in Unity.

Per run:

- Update or audit `docs/AssetCatalog.md`, `docs/AssetSpecCards.md`, and source prompts.
- Use built-in GPT Image for new target art unless explicitly blocked.
- Move raw outputs into `assets/generated/staging`.
- Promote only through cleanup/slicing into `assets/generated/production`.
- Copy production PNGs into Unity `Assets/Art/Generated/Production`.
- Run asset QA and report warnings separately from blocking errors.

Return format:

```text
Assets Needed:
Generated/Staged:
Promoted:
Unity Copies:
Placement Invariants:
QA Result:
```

## Unity Implementation Agent

Purpose: turn the approved design and art into the playable Unity build.

Per run:

- Keep scene position, slot id, pivot, PPU, scale, sorting, and collider stable unless the spec changes.
- Implement data-driven NPCs, dialogue, commissions, and economy where practical.
- Keep UI from blocking the main movement area.
- Build the official gameplay executable, not only a visual preview.

Return format:

```text
Implemented:
Build Path:
Controls:
Scenario:
Known Limits:
```

## Player Test Agent

Purpose: act as a real player and gate the milestone.

Per run:

- Launch the executable twice.
- Complete the target flow from a fresh start.
- Test boundary, camera, collision, occlusion, UI, dialogue, state feedback, and rewards.
- Judge whether the loop is understandable and interesting.
- Return P0/P1 failures immediately to the owning agent.

Return format:

```text
Launch:
Scenario Result:
P0/P1:
P2:
Fun Moment:
Confusing Moment:
Return Target:
```

