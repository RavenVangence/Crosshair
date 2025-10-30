# ⚡ Quick Start Guide

Get your crosshair overlay running in **under 2 minutes**!

---

## 🚀 Launch in 3 Steps

### 1️⃣ Run the Application
```powershell
dotnet run
```

### 2️⃣ See the Crosshair
A **green cross** should appear in the center of your screen! 🎯

### 3️⃣ Test the Controls
- Press **Insert** → Crosshair disappears/reappears
- Press **Ctrl+Q** → Application exits

**That's it!** You now have a working crosshair overlay.

---

## 🎨 Quick Customization

Want a different color or size? Edit `crosshair-settings.json`:

### Change Color to Cyan
```json
"Color": {
  "R": 0,
  "G": 255,
  "B": 255,
  "A": 255
}
```

### Make it Bigger
```json
"Size": 30,
"Thickness": 3
```

### Change Style to Dot
```json
"Style": "Dot"
```

**Restart the app** to see changes!

---

## 🎮 Popular Presets

Copy one of these into your `crosshair-settings.json`:

### 🟢 CS:GO Style (Small, Green)
```json
{
  "Style": "Cross",
  "Size": 12,
  "Thickness": 2,
  "Gap": 3,
  "Color": { "R": 0, "G": 255, "B": 0, "A": 255 },
  "OutlineColor": { "R": 0, "G": 0, "B": 0, "A": 200 },
  "ShowOutline": true,
  "OutlineThickness": 1
}
```

### 🔴 Valorant Style (Cyan Dot)
```json
{
  "Style": "Dot",
  "Size": 3,
  "Thickness": 2,
  "Gap": 0,
  "Color": { "R": 0, "G": 255, "B": 255, "A": 255 },
  "OutlineColor": { "R": 0, "G": 0, "B": 0, "A": 255 },
  "ShowOutline": true,
  "OutlineThickness": 1
}
```

### 🟡 Overwatch Style (Circle)
```json
{
  "Style": "Circle",
  "Size": 15,
  "Thickness": 2,
  "Gap": 4,
  "Color": { "R": 0, "G": 255, "B": 0, "A": 255 },
  "OutlineColor": { "R": 0, "G": 0, "B": 0, "A": 200 },
  "ShowOutline": true,
  "OutlineThickness": 1
}
```

---

## ❓ Troubleshooting

### Can't See Crosshair?
1. Press **Insert** (might be hidden)
2. Check your primary monitor (it centers there)
3. Try brighter colors: White `(255, 255, 255)` or Cyan `(0, 255, 255)`

### Hotkeys Not Working?
1. Make sure the app is running (check console window)
2. Try running as Administrator
3. Close other overlay apps (Discord, Steam, etc.)

### App Won't Start?
```powershell
# Check .NET version
dotnet --version
```
**Need .NET 8.0+** → Download from https://dotnet.microsoft.com/download

---

## 📚 Learn More

- **Full Documentation:** [README.md](README.md)
- **Customization Guide:** [CUSTOMIZATION.md](CUSTOMIZATION.md)
- **Build Instructions:** [BUILD.md](BUILD.md)
- **Architecture:** [ARCHITECTURE.md](ARCHITECTURE.md)

---

## 🎯 Tips for Gamers

1. **Test before playing:** Make sure crosshair is visible and comfortable
2. **Adjust for your game:** Bright games need darker crosshairs, dark games need brighter
3. **Keep it simple:** Smaller crosshairs improve accuracy
4. **Use outline:** Black outline helps visibility on any background
5. **Toggle when needed:** Press Insert to hide during cutscenes/menus

---

**Enjoy your custom crosshair!** 🎮✨

Need help? Check the full [README.md](README.md) or customize in [CUSTOMIZATION.md](CUSTOMIZATION.md).
