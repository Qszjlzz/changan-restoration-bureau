# Art Gap Audit v0.5

## Scope

This audit was prepared from:

- `docs/ArtBible.md`
- `docs/AssetCatalog.md`
- `docs/AssetSpecCards.md`
- `docs/UnitySlotContract.md`
- `assets/AssetManifest.json`
- `RunLog.md`
- the current Unity open-map proof builders and runtime bootstrap

Audit tag meanings:

- `missing`: no production-ready asset exists yet
- `placeholder`: Unity is still drawing a generated fallback or temporary shape
- `needs_redo`: an asset exists, but the asset contract, slot mapping, dimensions, or proof integration is not stable enough for v0.5 lock

## Cross-Cutting Pipeline Risks

1. `assets/AssetManifest.json` is stale for the open-map proof. It still describes the old proof slot layout and only lists artifacts, while the current playable proof uses different workbench/display positions and direct world pickup points.
2. `background_open_map.png` is currently `3072x1728`, while the written spec and validator expect `3072x1792`. This can affect framing and camera clamp confidence.
3. `AssetSpecCards.md` does not yet define spec cards for `map_prop_small`, `map_prop_medium`, `foreground_occluder`, `ui_panel`, `ui_button`, or `feedback_sprite`, even though those IDs are already referenced by the catalog.
4. The current official playable proof builder uses the open-map background, player, props, artifacts, foregrounds, and UI basics, but it does not currently place the four landmark sprites. Landmark art exists, but landmark placement is not part of the stable official proof path right now.
5. `furniture_workbench` and `furniture_display_case` are still placeholders generated inside Unity, so one of the most important placement checks is not yet using real art.
6. NPC bodies, dialogue portraits, dialogue-specific UI, and the new v0.5 lotus tile hero artifact states are not in production yet.

## Backgrounds (背景)

| Asset ID | Use | Suggested size / alpha | Pivot | PPU | Scene position or slot | Current status | Main risk |
| --- | --- | --- | --- | --- | --- | --- | --- |
| `background_open_map` | Main open Chang'an district map for exploration and all landmark anchoring | `3072x1792`, PNG, no transparency | `center` | `100` | world origin `(0, 0)`, Background layer, current proof scale `0.78` | `needs_redo` | Production image exists, but actual height is `1728`; current official playable proof also omits landmark placement, so final world readability is not fully validated. |
| `ground_path_cross` | Path readability and player lane guidance | tile or prop overlay, transparent PNG | `center` | `100` | path crossroads around bureau / market routes | `missing` | Without dedicated path overlays, the open map relies too heavily on the single painted background for navigation clarity. |
| `ground_market_patch` | Night-market zone marker and visual identity for the commission area | tile or prop overlay, transparent PNG | `center` | `100` | around `landmark_market` zone | `missing` | The market commission area may not read as a distinct functional space once dialogue and NPCs are added. |

## Landmarks (地标)

| Asset ID | Use | Suggested size / alpha | Pivot | PPU | Scene position or slot | Current status | Main risk |
| --- | --- | --- | --- | --- | --- | --- | --- |
| `landmark_bureau` | Main repair bureau hub and future bureau NPC anchor | `1024x1024`, transparent PNG | `bottom_center` | `100` | intended open-map position `(-3.25, 1.60)` from the earlier open-map builder | `needs_redo` | Production art exists, but the current official playable proof path does not place it, so entrance readability, occlusion, and bureau-adjacent NPC placement are still unverified. |
| `landmark_stele_yard` | Stele rubbing yard landmark and consultant NPC anchor | `1024x1024`, transparent PNG | `bottom_center` | `100` | intended open-map position `(4.35, 2.35)` | `needs_redo` | Same integration gap as above; without a stable placed landmark, `npc_stele_du_idle` will not have a verified home point. |
| `landmark_market` | Night-market commission landmark and Han Niangzi anchor | `1024x1024`, transparent PNG | `bottom_center` | `100` | intended open-map position `(-3.50, -2.35)` | `needs_redo` | This is the most important v0.5 story anchor, but it is not in the current official playable proof scene path. |
| `landmark_relic_yard` | Relic sorting yard landmark and field pickup zone anchor | `1024x1024`, transparent PNG | `bottom_center` | `100` | intended open-map position `(4.35, -1.80)` | `needs_redo` | Artifact pickups exist in the map, but the zone identity is weaker without the landmark in the stable proof path. |

