# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

Unity 2D Pong game ("Pong Me" by Any Stupid Idea / CatalystApps) — a fully programmatic game that requires no manual scene setup. All GameObjects, physics, UI, audio, and rendering are created at runtime by `GameSetup.cs`. This is an experimental project with plans to evolve into an AR app.

## Getting Started

1. Open the `Pong_Me/` project folder in Unity Hub (requires Unity 6 / 6000.4.x)
2. Import TMP Essentials if prompted
3. Open `Assets/Scenes/Pong.unity`
4. Press Play

## Build

- **Editor:** Build > Build Mac / Build > Build Windows (provided by `Assets/Scripts/Editor/BuildScript.cs`)
- **CLI:** Close Unity first, then run the batch build command (see README.md). Methods: `BuildScript.BuildMac`, `BuildScript.BuildWindows`, `BuildScript.BuildIOS`.
- **Output:** `Build/Mac/PongMe.app` or `Build/Windows/PongMe.exe` (Windows requires the entire `Build/Windows/` folder to be distributed together)
- **Windows requires:** Windows Build Support (Mono) module installed via Unity Hub. IL2CPP isn't available when building from a Mac host.

## Architecture

The game bootstraps entirely from `GameSetup.Awake()`, which instantiates all components:

- **GameSetup** → Creates camera, ball, paddles, walls, border lines, goals, UI (canvas, scores, settings panel, help panel, win text, **start overlay**), EventSystem, SoundManager, and GameManager. The Start overlay is built last so it renders above all other UI.
- **GameManager** (singleton) → Score state, win detection (first to 3), speed control (applies immediately via `BallController.ApplySpeedChange`), sound toggle, and the start gate. Tracks `gameStarted`; `StartGame()` is the only path that triggers the very first ball launch.
- **BallController** → Physics-based movement with Rigidbody2D, speed ramp on paddle hits, direction from hit position. Does NOT auto-launch on `Start()` — waits for `GameManager.StartGame()` to call `LaunchBall()`. Subsequent rounds re-launch automatically via `ResetBall()`.
- **PaddleController** → Dual-mode: keyboard input (W/S) for player, Y-tracking AI for opponent
- **GoalZone** → Trigger colliders behind paddles that call `GameManager.ScorePoint()` (no-op until `gameStarted`)
- **SoundManager** (singleton) → Procedural audio generation (square/sine waves), mute support. Exposes `PrimeAudio()` which `StartGame()` calls inside the user-gesture click handler — required for WebGL to unlock the AudioContext, harmless on other platforms.

## Start Overlay

On every platform, the game shows a full-screen "PRESS START" / "TAP TO START" overlay before the first ball launch. Tapping/clicking it (or pressing Space/Enter on desktop) calls `GameManager.StartGame()`, which hides the overlay, primes audio, and launches the ball. After "Play Again" the overlay does **not** re-show — the user has already proven they're here. The overlay's purposes:

1. **WebGL audio gate** — browsers require audio to start from a user gesture
2. **WebGL canvas focus** — clicking the canvas grabs keyboard focus so W/S work
3. **First-time player onboarding** — the subtitle (`W / S TO MOVE • FIRST TO 3 WINS`) gives newcomers the controls and the win condition before action begins

All sprites are generated from a 4x4 white texture at runtime. The gear icon is a procedurally drawn 64x64 texture. Audio clips are synthesized at 44100Hz.

## UI Pattern

Panels (Settings, Help) use top-anchored `RectTransform` layout with `RectMask2D` clipping. Buttons require `EventSystem` + `StandaloneInputModule` (created by GameSetup). Panel backgrounds use `CreateOpaqueSprite()` with `TextureFormat.RGB24` (no alpha channel) to ensure full opacity.

## Tags

Two custom tags defined in `TagManager.asset`: `Ball` and `Paddle`. Used for collision detection in `BallController.OnCollisionEnter2D`.

## Dependencies

`Packages/manifest.json`: 2D Sprite, TextMeshPro 3.0.6, uGUI, Physics2D, UI, IMGUI, Audio.

## Controls

- **W/S** — move left paddle
- **1/2/3** — ball speed (Slow/Medium/Fast)
- **M** — toggle sound
- **R** — reset/new game
- **Mouse** — click gear (settings) and ? (help) icons

## Key Settings (ProjectSettings.asset)

- Window: 1920x1080, windowed mode, resizable
- `defaultIsNativeResolution: 0` — uses specified dimensions
- `fullscreenMode: 3` — windowed

## Docs

- `docs/legal/TERMS_OF_USE.md` — AR safety/liability terms (DRAFT)
- `docs/research/ar_glasses_gaming_analysis.md` — AR headset comparison for gaming
- `docs/research/spotify_integration.md` — Spotify integration research (parked)
- `docs/database/schema.sql` + `seed_data.sql` — Marketplace DDL and test data
