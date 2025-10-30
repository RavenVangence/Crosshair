# 🎯 Crosshair Overlay - Advanced Features v2.0

## ✨ What's New in v2.0

### Major Features Implemented

#### 1. **Multi-Monitor Support** 🖥️
- Automatic detection of all connected monitors
- Three monitor selection modes:
  - **Primary Monitor**: Always centers on primary display
  - **Active Monitor**: Follows the monitor with the active window
  - **Specific Monitor**: Lock to a specific monitor by index

#### 2. **Profile System** 📁
- Create unlimited profiles for different games/scenarios
- Each profile can contain multiple crosshairs
- Switch between profiles with hotkeys (Ctrl+Left/Right)
- Example: "CS:GO Profile", "Valorant Profile", "General FPS"

#### 3. **Multiple Crosshairs per Profile** 🎯
- Add multiple crosshairs to each profile
- Switch between crosshairs with hotkeys (Ctrl+Up/Down)
- Mix vector and image crosshairs

#### 4. **Image Crosshair Support** 🖼️
- Import PNG, JPG, BMP images as crosshairs
- Adjustable size and opacity
- Perfect for custom designs or game-specific crosshairs

#### 5. **Advanced Transforms** 🔧
- **Offset X/Y**: Move crosshair from center (pixel-perfect positioning)
- **Scale**: Resize crosshairs (0.1x to 5.0x)
- **Rotation**: Rotate crosshairs (0-360 degrees) - *Future enhancement*

#### 6. **Built-in Template Library** 📚
Over 20 pre-made crosshair templates including:
- CS:GO Classic, Dot, T-Style
- Valorant Default, Dot, Circle
- Apex Legends Classic, Dot
- Overwatch Default, Circle, Dot
- Rainbow Six Siege styles
- Call of Duty styles
- Pro/Competitive presets

#### 7. **New Crosshair Styles** ✏️
- **Cross**: Classic 4-line crosshair with gap
- **Dot**: Simple center dot
- **Circle**: Ring crosshair
- **TShape**: T-shaped for precise aiming
- **Square**: Box outline
- **Plus**: Full cross without gap
- **XShape**: Diagonal X-shaped
- **Bracket**: Corner brackets

#### 8. **Enhanced Hotkey System** ⌨️
```
Insert           - Toggle crosshair visibility
Ctrl+Q           - Exit application  
Ctrl+Left/Right  - Switch profiles
Ctrl+Up/Down     - Switch crosshairs within profile
Ctrl+Shift+C     - Open configuration file
```

---

## 📝 Configuration Guide

### Settings File Structure

The app uses `crosshair-advanced.json` for configuration:

```json
{
  "CurrentProfileName": "Default",
  "Profiles": [
    {
      "Name": "Default",
      "ActiveCrosshairIndex": 0,
      "Crosshairs": [
        {
          "Name": "Default Cross",
          "Type": "Vector",
          "Vector": {
            "Style": "Cross",
            "Size": 20,
            "Thickness": 2,
            "Gap": 4,
            "Color": { "R": 0, "G": 255, "B": 0, "A": 255 },
            "OutlineColor": { "R": 0, "G": 0, "B": 0, "A": 200 },
            "ShowOutline": true,
            "OutlineThickness": 1,
            "ShowCenterDot": false,
            "CenterDotSize": 2
          },
          "Transform": {
            "OffsetX": 0,
            "OffsetY": 0,
            "Scale": 1.0,
            "Rotation": 0
          }
        }
      ]
    }
  ],
  "Rendering": {
    "Method": "Overlay",
    "TargetFPS": 60,
    "VSync": true,
    "AntiAliasing": true
  },
  "Monitor": {
    "SelectionMode": "Primary",
    "MonitorIndex": 0,
    "MonitorDeviceName": null
  },
  "Hotkeys": {
    "ToggleVisibility": "Insert",
    "Exit": "Ctrl+Q",
    "NextProfile": "Ctrl+Right",
    "PreviousProfile": "Ctrl+Left",
    "NextCrosshair": "Ctrl+Up",
    "PreviousCrosshair": "Ctrl+Down",
    "OpenUI": "Ctrl+Shift+C"
  }
}
```

---

## 💡 Usage Examples

