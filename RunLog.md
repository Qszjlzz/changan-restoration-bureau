# Run Log: 《长安修物局》

## 2026-05-19 Proof Setup

- Created proof-focused brief, art bible, asset spec cards, and asset manifest.
- Revised first implementation target from fixed shop interior to open 2D map placement validation after user clarified the Plant Tales reference.
- Unity proof scene will generate placeholder sprites from the manifest-aligned slot model on an open map with camera follow.
- Added open-map styleboard preview for the preferred hand-painted Guanzhong direction.

## Evidence To Capture

- Unity batchmode scene generation log.
- Unity validation log.
- Screenshot of `AssetProofScene` after Play Mode or editor open.
- Notes on any scale, pivot, sorting, or UI mismatch.

## 2026-05-20 Art Replacement v0.2

- Generated first real hand-painted Guanzhong open-map art set with GPT Image.
- Staged raw assets under `assets/generated/staging/`.
- Processed production assets under `assets/generated/production/`.
- Copied Unity-ready production art into `Assets/Art/Generated/Production`.
- Unity builder now prefers production art and falls back to procedural placeholders only when production files are missing.

## 2026-05-20 Playable Loop v0.3

- Added four-direction female keeper sprites and wired them into `ProofPlayer2D`.
- Added player movement bounds, camera clamp values, invisible boundary wall generation, and runtime Y sorting.
- Added proximity interaction targets for cleanup, sampling, repair, and display.
- Added objective and interaction hint UI for the first loop: clean grass, sample relic, repair, display.
- Unity batch build previously completed successfully, but the final rerun was blocked by the local Unity LicensingClient IPC timeout before the regenerated scene could be rebuilt into the executable.

## 2026-05-25 Art Occlusion v0.4 Pass 1

- Generated a hand-painted 4x4 relic/prop sprite sheet for damaged/repaired artifacts, grass, cleared grass, rubble, and old relic pile.
- Added chroma-key removal and slicing to `tools/process_generated_art.py`, promoting outputs to production and Unity Production folders.
- Updated the Unity scene builder so artifacts, grass, rubble, and relic pile prefer production sprites instead of procedural placeholders.
- Added `Prop_RubbleStones` as a second cleanup interaction point and allowed cleanup props to swap to a cleared-state sprite.
- Updated AssetCatalog and source prompts to mark v0.4 relic/prop assets as production-linked.
- Unity batch build succeeded and produced `Build/ChanganRestorationBureau/ChanganRestorationBureau.exe`.
- Remaining polish: inspect in Play Mode for sprite scale/edge fringes, then add true foreground split layers for roofs/walls/tree crowns.

## 2026-06-03 Art Occlusion v0.4 Pass 2

- Generated a 2x2 foreground occlusion sprite sheet for bureau roof eave, tree canopy, market awning, and wall edge.
- Added chroma-key cleanup, slicing, and production promotion for foreground assets.
- Updated `AssetCatalog` and source prompts with `foreground_occlusion_sheet_v0_4`.
- Added four foreground occluders to `AssetProofScene` through the Unity scene builder, all using `YSortRenderer`.
- Unity batch build succeeded and produced `Build/ChanganRestorationBureau/ChanganRestorationBureau.exe`.
- Remaining polish: playtest player movement around each occluder and tune scale/position if any object blocks an interaction point.

## 2026-06-03 Runtime Launch Recovery

- Re-tested `Build/ChanganRestorationBureau/ChanganRestorationBureau.exe`; the player process crashed during scene load with `level0 is corrupted` / `Position out of bounds`.
- Confirmed a minimal Unity smoke build can launch, so the failure is specific to the generated proof scene or its serialized content, not the Windows player environment.
- Added a lightweight `ChanganVisualPreview` build path that loads the current production map, female keeper, relic props, cleanup props, and foreground occluders while bypassing the crashing full proof scene.
- Built and launched `Build/ChanganVisualPreview/ChanganVisualPreview.exe`, then copied and launched it from `C:\ChanganVisualPreview\ChanganVisualPreview.exe` for path-safe viewing.
- Runtime evidence: `ChanganVisualPreview` stayed open with a valid window handle and no repeat of the `level0 corrupted` error in `Player.log`.
- Next fix: isolate which object or UI serialization in `AssetProofScene` corrupts the full playable build, then merge the preview stability back into the official proof executable.

## 2026-06-10 Multi-Agent Workflow Gate

