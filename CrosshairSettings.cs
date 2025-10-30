using System.Text.Json;

namespace Crosshair;

/// <summary>
/// Configuration settings for the crosshair overlay
/// </summary>
public class CrosshairSettings
{
    public CrosshairStyle Style { get; set; } = CrosshairStyle.Cross;
    public int Size { get; set; } = 20;
    public int Thickness { get; set; } = 2;
    public int Gap { get; set; } = 4;
    public ColorRGBA Color { get; set; } = new ColorRGBA(0, 255, 0, 255); // Green
    public ColorRGBA OutlineColor { get; set; } = new ColorRGBA(0, 0, 0, 200); // Black outline
    public bool ShowOutline { get; set; } = true;
    public int OutlineThickness { get; set; } = 1;

    /// <summary>
    /// Load settings from JSON file or create default
    /// </summary>
    public static CrosshairSettings Load(string filePath = "crosshair-settings.json")
    {
        try
        {
            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                return JsonSerializer.Deserialize<CrosshairSettings>(json) ?? new CrosshairSettings();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to load settings: {ex.Message}");
        }

        // Create default settings file
        var defaultSettings = new CrosshairSettings();
        defaultSettings.Save(filePath);
        return defaultSettings;
    }

    /// <summary>
    /// Save settings to JSON file
    /// </summary>
    public void Save(string filePath = "crosshair-settings.json")
    {
        try
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(this, options);
            File.WriteAllText(filePath, json);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to save settings: {ex.Message}");
        }
    }
}

public enum CrosshairStyle
{
    Cross,
    Dot,
    Circle,
    TShape,
    Square
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
}
