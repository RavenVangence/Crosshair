# 🎮 Crosshair Customization Guide

## Available Crosshair Styles

### 1. Cross (Default)
Classic crosshair with four lines extending from center.
```json
"Style": "Cross"
```

### 2. Dot
Simple center dot.
```json
"Style": "Dot"
```

### 3. Circle
Circular crosshair with optional center dot.
```json
"Style": "Circle"
```

### 4. TShape
T-shaped crosshair (useful for FPS games).
```json
"Style": "TShape"
```

### 5. Square
Square outline with optional center dot.
```json
"Style": "Square"
```

---

## Configuration Options

### Size
Controls the length/radius of the crosshair (in pixels).
```json
"Size": 20
```
- **Recommended range:** 10-50
- Larger values = bigger crosshair

### Thickness
Line thickness (in pixels).
```json
"Thickness": 2
```
- **Recommended range:** 1-5
- Thicker lines are easier to see but may obstruct view

### Gap
Gap from center (in pixels). Creates space in the middle.
```json
"Gap": 4
```
- **Recommended range:** 0-10
- `0` = no gap
- Useful for precise aiming

### Color (RGBA)
Main crosshair color.
```json
"Color": {
  "R": 0,    // Red (0-255)
  "G": 255,  // Green (0-255)
  "B": 0,    // Blue (0-255)
  "A": 255   // Alpha/Opacity (0-255)
}
```

**Popular colors:**
- **Green:** `"R": 0, "G": 255, "B": 0`
- **Cyan:** `"R": 0, "G": 255, "B": 255`
- **Red:** `"R": 255, "G": 0, "B": 0`
- **White:** `"R": 255, "G": 255, "B": 255`
- **Magenta:** `"R": 255, "G": 0, "B": 255`

### Outline
Adds a black outline for better visibility.
```json
"ShowOutline": true,
"OutlineColor": {
  "R": 0,
  "G": 0,
  "B": 0,
  "A": 200
},
"OutlineThickness": 1
```

---

## Example Configurations

### Competitive FPS (Small, Precise)
```json
{
  "Style": "Cross",
  "Size": 10,
  "Thickness": 1,
  "Gap": 2,
  "Color": { "R": 0, "G": 255, "B": 255, "A": 255 },
  "OutlineColor": { "R": 0, "G": 0, "B": 0, "A": 200 },
  "ShowOutline": true,
  "OutlineThickness": 1
}
```

### Casual Gaming (Large, Visible)
```json
{
  "Style": "Cross",
  "Size": 25,
  "Thickness": 3,
  "Gap": 6,
  "Color": { "R": 255, "G": 255, "B": 0, "A": 255 },
  "OutlineColor": { "R": 0, "G": 0, "B": 0, "A": 255 },
  "ShowOutline": true,
  "OutlineThickness": 2
}
```

### Dot Only (Minimal)
```json
{
  "Style": "Dot",
  "Size": 4,
  "Thickness": 2,
  "Gap": 0,
  "Color": { "R": 255, "G": 0, "B": 0, "A": 255 },
  "OutlineColor": { "R": 0, "G": 0, "B": 0, "A": 255 },
  "ShowOutline": true,
  "OutlineThickness": 1
}
```

### Circle Crosshair
```json
{
  "Style": "Circle",
  "Size": 15,
  "Thickness": 2,
  "Gap": 3,
  "Color": { "R": 0, "G": 255, "B": 0, "A": 255 },
  "OutlineColor": { "R": 0, "G": 0, "B": 0, "A": 200 },
  "ShowOutline": true,
  "OutlineThickness": 1
}
```

---

## Tips

1. **Restart Required:** After editing `crosshair-settings.json`, restart the application for changes to take effect.

2. **Visibility:** If you can't see your crosshair:
   - Increase `Size` and `Thickness`
   - Try a brighter color (white or cyan)
   - Enable `ShowOutline` with high opacity

3. **Performance:** The overlay is very lightweight, but if you experience issues:
   - Reduce `OutlineThickness`
   - Simplify the style (use `Dot` instead of `Circle`)

4. **Multiple Monitors:** The crosshair centers on your primary monitor by default.

---

Enjoy your custom crosshair! 🎯
