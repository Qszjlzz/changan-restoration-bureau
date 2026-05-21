# Art Bible: 《长安修物局》Proof Pass

## Purpose

本文件先固定“美术进入 Unity 的约束”，最终风格需要在三张风格板中选择后再锁定。所有资产先服务 proof scene：比例、pivot、UI 插槽、sorting layer 和交互反馈必须先稳定。

## Shared Rules

- Camera: 2D orthographic, top-down / slightly raised room view.
- Perspective: no strong horizon, no dramatic 3D perspective.
- Lighting: warm indoor lantern light, soft shadow, readable object silhouettes.
- Background: 1920x1080, single shop interior, no baked UI.
- Interactable sprites: transparent PNG, centered with clean margins.
- UI: paper, wood, ink, seal, or museum label motifs; no modern glassmorphism.
- Readability: important objects must remain legible at 128 px and 256 px.
- Palette anchors: clay red, warm wood, aged paper, ink black, bronze green, muted gold.

## Candidate Style A: Gentle Pixel

Use this if the goal is the safest Unity implementation and the closest cozy-game feel.

- Pixel-art inspired, but soft and not low-resolution harsh.
- Clear 1-2 px outline at target scale.
- Simple shape language and warm palette.
- Best for fast iteration and consistent GPT image generation.

## Candidate Style B: Hand-Painted Guanzhong

Use this if cultural atmosphere matters more than strict sprite uniformity.

- Hand-painted 2D props with visible brush texture.
- Warm paper texture and softened edges.
- Slightly richer object surface detail.
- Needs stricter prompt control and more rejection passes.

## Candidate Style C: Pixel Ink

Use this if the project wants a distinctive portfolio identity.

- Pixel silhouettes with ink-wash interior shading.
- Limited palette, black ink edges, red seal accents.
- UI feels like catalog cards, rubbings, and stamped archival labels.
- Highest risk for inconsistent asset generation.

## Unity Import Defaults

- Texture Type: Sprite (2D and UI)
- Sprite Mode: Single
- Pixels Per Unit: 100
- Mesh Type: Full Rect
- Filter Mode: Point for Gentle Pixel; Bilinear for hand-painted variants
- Compression: None for proof
- Pivot:
  - Background: center
  - Interactable props: bottom center
  - UI icons: center
  - Characters: bottom center

## Open Map Acceptance

- The player can walk across a map larger than one screen.
- Buildings, stalls, relic piles, and signage must read at camera scale.
- Interactables should sit naturally on the ground or furniture, not float.
- All 6 artifacts must fit their map pickup slots and display slots without manual per-object scaling.
- UI must remain aligned when icons are replaced.
- Display case slots must accept repaired artifacts without overlap.

## Map Asset Rules

- Large map background: 3072x1792 or tile-composed equivalent.
- Map props: 512x512 transparent PNG, bottom-center pivot.
- Landmarks: 1024x1024 transparent PNG, bottom-center pivot.
- Pickup artifacts: 256x256 transparent PNG, center pivot.
- Camera target view: orthographic size 4.4, player always readable.