## Characters (角色)

| Asset ID | Use | Suggested size / alpha | Pivot | PPU | Scene position or slot | Current status | Main risk |
| --- | --- | --- | --- | --- | --- | --- | --- |
| `character_keeper_down` | Player facing down | `512x512`, transparent PNG | `bottom_center` | `100` | player spawn `(0.0, -0.3)`, runtime movement sprite swap | `needs_redo` | Production sprite exists and is wired, but the official gameplay executable is not yet stable enough for final on-device placement sign-off. |
| `character_keeper_up` | Player facing up | `512x512`, transparent PNG | `bottom_center` | `100` | runtime directional swap | `needs_redo` | Same validation blocker as above. |
| `character_keeper_left` | Player facing left | `512x512`, transparent PNG | `bottom_center` | `100` | runtime directional swap | `needs_redo` | Same validation blocker as above. |
| `character_keeper_right` | Player facing right | `512x512`, transparent PNG | `bottom_center` | `100` | runtime directional swap | `needs_redo` | Same validation blocker as above. |
| `character_keeper_idle` | Catalog idle reference, future dialogue or standstill fallback | `512x512`, transparent PNG | `bottom_center` | `100` | not the main runtime sprite in current playable proof | `needs_redo` | The catalog still treats this as the main keeper asset, while the runtime now depends on the 4-direction set. The handoff is slightly out of sync. |

## NPC (NPC)

| Asset ID | Use | Suggested size / alpha | Pivot | PPU | Scene position or slot | Current status | Main risk |
| --- | --- | --- | --- | --- | --- | --- | --- |
| `character_commissioner_idle` | Legacy generic commissioner placeholder from the catalog | `512x512`, transparent PNG | `bottom_center` | `100` | no fixed slot in current proof | `missing` | This ID is too generic for v0.5 and risks diverging from the named story NPC assets below. |
| `npc_han_niangzi_idle` | Night-market commissioner body sprite | `512x512`, transparent PNG | `bottom_center` | `100` | market commission point near `landmark_market` | `missing` | v0.5 cannot deliver its first commission flow without a stable world anchor for Han Niangzi. |
| `npc_apprentice_dou_idle` | Bureau guide body sprite | `512x512`, transparent PNG | `bottom_center` | `100` | bureau entrance / workbench side near `landmark_bureau` | `missing` | The tutorial and end-of-day guidance loop cannot feel grounded without Dou on the map. |
| `npc_stele_du_idle` | Consultant body sprite | `512x512`, transparent PNG | `bottom_center` | `100` | rubbing yard consult point near `landmark_stele_yard` | `missing` | The optional consult choice in v0.5 has no visual anchor yet. |

## Artifacts (文物)

