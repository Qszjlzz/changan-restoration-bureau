# 2D Art Asset Agent: 《长安修物局》

## Role

Own the complete 2D art pipeline before Unity placement. Do not hand random images to Unity. Produce categorized assets, spec cards, prompts, staging folders, and a manifest that the Unity agent can consume.

## Required Order

1. Update `docs/AssetCatalog.md` before generating or replacing art.
2. Confirm every asset has a category, scene use, size, pivot, slot, and state.
3. Write or update prompts under `assets/source-prompts/`.
4. Generate or stage assets into `assets/generated/staging/` first.
5. Promote only assets that satisfy `docs/AssetSpecCards.md` and `docs/UnitySlotContract.md`.
6. Tell the Unity agent which manifest entries changed.

## Categories

- `backgrounds`: open map backgrounds, interior/shop closeups, path/ground variants.
- `landmarks`: large map buildings and cultural locations.
- `characters`: player, visitors, future NPCs.
- `interactables`: artifacts, grass/brush, relic piles, workstations, display cases.
- `ui`: panel frames, icons, buttons, meters, inventory slots.
- `feedback`: selection rings, repair sparkle, pickup marker, display marker.

## Hard Rules

- Every map prop and character uses bottom-center pivot unless the spec card says otherwise.
- Every icon and artifact pickup uses center pivot.
- Transparent sprites keep clear padding and no baked UI text.
- Generated art must be judged in Unity, not only as a standalone image.
- If a sprite does not fit, update the spec/prompt and regenerate; do not ask the Unity agent to manually stretch one asset.
