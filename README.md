# 🎯 Crosshair Overlay App - Advanced v2.0

A lightweight, **non-intrusive crosshair overlay** built in **C#**.  
The app renders a customizable crosshair on screen without injecting into any game process — making it **safe** and **anti-cheat–friendly** for general use.

---

## 🚀 Features

### Core Features
- 🧊 Transparent, topmost overlay window  
- 🖱️ Click-through (does not block mouse input)  
- 🎯 **8 customizable crosshair styles** (vector & image-based)
- ⌨️ **Advanced hotkey system** for quick switching
- 🖥️ **Multi-monitor support** with flexible selection
- 🪶 Lightweight (low CPU & GPU usage)

### Advanced Features (v2.0)
- 📁 **Profile System** - Create unlimited profiles for different games
- 🔄 **Multiple Crosshairs** - Switch between crosshairs per profile
- 🖼️ **Image Crosshair Support** - Import PNG/JPG/BMP images
- 📚 **20+ Built-in Templates** - Pre-made crosshairs from popular games
- 🎨 **Transform System** - Offset, scale, and rotate crosshairs
- 🎮 **Game-Specific Profiles** - CS:GO, Valorant, Apex, etc.

---

## ⌨️ Hotkeys

```
Insert           - Toggle crosshair visibility
Ctrl+Q           - Exit application
Ctrl+Left/Right  - Switch profiles
Ctrl+Up/Down     - Switch crosshairs
Ctrl+Shift+C     - Open configuration file
```

---

## 🏗️ Tech Stack

- **C# (.NET 8+)**
- **GameOverlay.Net** — for overlay rendering and transparency
- **Win32 Interop (P/Invoke)** — for hotkey and input handling
- **JSON Configuration** — easy-to-edit settings

---

## 📦 Setup

### Prerequisites
- .NET 8.0 SDK or later
- Windows OS

### Build & Run
```bash
# Restore dependencies
dotnet restore

# Build the project
dotnet build

# Run the application
dotnet run
```

Or simply:
```bash
dotnet run
```

---

## ⌨️ Controls

- **Insert** — Toggle crosshair visibility
- **Ctrl+Q** — Exit application

---

## ⚙️ Customization

### Quick Edit
Press `Ctrl+Shift+C` to open `crosshair-advanced.json` and edit:
- Crosshair colors, sizes, and styles
- Multiple profiles for different games
- Image crosshairs (PNG/JPG/BMP)
- Offset, scale, and rotation
- Monitor selection

### Example Configuration
```json
{
  "CurrentProfileName": "CS:GO",
  "Profiles": [
    {
      "Name": "CS:GO",
      "Crosshairs": [
        {
          "Name": "Rifle",
          "Type": "Vector",
          "Vector": {
            "Style": "Cross",
            "Size": 12,
            "Color": { "R": 0, "G": 255, "B": 0, "A": 255 }
          }
        }
      ]
    }
  ]
}
```

See **[ADVANCED_FEATURES.md](ADVANCED_FEATURES.md)** for complete documentation.

---

## � Documentation

- **[QUICKSTART.md](QUICKSTART.md)** - Get started in 2 minutes
- **[ADVANCED_FEATURES.md](ADVANCED_FEATURES.md)** - Complete v2.0 feature guide
- **[CUSTOMIZATION.md](CUSTOMIZATION.md)** - Detailed customization examples
- **[BUILD.md](BUILD.md)** - Build and deployment instructions
- **[ARCHITECTURE.md](ARCHITECTURE.md)** - Technical architecture
- **[IMPLEMENTATION_SUMMARY.md](IMPLEMENTATION_SUMMARY.md)** - Development summary

---

## 🎮 Built-in Templates

Pre-configured crosshairs from popular games:
- **CS:GO** - Classic, Dot, T-Style
- **Valorant** - Default, Dot, Circle
- **Apex Legends** - Classic, Dot
- **Overwatch** - Default, Circle, Dot
- **Rainbow Six Siege** - Default, Minimal
- **Call of Duty** - Classic, Bracket
- **Pro/Competitive** - Various competitive presets

---

## �🛡️ Anti-Cheat Safety

This overlay does **NOT**:
- Inject into game processes
- Hook DirectX/OpenGL
- Read or modify game memory
- Provide aim assistance

It simply draws a transparent window on top of your screen, similar to Discord overlay or OBS.

---

## 📝 License

MIT License — feel free to modify and distribute!
