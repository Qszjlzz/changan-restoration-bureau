# Asset Spec Cards

## Placement Invariant

- Art generation path: use the built-in GPT Image generator first, then copy raw outputs into `assets/generated/staging`.
- Promotion path: staging image -> local cleanup/slicing -> `assets/generated/production` -> Unity `Assets/Art/Generated/Production`.
- Do not push art replacement work until the production PNGs exist and Unity can import them.
- Replacing an asset must keep the original scene position, slot id, pivot, PPU, scale, and collider contract unless a RunLog entry explicitly records why the placement spec changed.
- If a sprite looks wrong in Unity, fix the prompt, cleanup, crop, pivot, or import mapping before moving the individual object by hand.

## background_shop_interior

- Category: background
- Size: 1920x1080
- Format: PNG
- Transparency: no
- Pivot: center
- Unity PPU: 100
- Sorting Layer: Background
- Scene Use: full proof scene backdrop
- Notes: orthographic room, no baked interactive highlights, no UI labels.

## background_open_map

- Category: background
- Size: 3072x1792
- Format: PNG
- Transparency: no
- Pivot: center
- Unity PPU: 100
- Sorting Layer: Background
- Scene Use: open Chang'an district proof map
- Notes: path network, open walking space, shop exterior, market, stele yard, relic sorting area.

## landmark_building

- Category: map landmark
- Size: 1024x1024
- Format: transparent PNG
- Pivot: bottom center
- Unity PPU: 100
- Sorting Layer: Furniture
- Slot Size: 4x3 floor units
- Collider: BoxCollider2D obstacle, optional trigger at entrance
- Scene Use: repair bureau, stele yard, market stall, storage shed.

## furniture_workbench

- Category: interactable furniture
- Size: 512x512
- Format: transparent PNG
- Pivot: bottom center
- Unity PPU: 100
- Sorting Layer: Furniture
- Slot Size: 3x1 floor units
- Collider: BoxCollider2D trigger, 2.8x1.0 units
- Scene Use: repair station.

## furniture_display_case

- Category: interactable furniture
- Size: 512x512
- Format: transparent PNG
- Pivot: bottom center
- Unity PPU: 100
- Sorting Layer: Furniture
- Slot Size: 4x1 floor units
- Collider: BoxCollider2D trigger, 3.8x1.0 units
- Scene Use: repaired item display.

## artifact_small

- Category: artifact
- Size: 256x256
- Format: transparent PNG
- Pivot: center
- Unity PPU: 100
- Sorting Layer: Props
- Slot Size: 1x1 display slot
- Collider: CircleCollider2D trigger, radius 0.45
- Scene Use: selectable object, inventory icon, detail panel icon.

## ui_icon

- Category: UI
- Size: 128x128
- Format: transparent PNG
- Pivot: center
- Unity PPU: 100
- Sorting Layer: UI
- Scene Use: buttons, status markers, inventory.
- Notes: keep 12 px clear padding.

## character_idle

- Category: character
- Size: 512x512
- Format: transparent PNG
- Pivot: bottom center
- Unity PPU: 100
- Sorting Layer: Characters
- Collider: CapsuleCollider2D, 0.7x1.1 units
- Scene Use: proof player or visitor idle.
