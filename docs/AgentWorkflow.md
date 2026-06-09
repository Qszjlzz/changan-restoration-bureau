# Multi-Agent Production Workflow

## Goal

Grow `Changan Restoration Bureau` from a placement proof into a complete, culturally grounded simulation game through short, testable iterations.

## Roles

| Agent | Owns | Cannot Approve Alone |
| --- | --- | --- |
| Game Design | loop, NPCs, dialogue, economy, progression, narrative | implementation quality |
| 2D Art | catalog, prompts, production assets, visual QA | gameplay acceptance |
| Unity Implementation | code, scenes, data, import mapping, builds | milestone release |
| Player Test | player-facing QA and release gate | new feature scope |
| Orchestrator | priority, handoffs, Git, automation, conflict resolution | bypassing failed gates |

## Iteration Contract

1. Player Test identifies the highest-impact problem.
2. Game Design defines one observable improvement and acceptance criteria.
3. Art lists required assets and validates their slot contract.
4. Unity implements the slice without moving existing production assets to compensate for bad crops or pivots.
5. Asset QA runs with `tools/validate_asset_contract.py`.
6. Unity builds the smallest relevant executable.
7. Player Test runs the prescribed scenario.
8. Failed checks return to the owning agent. Passed work is logged, committed, and pushed.

## Definition Of Done

- Production assets exist in staging, production, and Unity Production as required.
- Asset ids, dimensions, pivot, PPU, scale, slot, collider, and scene positions satisfy the contract.
- Unity compiles and the executable launches twice.
- The target player flow completes from a fresh start.
- No P0 or P1 test issue remains.
- `RunLog.md` includes evidence and the next highest-impact problem.

## Product Direction

The target is a 10-15 minute vertical slice built around one in-game day:

`morning commission -> field investigation -> restoration choice -> appraisal dialogue -> exhibition -> reputation and workshop upgrade`

The first content arc should use three recurring NPCs:

- `Archivist Shen`: teaches evidence, provenance, and cautious interpretation.
- `Market Runner A-Yue`: brings neighborhood stories and time-sensitive commissions.
- `Apprentice Dou`: assists at the workshop and turns tutorials into character growth.

Each artifact should connect to a person, place, and interpretive choice. Restoration must occasionally ask the player to choose between speed, visual completeness, and historical integrity.

## Release Gates

- `v0.5`: stable executable, one NPC commission, one dialogue, one complete restoration/exhibition day.
- `v0.6`: three NPCs, commission board, money/reputation, two restoration choices.
- `v0.7`: workshop upgrades, visitor reactions, artifact collection book, save/load.
- `v0.8`: polished 10-15 minute vertical slice with onboarding, audio, final art, and repeatable day loop.

## Per-Iteration Report

Every automation run must end with this structure:

```text
Player Test Problem:
Design Decision:
Art Requirements:
Unity Change:
Asset QA:
Build/Launch QA:
Commit/Push:
Next Return Target:
```

## Current Priority

The next accepted change must address the `AssetProofScene` P0 runtime crash or replace the preview/proof split with a single stable playable executable. New NPCs, economy, and dialogue should not be treated as shippable until the official gameplay build launches.
