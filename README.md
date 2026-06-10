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
Changan > Build Playable Proof Scene
```

This generates:

- placeholder PNG sprites under `Assets/Art/Generated/Placeholders` when production art is missing
- `Assets/Scenes/PlayableProofScene.unity`
- an open Chang'an district map with landmarks
- fixed workbench, display, and map pickup slots
- 6 selectable artifacts distributed around the map
- a UI detail panel with repair/display actions
- a restoration choice panel for quick reuse vs careful exhibit

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
- `1` / `2`: choose quick reuse or careful exhibit when the restoration panel is open
- mouse click: select an artifact
- `1` to `6`: select artifacts by index
- `Repair`: open the restoration choice for the selected artifact
- `Display`: place a conserved artifact into the next display slot

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

## Multi-Agent Workflow

The project now uses a dedicated production loop:

- `docs/AgentWorkflow.md`: role handoffs and release gates.
- `docs/AgentTaskCards.md`: task format for design, art, Unity, and player-test agents.
- `docs/AgentLoopBoard.md`: current approved slice, ownership, and next queue.
- `docs/PlayerTestScript.md`: repeatable smoke and gameplay test scripts.
- `tools/validate_asset_contract.py`: production-art and Unity-copy contract checker.

Current product target: keep the official gameplay executable stable while turning `v0.5` into one complete day around `The Lotus Roof Tile Of The Night Market`.
