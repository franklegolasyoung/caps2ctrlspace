# Caps2CtrlSpace

Map **CapsLock** to **Ctrl+Space** for IME input method switching on Windows.

![Windows 10/11](https://img.shields.io/badge/Windows-10%2F11-blue)
![.NET 8](https://img.shields.io/badge/.NET-8.0-purple)
![License](https://img.shields.io/badge/License-MIT-green)

## Why?

On Windows, switching between Chinese/Japanese/Korean and English input methods typically requires **Ctrl+Space** or **Win+Space**. The CapsLock key sits in a prime keyboard position but is rarely used. This tool remaps CapsLock to Ctrl+Space so you can switch IME with a single key press — also consistent with the default macOS behavior of using CapsLock to toggle input sources.

## Download

Download the latest release from the [Releases](https://github.com/franklegolasyoung/caps2ctrlspace/releases) page.

- **`Caps2CtrlSpace.exe`** — Single-file executable, no installation needed

## Getting Started

1. Download and run `Caps2CtrlSpace.exe`
2. The app window will appear with the current status
3. Press **CapsLock** to switch your input method (sends Ctrl+Space)
4. Close the window — the app minimizes to the system tray
5. Double-click the tray icon to reopen the settings window
6. Right-click the tray icon to Show or Exit

## Features

- **CapsLock to Ctrl+Space** — Low-level keyboard hook intercepts CapsLock globally
- **Modifier passthrough** — CapsLock with Shift/Ctrl/Alt/Win works normally
- **System tray** — Runs quietly in the background, always accessible
- **Auto-start** — Optional "Start with Windows" toggle
- **Single instance** — Prevents duplicate processes
- **Fluent UI** — Windows 11 native look with Mica backdrop

## Settings

| Option | Description |
|--------|-------------|
| **Keyboard Mapping** | Shows the current mapping status (CapsLock -> Ctrl+Space) |
| **Start with Windows** | Toggle auto-launch on login (writes to registry `HKCU\...\Run`) |

## Build from Source

### Requirements

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)

### Build

```bash
dotnet build -c Release
```

### Publish (self-contained single file)

```powershell
.\publish.ps1 -SelfContained
```

Output: `publish/Caps2CtrlSpace.exe`

## License

MIT
