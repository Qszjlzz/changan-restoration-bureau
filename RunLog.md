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
