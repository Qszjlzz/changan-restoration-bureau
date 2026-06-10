# Player Test Script

## Smoke Test

Run before every commit that changes Unity code, scene generation, or production art.

1. Build the official gameplay executable.
2. Launch it twice from a clean start on `careful_exhibit`.
3. Launch it twice from a clean start on `quick_reuse`.
4. Confirm no `level0 corrupted`, crash, hang, missing script, or null-reference spam appears in the player log.
5. Move with `WASD` for 30 seconds.
6. Push against all four map edges and confirm the camera does not show black void.
7. Walk behind at least one roof/eave or tree foreground and confirm occlusion is readable.
8. Open the main interaction UI and confirm it does not cover the player during movement.
9. Do not use click-to-select or `1-6` during release-gate runs unless reproducing a bug.

Fail on any P0/P1 issue.

## Current Gameplay Flow Test

1. Start at the bureau.
2. Move to a cleanup obstacle.
3. Press `E` to clear it.
4. Pick up a discovered artifact.
5. Return to the workbench.
6. Repair the artifact.
7. Move to the display case.
8. Display the artifact.
9. Confirm the objective text and detail panel update.

## v0.5 Target Flow Test

1. Talk to `Han Niangzi`.
2. Without reopening dialogue, explain what the object is, why it matters, and where to go next.
3. Inspect the lotus roof tile.
4. Identify dust, crack, and missing corner.
5. Optionally consult `Stele Rubbing Du`.
6. Spend limited work hours on restoration operations.
7. At the workbench, predict each branch's cost and reward from the UI alone.
8. Confirm the day-budget HUD shows hours, paste, and stone before choosing.
9. Run one fresh playthrough with `quick_reuse`.
10. Run one fresh playthrough with `careful_exhibit`.
11. Compare the objective copy, Han response, and reward changes between the two runs.
12. Reach the day summary and restate what changed plus what tomorrow is teasing.
13. Record any stretch longer than 20 seconds with no new decision, reveal, or feedback.

## Test Verdict

- `PASS`: no P0/P1, target flow complete, one fun moment and one improvement recorded.
- `RETURN_TO_UNITY`: crash, broken input, broken state, build failure, flow cannot finish.
- `RETURN_TO_ART`: wrong size, missing PNG, bad transparency, bad style match, bad position/pivot.
- `RETURN_TO_DESIGN`: boring repetition, unclear goal, no meaningful choice, weak reward, confusing story.
