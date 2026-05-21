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
