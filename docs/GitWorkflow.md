# Git Workflow for Changan Restoration Bureau

Git is used as the project's iteration safety net. It will not improve the game by itself, but it makes art replacement, Unity scene changes, and playable-loop experiments reversible and comparable.

## Branches

- `main`: stable playable proof only.
- `codex/art-v0-4`: formal relic art, grass, stones, foreground layers.
- `codex/gameplay-loop`: economy, commissions, rewards, and shop upgrades.
- `codex/ui-polish`: detail panel, prompts, inventory, and display UI.

Create one branch per experiment so weak ideas can be discarded without damaging the playable version.

## Commit Format

Use short conventional commits:

```text
feat: add proximity restoration loop
fix: clamp camera inside open map
art: replace keeper four-direction sprites
docs: update asset spec cards
chore: tune unity git ignore rules
```

## What Goes Into Git

Track:

- Unity project settings, scenes, scripts, metadata, and editor builders.
- Generated production prototype art needed to recreate the scene.
- Source prompts, asset manifests, art bible, spec cards, and run logs.
- Small helper scripts that rebuild or validate the proof.

Ignore:

- Unity `Library/`, `Temp/`, `Obj/`, `Logs/`, and local user settings.
- Windows build output under `Build/`.
- Machine-local logs and IDE cache files.

## Art Asset Rule

Generated image files are intentionally tracked for this prototype because they define the current look. For larger future art batches, use Git LFS so GitHub stores images efficiently.

Recommended LFS file types are already listed in `.gitattributes`: PNG, JPG, PSD, Aseprite, audio, video, FBX, and Blender files.

Do not push an art pass until its GPT Image outputs have been copied into staging, processed into production PNGs, imported into Unity, and validated in the generated proof scene. Asset replacement should preserve existing scene positions, pivots, slots, scales, and collider rules; visual mismatch should be fixed through prompt/cleanup/import adjustments before moving objects.

## Release Rhythm

Use small proof milestones:

- `v0.3`: first playable restoration loop.
- `v0.4`: proper relic art, grass/stone assets, and foreground occlusion.
- `v0.5`: NPC commissions and basic shop economy.
- `v0.6`: save/load, polish pass, and packaged demo.

Each milestone should end with:

- a committed Unity scene;
- a RunLog entry;
- a screenshot or short playtest note;
- a Windows build if Unity licensing/build tooling is working locally.