| Asset ID | Use | Suggested size / alpha | Pivot | PPU | Scene position or slot | Current status | Main risk |
| --- | --- | --- | --- | --- | --- | --- | --- |
| `artifact_terracotta_fragment_damaged` | Generic pickup artifact in the current proof | `256x256`, transparent PNG | `center` | `100` | pickup point `(3.9, -1.7)` | `needs_redo` | Production art exists, but `AssetManifest.json` still expects unsuffixed file names and does not reflect the current world pickup contract. |
| `artifact_terracotta_fragment_repaired` | Repaired state for workbench and display | `256x256`, transparent PNG | `center` | `100` | `workbench_slot`, then `display_slot_01..03` | `needs_redo` | Same manifest mismatch risk; repaired/display placement still depends on a stale machine-readable handoff. |
| `artifact_roof_tile_damaged` | Current generic lotus roof tile proof pickup | `256x256`, transparent PNG | `center` | `100` | pickup point `(4.9, -2.0)` | `needs_redo` | Existing proof asset is too small and too generic for the v0.5 hero commission; it should not be reused as the final story tile without a dedicated rework. |
| `artifact_roof_tile_repaired` | Current generic repaired roof tile proof asset | `256x256`, transparent PNG | `center` | `100` | `workbench_slot`, then `display_slot_01..03` | `needs_redo` | Not enough visual contrast for the upcoming choice between quick reuse and careful conservation outcomes. |
| `artifact_bronze_mirror_damaged` | Generic pickup artifact in the current proof | `256x256`, transparent PNG | `center` | `100` | pickup point `(-3.6, -2.35)` | `needs_redo` | Valid as proof filler, but not part of the v0.5 featured commission. Keep only as background content. |
| `artifact_bronze_mirror_repaired` | Generic repaired artifact | `256x256`, transparent PNG | `center` | `100` | `workbench_slot`, then `display_slot_01..03` | `needs_redo` | Same as above. |
| `artifact_rubbing_damaged` | Generic pickup artifact in the current proof | `256x256`, transparent PNG | `center` | `100` | pickup point `(4.2, 2.0)` | `needs_redo` | Usable as zone dressing, but not the hero asset for v0.5. |
| `artifact_rubbing_repaired` | Generic repaired artifact | `256x256`, transparent PNG | `center` | `100` | `workbench_slot`, then `display_slot_01..03` | `needs_redo` | Same as above. |
| `artifact_tangsancai_damaged` | Generic pickup artifact in the current proof | `256x256`, transparent PNG | `center` | `100` | pickup point `(-1.0, 2.2)` | `needs_redo` | Usable as ambient proof content only. |
| `artifact_tangsancai_repaired` | Generic repaired artifact | `256x256`, transparent PNG | `center` | `100` | `workbench_slot`, then `display_slot_01..03` | `needs_redo` | Same as above. |
| `artifact_bamboo_slip_damaged` | Generic pickup artifact in the current proof | `256x256`, transparent PNG | `center` | `100` | pickup point `(0.5, -0.25)` | `needs_redo` | Usable as ambient proof content only. |
| `artifact_bamboo_slip_repaired` | Generic repaired artifact | `256x256`, transparent PNG | `center` | `100` | `workbench_slot`, then `display_slot_01..03` | `needs_redo` | Same as above. |
| `artifact_lotus_tile_damaged` | v0.5 hero commission field pickup and inspection asset | `512x512`, transparent PNG | `center` or `bottom_center`, must be declared once | `100` | market-family commission pickup / inspection flow | `missing` | This is the core artifact for the day loop and needs its own larger, more legible treatment. |
| `artifact_lotus_tile_quickfix` | v0.5 fast reuse outcome | `512x512`, transparent PNG | `center` or `bottom_center`, must match damaged state | `100` | restoration choice result / return flow | `missing` | Must read as visibly practical but less archival than the conserved version. |
| `artifact_lotus_tile_conserved` | v0.5 careful exhibition outcome | `512x512`, transparent PNG | `center` or `bottom_center`, must match damaged state | `100` | restoration choice result / exhibition flow | `missing` | Must read as museum-worthy and materially different from the quickfix state. |

## Interactive Obstacles (交互障碍)