- Added a production workflow that separates Game Design, 2D Art, Unity Implementation, Player Test, and Orchestrator responsibilities.
- Added task cards for each agent and a repeatable player test script covering launch, camera, boundaries, occlusion, UI, current gameplay flow, and the planned v0.5 day loop.
- Added `DesignRoadmap.md` with the next product target: first fix the official gameplay executable, then build v0.5 around `The Lotus Roof Tile Of The Night Market`.
- Added `tools/validate_asset_contract.py` to compare production PNGs against Unity copies, expected dimensions, and PPU metadata.
- Asset QA result: passed with one non-blocking warning. `background_open_map.png` is `3072x1728`, while the spec expects `3072x1792`; this should be resolved by the Art Agent before final v0.5 art lock.
- Player Test gate remains blocked for official gameplay release until the `AssetProofScene` / `level0 corrupted` P0 is fixed.

## 2026-06-10 Official Build Launch Recovery

- Replaced the official proof build path so it now builds `PlayableProofScene.unity` through `ChanganPlayableProofSceneBuilder` and boots runtime UI through `ProofRuntimeBootstrap`.
- Split `InteractionTarget` into its own script file so the generated scene now serializes interaction targets with normal GUID-backed script references instead of local file-only references.
- Rebuilt the official Windows executable and mirrored it to an ASCII-safe temp launch path for verification.
- Latest launch evidence: the executable stayed alive for at least 12 seconds with a real window handle, and `Player.log` advanced into managed runtime logs:
  - `[Changan] ProofRuntimeBootstrap.Awake begin`
  - `[Changan] ProofRuntimeBootstrap built UI`
  - `[Changan] ProofRuntimeBootstrap.Awake complete`
  - `[Changan] ProofObjectiveState.Start target=artifact_terracotta_fragment`
  - `[Changan] AssetProofController.Start artifacts=6 displays=3 workbench=True ui=True`
- The previous `level0 corrupted` / `Position out of bounds` crash did not appear after this fix pass.
- Added multi-agent handoff docs for the next slice:
  - `docs/TaskSlice_v0_5_LotusTile.md`
  - `docs/ArtGapAudit_v0_5.md`
  - `docs/PlayerTestReportTemplate.md`
- Next gate: second clean launch plus smoke-play through `move -> cleanup -> sample -> repair -> display`, then begin landmark reintegration and the first NPC commission slice.

## 2026-06-10 Playable Proof Stabilization Pass 2

- Reintroduced the four open-map landmarks into the stable `PlayableProofScene` path:
  - `landmark_bureau`
  - `landmark_market`
  - `landmark_stele_yard`
  - `landmark_relic_yard`
- Added richer gameplay trace logs for cleanup, selection, repair, display, slot placement, and objective progression to support the Player Test Agent.
- Added `tools/smoke_play_proof.ps1` and the in-game `ProofSmokeAutoplay` helper. The official executable can now run a built-in smoke route with `-smoke-play`, instead of relying on brittle desktop key injection.
- Smoke evidence: the official gameplay executable launched twice and completed the full proof route twice through the automated smoke path with no crash markers and no missing progression markers.
- Added `ProofDayState` plus `Assets/Resources/Data/ProofNarrativeCatalog.json` as the first in-engine data skeleton for the v0.5 commission slice `The Lotus Roof Tile Of The Night Market`.
- Asset QA still passes with the same one non-blocking warning: `background_open_map.png` remains `3072x1728` instead of the written `3072x1792` spec.
- Next gate: replace placeholder workbench/display visuals, add world anchors and first interactions for `Han Niangzi`, `Apprentice Dou`, and `Stele Rubbing Du`, then wire the first commission acceptance and day-phase transitions onto the new `ProofDayState`.

## 2026-06-10 Narrative Skeleton Pass 1

- Added the first runtime narrative layer to the stable playable proof:
  - `ProofDialogueController`
  - `ProofNpcInteractable`
  - expanded `ProofDayState`
- Added three world NPC anchors to the official `PlayableProofScene` path using fallback hand-painted keeper art plus name labels until dedicated NPC sprites are generated:
  - `Han Niangzi` at the night market
  - `Apprentice Dou` near the bureau
  - `Stele Rubbing Du` at the stele yard
- Added dialogue-driven day-phase transitions:
  - Han accepts the lotus roof tile commission
  - Du unlocks the optional consultation state
  - Han resolves the current outcome after display
  - Dou opens the day ledger summary