### Example 1: Multiple Profiles for Different Games

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
            "Thickness": 2,
            "Gap": 3,
            "Color": { "R": 0, "G": 255, "B": 0, "A": 255 }
          }
        },
        {
          "Name": "AWP",
          "Type": "Vector",
          "Vector": {
            "Style": "Dot",
            "Size": 2,
            "Color": { "R": 0, "G": 255, "B": 255, "A": 255 }
          }
        }
      ]
    },
    {
      "Name": "Valorant",
      "Crosshairs": [
        {
          "Name": "Default",
          "Type": "Vector",
          "Vector": {
            "Style": "Cross",
            "Size": 10,
            "Thickness": 2,
            "Gap": 2,
            "Color": { "R": 255, "G": 255, "B": 255, "A": 255 }
          }
        }
      ]
    }
  ]
}
```

**Usage**: Press `Ctrl+Right` to switch from CS:GO to Valorant profile!

### Example 2: Image Crosshair

```json
{
  "Name": "Custom Image",
  "Type": "Image",
  "Image": {
    "ImagePath": "C:\\Users\\YourName\\crosshairs\\my_crosshair.png",
    "Width": 32,
    "Height": 32,
    "PreserveAspectRatio": true,
    "Opacity": 1.0
  },
  "Transform": {
    "OffsetX": 0,
    "OffsetY": -5,
    "Scale": 1.2
  }
}
```

### Example 3: Advanced Transform (Offset for Lean Mechanics)

For games with lean mechanics, you can offset the crosshair:

```json
{
  "Name": "Lean Right",
  "Type": "Vector",
  "Vector": {
    "Style": "Cross",
    "Size": 15,
    "Thickness": 2
  },
  "Transform": {
    "OffsetX": 50,
    "OffsetY": 0,
    "Scale": 0.8
  }
}
```

---

## 🖥️ Multi-Monitor Configuration

### Monitor Selection Modes

#### Primary Monitor (Default)
```json
"Monitor": {
  "SelectionMode": "Primary"
}
```

#### Active Monitor (Follows Active Window)
```json
"Monitor": {
  "SelectionMode": "Active"
}
```

#### Specific Monitor by Index
```json
"Monitor": {
  "SelectionMode": "Index",
  "MonitorIndex": 1
}
```

---

## 🎨 Crosshair Styles Reference

### Cross
```
    |
  --+--
    |
```

### Dot
```
  •
```

### Circle
```
  ⭕
```

### TShape
```
 -----
   |
   |
```

### Square
```
 ┌─┐
 │ │
 └─┘
```

### Plus
```
   |
 --+--
   |
```

### XShape
```
 \ /
  X
 / \
```

### Bracket
```
 ┌ ┐
 
 └ ┘
```

---

## 🚀 Quick Start

### 1. Run the Application
```powershell
dotnet run
```

### 2. Edit Configuration
Press `Ctrl+Shift+C` to open `crosshair-advanced.json` in your default text editor.

### 3. Create a New Profile
Add a new profile to the `Profiles` array:
```json
{
  "Name": "My Profile",
  "Crosshairs": [
    {
      "Name": "My Crosshair",
      "Type": "Vector",
      "Vector": {
        "Style": "Cross",
        "Size": 20,
        "Thickness": 2,
        "Gap": 4,
        "Color": { "R": 255, "G": 0, "B": 0, "A": 255 }
      }
    }
  ]
}
```

### 4. Switch Profiles
Use `Ctrl+Left/Right` to cycle through profiles.

---

## 📊 Performance

- **CPU Usage**: ~2-3% (with VSync at 60 FPS)
- **Memory**: ~40-60 MB
- **GPU**: Minimal (simple 2D primitives)

---

## 🛠️ Troubleshooting

### Crosshair Not Appearing
1. Check if hidden (`Insert` to toggle)
2. Verify `CurrentProfileName` matches a profile in the list
3. Ensure `ActiveCrosshairIndex` is within bounds

### Multi-Monitor Issues
1. Set `SelectionMode` to `"Primary"` first
2. Use monitor index from the detected monitors list (shown at startup)

### Image Crosshair Not Loading
1. Use absolute paths for `ImagePath`
2. Supported formats: PNG, JPG, JPEG, BMP
3. Check file permissions

---

## 🔮 Future Enhancements

- [ ] Pixel-level crosshair editor GUI
- [ ] DirectX hook rendering mode
- [ ] Xbox Game Bar widget integration
- [ ] Animated crosshairs
- [ ] Hit marker effects
- [ ] Full WPF configuration UI

---

## 💾 Backup Your Configuration

Always backup `crosshair-advanced.json` before making major changes!

```powershell
Copy-Item crosshair-advanced.json crosshair-advanced.backup.json
```

---

**Enjoy your fully customizable crosshair overlay!** 🎯✨