| Asset ID | Use | Suggested size / alpha | Pivot | PPU | Scene position or slot | Current status | Main risk |
| --- | --- | --- | --- | --- | --- | --- | --- |
| `prop_grass_patch` | Clearable grass blocker before discovery | `512x512`, transparent PNG | `bottom_center` | `100` | current proof positions `(1.7, -2.15)` and `(2.35, -2.55)` | `needs_redo` | Production art exists, but `map_prop_small` is not defined in `AssetSpecCards.md`, so the art contract is incomplete. |
| `prop_cleared_grass` | Cleared state after grass interaction | `512x512`, transparent PNG | `bottom_center` | `100` | same positions as `prop_grass_patch` after cleanup | `needs_redo` | Same missing spec-card issue; if crop differs from the uncleared state, the swap can jump visually. |
| `prop_rubble_stones` | Clearable rubble blocker | `512x512`, transparent PNG | `bottom_center` | `100` | current proof position `(3.15, -2.2)` | `needs_redo` | Same contract gap as above; still needs stable gameplay build verification. |
| `prop_relic_pile` | Interactable relic pile / ambient obstruction | `512x512`, transparent PNG | `bottom_center` | `100` | current proof position `(4.35, -2.35)` | `needs_redo` | `map_prop_medium` is not formally defined in the spec cards, so sizing and padding rules are implied rather than locked. |
| `prop_stele_fragment` | Rubbing-yard interaction prop or consult clue | `512x512`, transparent PNG | `bottom_center` | `100` | stele-yard clue point near `landmark_stele_yard` | `missing` | v0.5 wants more readable field investigation and zone identity; this is the most obvious missing prop in that area. |

## Foreground Occlusion (前景遮挡)

| Asset ID | Use | Suggested size / alpha | Pivot | PPU | Scene position or slot | Current status | Main risk |
| --- | --- | --- | --- | --- | --- | --- | --- |
| `foreground_bureau_eave` | Bureau roof-edge occluder | `768x768`, transparent PNG | `bottom_center` | `100` | current proof position `(-4.2, 2.05)` | `needs_redo` | Production art exists, but `foreground_occluder` has no written spec card yet, so crop, padding, and sort expectations are not locked. |
| `foreground_tree_canopy` | Tree crown occluder | `768x768`, transparent PNG | `bottom_center` | `100` | current proof position `(1.15, 1.75)` | `needs_redo` | Same contract gap; current placement is proof-level only. |
| `foreground_market_awning` | Market awning occluder | `768x768`, transparent PNG | `bottom_center` | `100` | current proof position `(-3.75, -1.55)` | `needs_redo` | This is especially important once Han Niangzi is added, because bad padding will make dialogue interactions look blocked or misaligned. |
| `foreground_wall_edge` | Wall corner / doorway edge occluder | `768x768`, transparent PNG | `bottom_center` | `100` | current proof position `(3.65, 0.25)` | `needs_redo` | Same contract gap; should be revalidated once the official build is stable. |

## UI (UI)

| Asset ID | Use | Suggested size / alpha | Pivot | PPU | Scene position or slot | Current status | Main risk |
| --- | --- | --- | --- | --- | --- | --- | --- |
| `ui_detail_panel` | Current artifact detail panel | `512x768`, transparent PNG | `center` | `100` | current runtime anchor near right side `(480, -25)` in overlay canvas | `needs_redo` | Production art exists, but `ui_panel` is not yet a formal spec card and the panel only covers the proof loop, not v0.5 dialogue or day summary. |
| `ui_repair_button` | Current repair action button | `384x128`, transparent PNG | `center` | `100` | inside detail panel at `(-58, -135)` | `needs_redo` | Production art exists, but `ui_button` is not formally spec-locked and the button only supports the current proof flow. |
| `ui_display_button` | Current display action button | `384x128`, transparent PNG | `center` | `100` | inside detail panel at `(58, -135)` | `needs_redo` | Same as above. |
| `ui_coin_icon` | Coin / funding readout | `128x128`, transparent PNG | `center` | `100` | future HUD resource strip | `missing` | v0.5 day summary and reward feedback need currency readability. |
| `ui_grade_icon` | Appraisal grade or quality marker | `128x128`, transparent PNG | `center` | `100` | future detail panel / result panel | `missing` | Useful later, but not first-blocking for v0.5. |
| `ui_workhour_icon` | Work-hour resource icon | `128x128`, transparent PNG | `center` | `100` | future HUD resource strip | `missing` | The player cannot easily parse time pressure without it. |
| `ui_paste_icon` | Paste material icon | `128x128`, transparent PNG | `center` | `100` | future HUD resource strip / restoration choice panel | `missing` | Required for restoration-cost clarity. |
| `ui_stone_powder_icon` | Stone powder material icon | `128x128`, transparent PNG | `center` | `100` | future HUD resource strip / restoration choice panel | `missing` | Same as above. |
| `ui_trust_icon` | Neighborhood trust result icon | `128x128`, transparent PNG | `center` | `100` | day summary / result panel | `missing` | The reuse path reward will feel abstract without a visual marker. |
| `ui_reputation_icon` | Scholarly reputation result icon | `128x128`, transparent PNG | `center` | `100` | day summary / result panel | `missing` | The exhibition path reward will feel abstract without a visual marker. |
| `ui_day_summary_panel` | End-of-day results panel | declared before import, suggested `1024x768`, transparent PNG | `center` | `100` | modal overlay after commission resolution | `missing` | A required v0.5 acceptance UI asset. |
| `ui_restoration_choice_panel` | Quickfix vs conservation choice UI | declared before import, suggested `1024x512`, transparent PNG | `center` | `100` | modal overlay at workbench decision point | `missing` | A required v0.5 acceptance UI asset. |
| `ui_dialogue_panel` | Dialogue panel frame for NPC conversations | declared before import, suggested `1280x320`, transparent PNG | `center` | `100` | bottom dialogue overlay with text and portrait slots | `missing` | Dialogue can be functional without this, but presentation and readability will feel unfinished very quickly. |