- Updated the objective system so it now tracks the narrative route instead of only the old proof loop.
- Expanded `ProofSmokeAutoplay` and `tools/smoke_play_proof.ps1` so the official build now auto-validates:
  - take commission
  - cleanup
  - sample relic
  - consult Du
  - repair
  - display
  - resolve with Han
  - read day summary with Dou
- Verification evidence:
  - Unity official Windows build succeeded.
  - `tools/validate_asset_contract.py` passed with the same single warning on `background_open_map.png`.
  - `tools/smoke_play_proof.ps1` passed with two consecutive launches and no missing markers.
- Added narrative handoff docs from the parallel design/test agents:
  - `docs/NpcDialogue_v0_5.md`
  - `docs/NarrativeSmokeChecks.md`
- Next gate: replace placeholder bureau furniture, add dedicated NPC/body/portrait production art, then introduce the first explicit restoration branch choice instead of the current fixed `careful_exhibit` outcome.
## 2026-06-10 Blocked Run
- Orchestrator was blocked before repo inspection because local execution tools failed during sandbox setup refresh.
- `functions.shell_command` returned `windows sandbox: setup refresh failed with status exit code: 1`.
- `mcp__node_repl.js` returned `node_repl kernel exited unexpectedly` with `windows sandbox failed: spawn setup refresh`.
- No safe repo read, build, smoke, asset validation, or player-test actions were executed this run.
- Next priority remains restoring local command execution so the official build and double-launch smoke flow can resume.
## 2026-06-10 Automation note
- This run was blocked before repository inspection because local execution tools failed during Windows sandbox refresh/setup.
- `functions.shell_command` and `mcp__node_repl.js` both failed before command execution, so no build, smoke play, asset validation, or gameplay changes were attempted.
- Resume from the standard workflow only after local command execution is restored.

## 2026-06-10 Restoration Branch Choice Pass

- Added a real restoration-choice layer to the official playable path:
  - `ProofRestorationChoiceController`
  - `AssetProofController` repair-choice handoff
  - `InteractionController` choice-panel gating
  - branch-aware objective, dialogue, and detail-panel states
- Quick reuse and careful exhibit now diverge in player flow:
  - `quick_reuse`: repair -> return directly to Han Niangzi
  - `careful_exhibit`: repair -> display -> resolve with Han Niangzi
- Updated smoke coverage so the official build can validate both restoration branches instead of only the old careful-only route:
  - `tools/smoke_play_proof.ps1 -Branch careful_exhibit`
  - `tools/smoke_play_proof.ps1 -Branch quick_reuse`
  - `ProofSmokeAutoplay` now accepts `-smoke-branch`
- Added and updated workflow docs so the multi-agent loop has a concrete current target:
  - `docs/AgentLoopBoard.md`
  - `docs/AgentWorkflow.md`
  - `docs/DesignRoadmap.md`
  - `docs/PlayerTestScript.md`
  - `docs/NarrativeSmokeChecks.md`
  - `README.md`
- Updated the recurring automation `v0-4` to use the new loop board and to require smoke validation on both branches every run.

Verification:

- Asset QA: `tools/validate_asset_contract.py` passed with the same one non-blocking warning:
  - `background_open_map.png` is still `3072x1728` instead of the written `3072x1792` spec
- Official Windows build: passed through `ChanganProofBuild.BuildWindowsProof`
  - build evidence log: `unity-build-player-v0_5.log`
  - executable refreshed at `Build/ChanganRestorationBureau/ChanganRestorationBureau.exe`
- Smoke QA:
  - `careful_exhibit`: passed two launches
  - `quick_reuse`: initially failed because the test was still launching an old executable; after a real waited rebuild, passed two launches

Player-test/design/art takeaways from the sidecar agents:

- Biggest current design gain: visible branch choice now matters, but the next slice should expose day-budget pressure more clearly in the HUD.
- Biggest current art gap: placeholder workbench/display, fallback NPC bodies/portraits, and the generic roof-tile hero artifact should be the next GPT Image batch.
- Biggest current test upgrade: keep branch coverage split across two fresh runs and avoid relying on debug selection during release-gate passes.

Next return target:

1. visible day-budget HUD and blocked-action feedback
2. production furniture replacement for workbench/display
3. dedicated NPC body + portrait art
4. lotus-tile-specific hero artifact art and asset-id cleanup
