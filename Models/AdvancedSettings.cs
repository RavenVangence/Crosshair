using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Crosshair.Models;

/// <summary>
/// Advanced configuration with profiles, multiple crosshairs, and rendering options
/// </summary>
public class AdvancedSettings
{
    public string CurrentProfileName { get; set; } = "Default";
    public List<CrosshairProfile> Profiles { get; set; } = new();
    public RenderingSettings Rendering { get; set; } = new();
    public Dictionary<string, string> Hotkeys { get; set; } = new();
    public MonitorSettings Monitor { get; set; } = new();

    public AdvancedSettings()
    {
        // Create default profile
        Profiles.Add(new CrosshairProfile
        {
            Name = "Default",
            Crosshairs = new List<CrosshairConfig>
            {
                CrosshairConfig.CreateDefault()
            }
        });

        // Default hotkeys
        Hotkeys = new Dictionary<string, string>
        {
            { "ToggleVisibility", "Insert" },
            { "Exit", "Ctrl+Q" },
            { "NextProfile", "Ctrl+Right" },
            { "PreviousProfile", "Ctrl+Left" },
            { "NextCrosshair", "Ctrl+Up" },
            { "PreviousCrosshair", "Ctrl+Down" },
            { "OpenUI", "Ctrl+Shift+C" }
        };
    }

    public CrosshairProfile? GetCurrentProfile()
    {
        return Profiles.FirstOrDefault(p => p.Name == CurrentProfileName);
    }

    public static AdvancedSettings Load(string filePath = "crosshair-advanced.json")
    {
        try
        {
            // Use executable directory for settings file
            var exeDir = AppDomain.CurrentDomain.BaseDirectory;
            var fullPath = Path.Combine(exeDir, filePath);

            if (System.IO.File.Exists(fullPath))
            {
                string json = System.IO.File.ReadAllText(fullPath);
                var settings = JsonSerializer.Deserialize<AdvancedSettings>(json);
                if (settings != null && settings.Profiles.Count > 0)
                    return settings;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to load advanced settings: {ex.Message}");
        }

        var defaultSettings = new AdvancedSettings();
        defaultSettings.Save(filePath);
        return defaultSettings;
    }

    public void Save(string filePath = "crosshair-advanced.json")
    {
        try
        {
            // Use executable directory for settings file
            var exeDir = AppDomain.CurrentDomain.BaseDirectory;
            var fullPath = Path.Combine(exeDir, filePath);

            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };
            string json = JsonSerializer.Serialize(this, options);
            System.IO.File.WriteAllText(fullPath, json);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to save settings: {ex.Message}");
        }
    }
}

public class CrosshairProfile
{
    public string Name { get; set; } = "Default";
    public List<CrosshairConfig> Crosshairs { get; set; } = new();
    public int ActiveCrosshairIndex { get; set; } = 0;

    public CrosshairConfig? GetActiveCrosshair()
    {
        if (ActiveCrosshairIndex >= 0 && ActiveCrosshairIndex < Crosshairs.Count)
            return Crosshairs[ActiveCrosshairIndex];
        return null;
    }
}

public class CrosshairConfig
{
    public string Name { get; set; } = "Crosshair";
    public CrosshairType Type { get; set; } = CrosshairType.Vector;

    // Vector crosshair settings
    public VectorCrosshairSettings? Vector { get; set; }

    // Image crosshair settings
    public ImageCrosshairSettings? Image { get; set; }

    // Transform settings
    public TransformSettings Transform { get; set; } = new();

    public static CrosshairConfig CreateDefault()
    {
        return new CrosshairConfig
        {
            Name = "Default Cross",
            Type = CrosshairType.Vector,
            Vector = new VectorCrosshairSettings
            {
                Style = CrosshairStyle.Cross,
                Size = 20,
                Thickness = 2,
                Gap = 4,
                Color = new ColorRGBA(0, 255, 0, 255),
                OutlineColor = new ColorRGBA(0, 0, 0, 200),
                ShowOutline = true,
                OutlineThickness = 1
            },
            Transform = new TransformSettings()
        };
    }
}

public class VectorCrosshairSettings
{
    public CrosshairStyle Style { get; set; } = CrosshairStyle.Cross;
    public int Size { get; set; } = 20;
    public int Thickness { get; set; } = 2;
    public int Gap { get; set; } = 4;
    public ColorRGBA Color { get; set; } = new ColorRGBA(0, 255, 0, 255);
    public ColorRGBA OutlineColor { get; set; } = new ColorRGBA(0, 0, 0, 200);
    public bool ShowOutline { get; set; } = true;
    public int OutlineThickness { get; set; } = 1;

    // Additional vector properties
    public bool ShowCenterDot { get; set; } = false;
    public int CenterDotSize { get; set; } = 2;
}

public class ImageCrosshairSettings
{
    public string ImagePath { get; set; } = "";
    public int Width { get; set; } = 32;
    public int Height { get; set; } = 32;
    public bool PreserveAspectRatio { get; set; } = true;
    public float Opacity { get; set; } = 1.0f;
}

public class TransformSettings
{
    public float OffsetX { get; set; } = 0;
    public float OffsetY { get; set; } = 0;
    public float Scale { get; set; } = 1.0f;
    public float Rotation { get; set; } = 0; // Degrees
}

public class RenderingSettings
{
    public RenderingMethod Method { get; set; } = RenderingMethod.Overlay;
    public int TargetFPS { get; set; } = 60;
    public bool VSync { get; set; } = true;
    public bool AntiAliasing { get; set; } = true;
}

public class MonitorSettings
{
    public MonitorSelectionMode SelectionMode { get; set; } = MonitorSelectionMode.Primary;
    public int MonitorIndex { get; set; } = 0;
    public string? MonitorDeviceName { get; set; }
}

public enum CrosshairType
{
    Vector,
    Image,
    Custom
}

public enum CrosshairStyle
{
    Cross,
    Dot,
    Circle,
    TShape,
    Square,
    Plus,
    XShape,
    Bracket,
    Custom
}

public enum RenderingMethod
{
    Overlay,      // GameOverlay.Net (current)
    DirectHook,   // DirectX hook (future)
    GameBarWidget // Xbox Game Bar widget (future)
}

public enum MonitorSelectionMode
{
    Primary,
    Active,      // Monitor with active window
    Index,       // Specific monitor by index
    DeviceName   // Specific monitor by device name
}

public class ColorRGBA
{
    public byte R { get; set; }
    public byte G { get; set; }
    public byte B { get; set; }
    public byte A { get; set; }

    public ColorRGBA() { }

    public ColorRGBA(byte r, byte g, byte b, byte a)
    {
        R = r;
        G = g;
        B = b;
        A = a;
    }

    public override string ToString() => $"RGBA({R}, {G}, {B}, {A})";
}
