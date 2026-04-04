# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

Unity 2D Pong game ("Unity Pong" by CatalystApps) — a fully programmatic game that requires no manual scene setup. All GameObjects, physics, UI, and rendering are created at runtime by `GameSetup.cs`. This is an experimental project with plans to evolve into an AR app targeting Viture headsets.

## Getting Started

1. Open `Unity_Play/` folder in Unity Hub
2. Create a new empty scene (`File → New Scene → Empty`)
3. Add an empty GameObject and attach the `GameSetup` script
4. Press Play

No assets, prefabs, or scene wiring needed — everything is code-generated.

## Architecture

The game bootstraps entirely from `GameSetup.Awake()`, which instantiates all other components:

- **GameSetup** → Creates camera, ball, paddles, walls, goals, UI canvas, and GameManager at runtime
- **GameManager** (singleton) → Score state, UI updates, reset logic. Called by GoalZone triggers.
- **BallController** → Physics-based movement, speed ramp on paddle hits, direction from hit position
- **PaddleController** → Dual-mode: keyboard input (W/S) for player, Y-tracking AI for opponent
- **GoalZone** → Trigger colliders behind paddles that call `GameManager.ScorePoint()`

All sprites are generated from a 4x4 white texture at runtime. No external art assets.

## Tags

Two custom tags are required and defined in `TagManager.asset`: `Ball` and `Paddle`. These are used for collision detection in `BallController.OnCollisionEnter2D`.

## Dependencies

Defined in `Packages/manifest.json`: 2D Sprite, TextMeshPro (3.0.6), uGUI, Physics2D, UI, IMGUI.

## Controls

- **W/S** — left paddle (player)
- **Right paddle** — AI-controlled
- **R** — reset scores and ball

## Legal & Research Docs

- `docs/legal/TERMS_OF_USE.md` — AR safety/liability terms (DRAFT, not legally reviewed, targets Viture headset use)
- `docs/research/spotify_integration.md` — Spotify integration research (parked; no native Unity streaming SDK exists)
