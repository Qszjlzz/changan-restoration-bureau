# Unity Slot Contract

This contract is the handoff between the 2D asset agent and the Unity implementation agent.

## Non-Negotiable Rule

Art is generated to fit slots. Unity slots are not hand-adjusted per individual image unless the spec card changes.

## Scene Slots

| Slot | Unity Type | Accepts | Pivot | Purpose |
| --- | --- | --- | --- | --- |
| `workbench_slot` | `WorkbenchSlot` | damaged/repaired artifact | center | temporary repair placement in 修物局 |
| `display_slot_01..03` | `DisplayCaseSlot` | repaired artifact | center | final display proof in 修物局 |
| `pickup_slot_01..06` | `ShelfSlot` | damaged artifact pickup | center | map-distributed relic/sample points |
| `inventory_slot_01` | `InventorySlot` | UI icon | center | later inventory proof |

## Replacement Asset Rules

- Keep the manifest `asset.id` stable.
- Replacement sprite filenames should match `expected_file` or be mapped explicitly in the manifest.
- Do not bake drop shadows into transparent artifact sprites for proof; use Unity selection/highlight instead.
- Keep objects centered inside the canvas with at least 8% transparent padding.
- If one sprite does not fit, update its spec card or prompt; do not scale only that one object in the scene.

## Code Expectations

- Runtime code selects artifacts through Collider2D, not pixel-perfect hit tests.
- UI uses fixed `RectTransform` containers; images may change, panel layout must not.
- Repaired/displayed states are sprite swaps and slot transfers, so final art should provide damaged and repaired variants for each artifact.
- Camera follows the player across an open map; artwork must be judged both near the player and at screen-scale distance.
