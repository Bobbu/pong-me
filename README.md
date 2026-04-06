# Pong Me

A retro-styled Pong game built in Unity 6, with bold green-on-black visuals inspired by classic CRT monitors. Everything is generated programmatically at runtime — no manual scene wiring required.

![Unity Pong Logo](Assets/AppIcon.png)

## Features

- **Classic Pong gameplay** — Player vs AI, first to 3 wins
- **Procedural audio** — Synthesized retro sound effects (wall bounce, paddle hit, score, win fanfare)
- **In-game Settings panel** — Click the gear icon or press 1/2/3 to change ball speed (Slow/Medium/Fast), toggle sound on/off
- **In-game Help panel** — Click the ? icon for controls and instructions
- **Fully programmatic** — All game objects, sprites, physics, UI, and audio created in code at runtime
- **Resizable window** — Launches at 1920x1080, freely resizable

## Quick Start

### Prerequisites
- Unity 6 (6000.4.x LTS) — install via [Unity Hub](https://unity.com/download)
- macOS, Windows, or Linux

### Open & Play
1. Open Unity Hub → **Add** → select the `Pong_Me` folder
2. When prompted, import **TMP Essentials** (Window → TextMeshPro → Import TMP Essential Resources)
3. Open the scene at `Assets/Scenes/Pong.unity`
4. Press **Play**

### Build for Mac
- **From Unity:** Build → Build Mac (menu item provided by `Assets/Scripts/Editor/BuildScript.cs`)
- **From command line:**
  ```bash
  /Applications/Unity/Hub/Editor/6000.4.1f1/Unity.app/Contents/MacOS/Unity \
    -batchmode -nographics \
    -projectPath "$(pwd)" \
    -executeMethod BuildScript.BuildMac \
    -quit
  ```
- Output: `Build/Mac/PongMe.app`

### Build for Windows
Requires the **Windows Build Support (Mono)** module installed via Unity Hub (Installs → ⚙️ → Add modules).
- **From Unity:** Build → Build Windows
- **From command line:**
  ```bash
  /Applications/Unity/Hub/Editor/6000.4.1f1/Unity.app/Contents/MacOS/Unity \
    -batchmode -nographics \
    -projectPath "$(pwd)" \
    -executeMethod BuildScript.BuildWindows \
    -quit
  ```
- Output: `Build/Windows/PongMe.exe` plus `PongMe_Data/`, `UnityPlayer.dll`, and other runtime files. **All files in `Build/Windows/` must be distributed together.**
- Note: builds are unsigned, so Windows SmartScreen will warn users on first launch (More info → Run anyway).
- The user-facing `README.txt` lives at `installer/windows-README.txt` and is automatically copied into the zip by the release workflow. (Unity wipes `Build/Windows/` on every rebuild, so the README is not stored there directly.)

## Controls

| Key | Action |
|-----|--------|
| **W / S** | Move left paddle up/down |
| **1 / 2 / 3** | Ball speed: Slow / Medium / Fast |
| **M** | Toggle sound on/off |
| **R** | Reset / New game |
| **Cmd+Q** | Quit (Mac) |

The right paddle is AI-controlled.

## Architecture

The game bootstraps entirely from `GameSetup.Awake()`:

```
GameSetup (Bootstrap)
├── Camera (orthographic, solid black)
├── Ball → BallController (physics, speed ramp, bounce direction)
├── LeftPaddle → PaddleController (player input: W/S)
├── RightPaddle → PaddleController (AI: tracks ball Y)
├── Walls (top/bottom colliders + visible border lines)
├── GoalZones (triggers behind paddles → GameManager.ScorePoint)
├── Center Line (dashed, dim green)
├── Canvas
│   ├── Scores (top center, above border line)
│   ├── Win Text (center, hidden until game over)
│   ├── Settings Button (gear icon, top-left) → Settings Panel
│   └── Help Button (?, top-right) → Help Panel
├── SoundManager (procedural audio: wall, paddle, score, win)
├── EventSystem (routes mouse clicks to UI buttons)
└── GameManager (singleton: score state, win detection, speed control)
```

All sprites are generated from a 4x4 white texture. The gear icon is a procedurally drawn 64x64 sprite. Audio clips are synthesized square/sine waves.

## Project Structure

```
Pong_Me/
├── Assets/
│   ├── AppIcon.png              — App icon (retro CRT style)
│   ├── Scenes/Pong.unity        — Main scene
│   └── Scripts/
│       ├── GameSetup.cs         — Builds entire game at runtime
│       ├── GameManager.cs       — Score, win detection, speed/sound control
│       ├── BallController.cs    — Ball physics and collision
│       ├── PaddleController.cs  — Player input + AI
│       ├── GoalZone.cs          — Score triggers
│       ├── SoundManager.cs      — Procedural audio generation + playback
│       └── Editor/
│           └── BuildScript.cs   — Build automation (Build > Build Mac)
├── docs/
│   ├── legal/
│   │   └── TERMS_OF_USE.md      — AR safety/liability terms (DRAFT)
│   ├── research/
│   │   ├── ar_glasses_gaming_analysis.md  — AR headset comparison
│   │   ├── spotify_integration.md         — Spotify feasibility (parked)
│   │   └── Claude_Code_VSCode_Setup_Guide.md/pdf
│   └── database/
│       ├── schema.sql           — Product/Customer/Seller DDL
│       └── seed_data.sql        — Test data
├── ProjectSettings/             — Unity project configuration
├── Packages/manifest.json       — Unity package dependencies
├── CLAUDE.md                    — Claude Code guidance
└── README.md                    — This file
```

## Documentation

| Document | Description |
|----------|-------------|
| [Terms of Use](docs/legal/TERMS_OF_USE.md) | AR safety and liability terms (DRAFT — requires legal review) |
| [AR Glasses Analysis](docs/research/ar_glasses_gaming_analysis.md) | Product comparison of AR headsets for gaming (April 2026) |
| [Spotify Research](docs/research/spotify_integration.md) | Spotify integration feasibility (parked) |
| [Claude Code + VS Code Guide](docs/Claude_Code_VSCode_Setup_Guide.md) | Setup guide for new developers |
| [Database Schema](docs/database/schema.sql) | Marketplace DDL (Product, Customer, Seller) |

## License

TBD
