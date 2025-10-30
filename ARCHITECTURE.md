# 🏗️ Architecture Overview

## Project Structure

```
Crosshair/
├── Program.cs                  # Application entry point
├── OverlayManager.cs          # Manages DirectX overlay and rendering
├── HotkeyManager.cs           # Handles global hotkeys (Insert, Ctrl+Q)
├── CrosshairSettings.cs       # Configuration model and JSON serialization
├── NativeMethods.cs           # Win32 API P/Invoke declarations
├── crosshair-settings.json    # User configuration file
├── Crosshair.csproj           # Project file
├── README.md                  # User documentation
├── BUILD.md                   # Build and deployment guide
├── CUSTOMIZATION.md           # Customization guide
└── LICENSE                    # MIT License
```

---

## Component Architecture

### 1. **Program.cs** - Application Lifecycle
- **Responsibilities:**
  - Initialize application
  - Load settings from JSON
  - Create overlay and hotkey managers
  - Coordinate multi-threaded execution
  
- **Threading Model:**
  - **Main Thread:** Runs DirectX overlay (GameOverlay.Net)
  - **STA Thread:** Runs Windows Forms message pump for hotkeys

### 2. **OverlayManager.cs** - Rendering Engine
- **Responsibilities:**
  - Create transparent, topmost window
  - Initialize DirectX graphics
  - Render crosshair at screen center
  - Handle visibility toggling
  
- **Key Technologies:**
  - `GameOverlay.Net` for DirectX rendering
  - Hardware-accelerated graphics with VSync
  - Per-primitive anti-aliasing for smooth lines

- **Rendering Styles:**
  - Cross: Four lines from center with gap
  - Dot: Filled circle at center
  - Circle: Ring with optional center dot
  - TShape: Horizontal line with vertical down
  - Square: Rectangle outline with optional center dot

### 3. **HotkeyManager.cs** - Input Handling
- **Responsibilities:**
  - Register global hotkeys with Windows
  - Listen for WM_HOTKEY messages
  - Toggle overlay visibility
  - Exit application
  
- **Hotkeys:**
  - `Insert` → Toggle crosshair on/off
  - `Ctrl+Q` → Exit application
  
- **Implementation:**
  - Invisible Windows Forms window (message-only)
  - Win32 RegisterHotKey API
  - Non-blocking hotkey detection

### 4. **CrosshairSettings.cs** - Configuration
- **Responsibilities:**
  - Define settings structure
  - Load from `crosshair-settings.json`
  - Save default settings if missing
  - Serialize/deserialize with System.Text.Json
  
- **Settings:**
  - Style, Size, Thickness, Gap
  - Color (RGBA)
  - Outline (Color, Thickness, Enabled)

### 5. **NativeMethods.cs** - Win32 Interop
- **Responsibilities:**
  - P/Invoke declarations for Win32 APIs
  - Hotkey registration (RegisterHotKey, UnregisterHotKey)
  - Window style constants
  - Virtual key codes
  
- **APIs Used:**
  - `user32.dll` for hotkeys and window manipulation

---

## Data Flow

```
┌─────────────┐
│ Program.cs  │
└──────┬──────┘
       │ 1. Load settings
       ▼
┌──────────────────┐
│ CrosshairSettings│◄── crosshair-settings.json
└──────────────────┘
       │ 2. Create managers
       ├─────────────────┬──────────────────┐
       ▼                 ▼                  ▼
┌─────────────┐   ┌─────────────┐   ┌─────────────┐
│OverlayMgr   │   │ HotkeyMgr   │   │NativeMethods│
│(Main Thread)│   │(STA Thread) │   │ (P/Invoke)  │
└──────┬──────┘   └──────┬──────┘   └─────────────┘
       │                 │
       │ 3. Render       │ 4. Listen hotkeys
       ▼                 ▼
  ┌─────────┐      ┌──────────┐
  │DirectX  │      │Win32 API │
  │Overlay  │      │Messages  │
  └─────────┘      └──────────┘
       │                 │
       │                 │ 5. Toggle visibility
       │◄────────────────┘
       ▼
  ┌─────────┐
  │ Screen  │
  └─────────┘
```

