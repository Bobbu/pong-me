# Getting Started with Claude Code + VS Code

## Prerequisites

Before getting started, you will need:

1. **VS Code** version 1.98.0 or higher — download from https://code.visualstudio.com/
2. **Claude subscription** — a Claude Pro, Max, Team, or Enterprise account
3. **Internet connection** — required for authentication and API calls
4. **Git (Windows only)** — if on Windows, install Git for Windows from https://git-scm.com/downloads/win

---

## Step 1: Install the VS Code Extension

1. Open VS Code
2. Press **Cmd+Shift+X** (Mac) or **Ctrl+Shift+X** (Windows/Linux) to open Extensions
3. Search for **"Claude Code"**
4. Click **Install**
5. Restart VS Code if prompted

### Optional: Install the CLI Separately

The extension includes the CLI, but you can also install it standalone for terminal use:

- **macOS (Homebrew):** `brew install --cask claude-code`
- **macOS/Linux (curl):** `curl -fsSL https://claude.ai/install.sh | bash`
- **Windows (WinGet):** `winget install Anthropic.ClaudeCode`

---

## Step 2: Sign In

1. Click the **Spark icon** in the top-right corner of the VS Code editor, or click **"Claude Code"** in the status bar
2. A browser window will open — sign in with your Claude account
3. Grant permission and return to VS Code
4. You're ready to go!

---

## Step 3: Start Using Claude Code

### Open a Conversation

- Click the **Spark icon** in the editor toolbar (top-right corner)
- Or use Command Palette: **Cmd+Shift+P** (Mac) / **Ctrl+Shift+P** (Windows) and search for "Claude Code"

### Ask Claude for Help

- Type a prompt like: *"Explain this function"* or *"Fix the bug in this file"*
- Claude automatically reads selected text in the editor
- Use **@** to mention specific files or folders (e.g., `@src/app.ts#5-10`)

### Review and Accept Changes

- Claude shows proposed changes side-by-side
- Accept, reject, or give feedback
- Changes are applied directly to your files

---

## Key Features

| Feature | How to Use |
|---------|-----------|
| **File context** | Press Option+K (Mac) / Alt+K (Windows) to add file references |
| **Conversation history** | Click the dropdown at top to resume past conversations |
| **Slash commands** | Type `/` in the prompt box to see available commands |
| **Permission modes** | Click the mode button at bottom of prompt box |
| **Multiple sessions** | Use Cmd+Shift+Esc / Ctrl+Shift+Esc for a new tab |

---

## Keyboard Shortcuts

| Action | Mac | Windows/Linux |
|--------|-----|---------------|
| Focus between editor and Claude | Cmd+Esc | Ctrl+Esc |
| Insert file reference | Option+K | Alt+K |
| New conversation tab | Cmd+Shift+Esc | Ctrl+Shift+Esc |
| New conversation (when focused) | Cmd+N | Ctrl+N |

---

## Next Steps

1. **Explore workflows** — ask Claude to explain code, refactor functions, fix bugs, or write tests
2. **Add a CLAUDE.md file** — create a `CLAUDE.md` in your project root with coding standards and instructions for Claude to follow
3. **Connect tools** — use `/mcp` to connect external tools (GitHub, Jira, databases, etc.)

---

## Troubleshooting

- **Extension won't install?** Ensure VS Code version is 1.98.0 or higher
- **Spark icon not showing?** Open a file first, then check the editor toolbar
- **Sign-in not working?** Check your internet connection and restart VS Code
- **Need a guided tour?** Run "Claude Code: Open Walkthrough" from the Command Palette

---

*Guide generated April 2026*
