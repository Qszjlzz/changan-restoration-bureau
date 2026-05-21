# Changan Restoration Bureau Proof Project

This project validates whether AI-generated 2D art can be placed cleanly into an open Unity 2D map through fixed specs, slots, pivots, and UI containers.

## What Exists

- `docs/GameBrief.md`: proof-focused game brief.
- `docs/ArtBible.md`: shared art rules and three candidate styles.
- `docs/AssetCatalog.md`: dedicated art-agent-owned asset breakdown.
- `docs/AssetSpecCards.md`: size, pivot, collider, and Unity import rules.
- `assets/AssetManifest.json`: slot and first artifact manifest.
- `assets/generated/styleboards/*.svg`: local same-layout styleboard previews, including an open-map layout.
- `unity/ChanganRestorationBureau`: Unity 2D proof project.

## Unity Proof Flow

Open `unity/ChanganRestorationBureau` in Unity 2022.3+.

From the Unity menu, run:

```text
Changan > Build Asset Proof Scene
```

This generates:

- placeholder PNG sprites under `Assets/Art/Generated/Placeholders`
- `Assets/Scenes/AssetProofScene.unity`
- an open Chang'an district map with landmarks
- fixed workbench, display, and map pickup slots
- 6 selectable artifacts distributed around the map
- a UI detail panel with repair/display actions

Or run:

```powershell
.\tools-run-unity-proof.ps1
```

That also builds:

```text
Build/ChanganRestorationBureau/ChanganRestorationBureau.exe
```

## Controls

- `WASD`: move the shopkeeper
- `E`: interact with nearby grass, relics, workbench, and display case
- mouse click: select an artifact
- `1` to `6`: select artifacts by index
- `修复`: repair selected artifact
- `陈列`: place repaired artifact into the next display slot

## Git Workflow

See `docs/GitWorkflow.md` for the branch, commit, asset tracking, and milestone rules used by this prototype. The short version:

- keep `main` playable;
- use focused branches for art, gameplay, and UI experiments;
- track Unity scenes, scripts, metadata, prompts, specs, and production prototype art;
- ignore Unity cache folders and local build output;
- use Git LFS for larger future image/audio/model batches.

## Validation Target

The proof succeeds when replacement art can use the same filenames/specs and still fits:

- open-map player scale and camera follow
- workbench slot
- display case slots
- distributed pickup locations
- artifact colliders
- detail panel icon slot
- repair/display state UI