---

## Technology Stack

### Core Framework
- **.NET 8.0** (Windows-specific)
- **C# 12** with nullable reference types

### Dependencies
- **GameOverlay.Net 4.3.0**
  - DirectX 11 rendering
  - Hardware-accelerated graphics
  - Transparent overlay support
  
- **System.Windows.Forms**
  - Hotkey message pump
  - Win32 window handle management

### Native APIs
- **user32.dll**
  - RegisterHotKey / UnregisterHotKey
  - GetWindowLong / SetWindowLong

---

## Design Patterns

### 1. **Manager Pattern**
Each major component (Overlay, Hotkey, Settings) has a dedicated manager class.

### 2. **Event-Driven Architecture**
- GameOverlay.Net events: `SetupGraphics`, `DrawGraphics`, `DestroyGraphics`
- Windows messages: `WM_HOTKEY`

### 3. **Separation of Concerns**
- **Rendering:** OverlayManager
- **Input:** HotkeyManager
- **Configuration:** CrosshairSettings
- **Interop:** NativeMethods

### 4. **Dependency Injection**
Settings are loaded once and injected into managers.

---

## Security & Safety

### Why It's Anti-Cheat Safe

1. **No Process Injection**
   - Does NOT inject DLLs into games
   - Does NOT hook DirectX/OpenGL/Vulkan
   
2. **No Memory Manipulation**
   - Does NOT read game memory
   - Does NOT modify game files
   
3. **Passive Overlay**
   - Simply draws a transparent window on top
   - Same principle as Discord overlay, Steam overlay, etc.
   
4. **No Automation**
   - No aim assistance
   - No recoil control
   - No input simulation

**Result:** Acts like a physical crosshair sticker on your monitor.

---

## Performance Characteristics

### CPU Usage
- **Idle:** <1% CPU (VSync at 60 FPS)
- **Active:** ~2-3% CPU during rendering

### Memory Usage
- **Footprint:** ~30-50 MB RAM
- **DirectX buffers:** Minimal (single quad rendering)

### GPU Usage
- **Minimal:** Simple 2D primitives
- **VSync enabled:** Caps at monitor refresh rate

---

## Limitations

1. **Windows Only:** Uses Win32 APIs and DirectX
2. **Primary Monitor:** Centers on primary display
3. **Single Instance:** No multi-crosshair support
4. **Static Positioning:** Always screen-centered
5. **No Image Crosshairs:** Vector-based only (currently)

---

## Future Enhancements (Potential)

- [ ] Multi-monitor support (crosshair on active monitor)
- [ ] Custom positioning (offset from center)
- [ ] Image-based crosshairs (PNG support)
- [ ] Configuration GUI (instead of JSON editing)
- [ ] System tray icon with quick settings
- [ ] Profiles (switch between saved configs)
- [ ] Animated crosshairs (pulse, fade, etc.)
- [ ] Hit marker effects (for practice)

---

## Debugging

### Enable Console Logging
Change `OutputType` in `.csproj`:
```xml
<OutputType>Exe</OutputType>  <!-- Shows console window -->
```

### Common Issues

**Overlay not rendering:**
- Check DirectX support (requires DX11)
- Update graphics drivers
- Verify GameOverlay.Net installation

**Hotkeys not working:**
- Another app may have registered same hotkeys
- Run as Administrator if needed
- Check Task Manager for conflicting apps

---

## Contributing

When extending this project:

1. **Maintain separation:** Keep managers independent
2. **Follow patterns:** Use existing event-driven model
3. **Test thoroughly:** Especially DirectX rendering
4. **Update docs:** Keep README and guides current

---

**Built with ❤️ for gamers who need a reliable crosshair overlay.**
