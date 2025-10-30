# 🎯 Crosshair Overlay App

A lightweight, **non-intrusive crosshair overlay** built in **C#**.  
The app renders a customizable crosshair on screen without injecting into any game process — making it **safe** and **anti-cheat–friendly** for general use.

---

## 🚀 Features

- 🧊 Transparent, topmost overlay window  
- 🖱️ Click-through (does not block mouse input)  
- 🎯 Customizable vector or image crosshair  
- ⌨️ Toggle visibility with **Insert** key  
- 🧩 Supports multiple monitors and resolutions  
- 🪶 Lightweight (low CPU & GPU usage)

---

## 🏗️ Tech Stack

- **C# (.NET 8+)**
- **GameOverlay.Net** — for overlay rendering and transparency
- **Win32 Interop (P/Invoke)** — for hotkey and input handling

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

Edit `crosshair-settings.json` to customize:
- Crosshair color (RGBA)
- Size and thickness
- Style (Cross, Dot, Circle, etc.)
- Gap between lines

---

## 🛡️ Anti-Cheat Safety

This overlay does **NOT**:
- Inject into game processes
- Hook DirectX/OpenGL
- Read or modify game memory
- Provide aim assistance

It simply draws a transparent window on top of your screen, similar to Discord overlay or OBS.

---

## 📝 License

MIT License — feel free to modify and distribute!
