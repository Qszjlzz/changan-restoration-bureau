# Design Roadmap

## Current Diagnosis

The prototype proves movement, placement, repair, and display, but the actions do not yet create meaningful decisions, relationships, or management pressure. The official proof build also has a P0 scene-load crash, while the visual preview does not contain the complete gameplay loop.

## v0.5: One Complete Day

Player-facing goal: finish one commission from conversation to exhibition.

Content slice: `The Lotus Roof Tile Of The Night Market`.

- Add commissioner `Han Niangzi`, a night-market stall owner asking for help with a reused lotus roof tile from her family's old shopfront.
- Add guide NPC `Apprentice Dou` for tutorial hints, material reminders, and end-of-day upgrade prompts.
- Add consultant NPC `Stele Rubbing Du` near the rubbing yard; consulting him costs one work hour but reveals that the tile was reused after an old market fire.
- Give the player one artifact investigation objective: identify dust, crack, and missing corner.
- Add one restoration decision: fast cosmetic repair for reuse, or careful conservation for exhibition.
- Reward money, neighborhood trust, and scholarly reputation differently.
- End the day with an exhibition or return response and a clear summary.
- Replace the preview/full-build split with one stable executable.

Minimum numbers:

- Start with 12 coins, 5 work hours, 2 paste, and 1 stone powder.
- Each restoration operation costs 1 work hour.
- Joining uses 1 paste. Filling uses 1 stone powder.
- Consulting Du costs 1 work hour and unlocks the best explanation.
- Return-for-reuse result: +12 coins, +2 neighborhood trust.
- Careful-exhibition result: +8 coins, +2 scholarly reputation, +2 next-day visitor income.
- First upgrade `Clear Window Lamp` costs 10 coins and grants +1 work hour on later days.

Acceptance:

- Fresh launch -> talk -> investigate -> collect -> repair choice -> exhibit -> reward -> day summary.
- The build launches twice with no P0/P1 issue.
- The player understands why the two repair choices differ.
- The player makes at least two choices that affect time, money, reputation, or dialogue.
- No flow relies on the visual preview build as a substitute for the official gameplay build.

## v0.6: Workshop Management

- Three recurring NPCs and rotating commissions.
- Money, reputation, material stock, and limited workbench capacity.
- Two artifact families with distinct restoration interactions.
- Commission board and daily planning.

## v0.7: Cultural Memory

- Artifact collection book with provenance, uncertainty, and neighborhood links.
- Visitor reactions that change with display choices.
- Workshop upgrades and apprentice abilities.
- Save/load and three-day progression.

## v0.8: Vertical Slice

- Final onboarding and UI.
- Consistent NPC, prop, artifact, and dialogue portrait art.
- Audio feedback and ambient Chang'an market soundscape.
- Balanced 10-15 minute slice with a reason to replay a second day.
