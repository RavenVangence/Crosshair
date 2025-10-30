# 🛠️ Build & Deployment Guide

## Prerequisites

- **Windows 10/11** (64-bit recommended)
- **.NET 8.0 SDK** or later
  - Download from: https://dotnet.microsoft.com/download

---

## Quick Start

### 1. Verify .NET Installation
```powershell
dotnet --version
```
Should show `8.0.x` or higher.

### 2. Build the Project
```powershell
cd "c:\Users\lodrickm\Documents\Crosshair"
dotnet build
```

### 3. Run the Application
```powershell
dotnet run
```

Or run the compiled executable directly:
```powershell
.\bin\Debug\net8.0-windows\Crosshair.exe
```

---

## Release Build (Optimized)

### Create Release Build
```powershell
dotnet build --configuration Release
```

The optimized executable will be in:
```
bin\Release\net8.0-windows\Crosshair.exe
```

---

## Standalone Deployment

### Self-Contained Executable (No .NET Required)

Build a portable version that includes .NET runtime:

```powershell
# For Windows x64
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true
```

```powershell
# For Windows x86 (32-bit)
dotnet publish -c Release -r win-x86 --self-contained true -p:PublishSingleFile=true
```

The standalone executable will be in:
```
bin\Release\net8.0-windows\win-x64\publish\Crosshair.exe
```

### Framework-Dependent (Smaller, Requires .NET)

```powershell
dotnet publish -c Release -r win-x64 --self-contained false -p:PublishSingleFile=true
```

---

## Distribution

### Files to Include

When distributing, include:
1. `Crosshair.exe` (from publish folder)
2. `crosshair-settings.json` (optional - auto-generated if missing)
3. `README.md` (optional documentation)
4. `CUSTOMIZATION.md` (optional guide)

### Folder Structure (Portable Package)
```
Crosshair/
├── Crosshair.exe
├── crosshair-settings.json
├── README.md
└── CUSTOMIZATION.md
```

---

## Troubleshooting

### "The application requires .NET Runtime"
Install .NET 8.0 Runtime or use the self-contained build.

### "Failed to register hotkeys"
Another application may be using the same hotkeys. Close other overlay tools or rebind in code.

### "Access Denied" or "Admin Required"
Some games with anti-cheat may require running as Administrator:
```powershell
# Right-click Crosshair.exe → Run as Administrator
```

### Overlay Not Visible
1. Check if Insert was pressed (toggles visibility)
2. Verify `crosshair-settings.json` has visible colors
3. Try running on primary monitor
4. Check Windows Display Settings (scaling/resolution)

---

## Development

### Hot Reload (Development)
```powershell
dotnet watch run
```

### Clean Build
```powershell
dotnet clean
dotnet build
```

### Update Dependencies
```powershell
dotnet restore
```

---

## Performance Optimization

### Reduce File Size
```powershell
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:PublishTrimmed=true
```

**Note:** Trimming may reduce compatibility. Test thoroughly.

---

## Creating an Installer (Optional)

Consider using tools like:
- **Inno Setup** (free, Windows)
- **WiX Toolset** (advanced MSI creation)
- **NSIS** (Nullsoft Scriptable Install System)

---

Happy building! 🚀