## Dialogue Portraits (对话头像)

| Asset ID | Use | Suggested size / alpha | Pivot | PPU | Scene position or slot | Current status | Main risk |
| --- | --- | --- | --- | --- | --- | --- | --- |
| `portrait_han_niangzi` | Dialogue portrait for Han Niangzi | `512x512`, transparent PNG | `center` | `100` | left or right portrait slot inside `ui_dialogue_panel` | `missing` | Without a portrait, the commissioner scene will lose warmth and story specificity. |
| `portrait_apprentice_dou` | Dialogue portrait for Apprentice Dou | `512x512`, transparent PNG | `center` | `100` | left or right portrait slot inside `ui_dialogue_panel` | `missing` | Dou carries onboarding and end-of-day framing; a portrait will help the guidance feel character-led rather than system-only. |
| `portrait_stele_du` | Dialogue portrait for Stele Rubbing Du | `512x512`, transparent PNG | `center` | `100` | left or right portrait slot inside `ui_dialogue_panel` | `missing` | Du's consult choice depends on character flavor; no portrait will flatten that decision. |

## First-Priority Asset Pack For The Next Generation Pass

This is the pack that should be generated first for v0.5. It is small enough to be actionable, and it removes the biggest art-side blockers for the first complete commission loop.

1. `artifact_lotus_tile_damaged`
2. `artifact_lotus_tile_quickfix`
3. `artifact_lotus_tile_conserved`
4. `npc_han_niangzi_idle`
5. `npc_apprentice_dou_idle`
6. `npc_stele_du_idle`
7. `portrait_han_niangzi`
8. `portrait_apprentice_dou`
9. `portrait_stele_du`
10. `furniture_workbench`
11. `furniture_display_case`
12. `ui_dialogue_panel`
13. `ui_restoration_choice_panel`
14. `ui_day_summary_panel`
15. `ui_coin_icon`
16. `ui_workhour_icon`
17. `ui_paste_icon`
18. `ui_stone_powder_icon`
19. `ui_trust_icon`
20. `ui_reputation_icon`
21. `background_open_map` refresh to the written `3072x1792` spec

Why this pack comes first:

- It unlocks the full v0.5 day loop from talk -> investigate -> choose -> resolve -> summary.
- It replaces the last obvious placeholder furniture in the playable proof.
- It fixes the one known background framing warning before more UI and NPC placement are judged against the map.
- It avoids regenerating the four landmarks immediately, because those already exist and should first be reintroduced into the stable official proof path before being repainted.
