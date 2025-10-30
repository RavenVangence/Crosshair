using System.Text.Json;
using Crosshair.Models;

namespace Crosshair;

/// <summary>
/// Legacy configuration settings for backwards compatibility
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
            if (System.IO.File.Exists(filePath))
            {
                string json = System.IO.File.ReadAllText(filePath);
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
            System.IO.File.WriteAllText(filePath, json);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to save settings: {ex.Message}");
        }
    }
}