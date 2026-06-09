# Player Test Script

## Smoke Test

Run before every commit that changes Unity code, scene generation, or production art.

1. Build the official gameplay executable.
2. Launch it twice from a clean start.
3. Confirm no `level0 corrupted`, crash, hang, missing script, or null-reference spam appears in the player log.
4. Move with `WASD` for 30 seconds.
5. Push against all four map edges and confirm the camera does not show black void.
6. Walk behind at least one roof/eave or tree foreground and confirm occlusion is readable.
7. Open the main interaction UI and confirm it does not cover the player during movement.

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
2. Ask at least one optional question.
3. Inspect the lotus roof tile.
4. Identify dust, crack, and missing corner.
5. Optionally consult `Stele Rubbing Du`.
6. Spend limited work hours on restoration operations.
7. Choose return-for-reuse or careful-exhibition.
8. Read the closing NPC response.
9. Confirm coin, trust, reputation, material, and work-hour changes.
10. Reach the day summary.

## Test Verdict

- `PASS`: no P0/P1, target flow complete, one fun moment and one improvement recorded.
- `RETURN_TO_UNITY`: crash, broken input, broken state, build failure, flow cannot finish.
- `RETURN_TO_ART`: wrong size, missing PNG, bad transparency, bad style match, bad position/pivot.
- `RETURN_TO_DESIGN`: boring repetition, unclear goal, no meaningful choice, weak reward, confusing story.

