using Crosshair.Models;
using CrosshairStyle = Crosshair.Models.CrosshairStyle;
using ColorRGBA = Crosshair.Models.ColorRGBA;

namespace Crosshair.Templates;

/// <summary>
/// Built-in crosshair templates from popular games
/// </summary>
public static class CrosshairTemplates
{
    public static List<CrosshairTemplate> GetAllTemplates()
    {
        return new List<CrosshairTemplate>
        {
            // CS:GO Templates
            CreateTemplate("CS:GO Classic", "Counter-Strike Classic", CrosshairStyle.Cross, 12, 2, 3, 0, 255, 0),
            CreateTemplate("CS:GO Dot", "CS:GO Small Dot", CrosshairStyle.Dot, 2, 2, 0, 255, 255, 0),
            CreateTemplate("CS:GO T-Style", "CS:GO T-Shaped", CrosshairStyle.TShape, 10, 2, 2, 0, 255, 255),
            
            // Valorant Templates
            CreateTemplate("Valorant Default", "Valorant Standard", CrosshairStyle.Cross, 10, 2, 2, 255, 255, 255),
            CreateTemplate("Valorant Dot", "Valorant Precise Dot", CrosshairStyle.Dot, 2, 2, 0, 0, 255, 255),
            CreateTemplate("Valorant Circle", "Valorant Circle Aim", CrosshairStyle.Circle, 12, 2, 3, 255, 255, 255),
            
            // Apex Legends Templates
            CreateTemplate("Apex Classic", "Apex Default Cross", CrosshairStyle.Cross, 15, 2, 4, 255, 0, 0),
            CreateTemplate("Apex Dot", "Apex Center Dot", CrosshairStyle.Dot, 3, 2, 0, 255, 0, 0),
            
            // Overwatch Templates
            CreateTemplate("Overwatch Default", "Overwatch Standard", CrosshairStyle.Cross, 14, 3, 5, 0, 255, 0),
            CreateTemplate("Overwatch Circle", "Overwatch Circle", CrosshairStyle.Circle, 16, 2, 4, 0, 255, 0),
            CreateTemplate("Overwatch Dot", "Overwatch Dot", CrosshairStyle.Dot, 4, 3, 0, 255, 255, 255),
            
            // Rainbow Six Siege Templates
            CreateTemplate("R6S Default", "Rainbow Six Default", CrosshairStyle.Cross, 10, 1, 2, 255, 255, 255),
            CreateTemplate("R6S Minimal", "R6S Minimal Dot", CrosshairStyle.Dot, 2, 1, 0, 0, 255, 0),
            
            // Call of Duty Templates
            CreateTemplate("COD Classic", "Call of Duty Classic", CrosshairStyle.Cross, 16, 2, 4, 255, 255, 255),
            CreateTemplate("COD Bracket", "COD Bracket Style", CrosshairStyle.Bracket, 18, 2, 6, 255, 255, 0),
            
            // Custom/Competitive Templates
            CreateTemplate("Pro Minimal", "Minimal Competitive", CrosshairStyle.Dot, 1, 1, 0, 0, 255, 255),
            CreateTemplate("Pro Cross Small", "Small Competitive Cross", CrosshairStyle.Cross, 8, 1, 2, 0, 255, 255),
            CreateTemplate("Pro Cross Medium", "Medium Competitive Cross", CrosshairStyle.Cross, 14, 2, 3, 0, 255, 0),
            CreateTemplate("Large Visible", "Large High Visibility", CrosshairStyle.Cross, 25, 3, 6, 255, 255, 0),
            CreateTemplate("X Style", "X-Shaped Crosshair", CrosshairStyle.XShape, 20, 2, 4, 255, 0, 255),
        };
    }

    private static CrosshairTemplate CreateTemplate(
        string name,
        string description,
        CrosshairStyle style,
        int size,
        int thickness,
        int gap,
        byte r,
        byte g,
        byte b)
    {
        return new CrosshairTemplate
        {
            Name = name,
            Description = description,
            Config = new CrosshairConfig
            {
                Name = name,
                Type = CrosshairType.Vector,
                Vector = new VectorCrosshairSettings
                {
                    Style = style,
                    Size = size,
                    Thickness = thickness,
                    Gap = gap,
                    Color = new Models.ColorRGBA(r, g, b, 255),
                    OutlineColor = new Models.ColorRGBA(0, 0, 0, 200),
                    ShowOutline = true,
                    OutlineThickness = 1
                },
                Transform = new TransformSettings()
            }
        };
    }

    public static CrosshairConfig CreateFromParameters(CrosshairGeneratorParams parameters)
    {
        return new CrosshairConfig
        {
            Name = parameters.Name,
            Type = CrosshairType.Vector,
            Vector = new VectorCrosshairSettings
            {
                Style = parameters.Style,
                Size = parameters.Size,
                Thickness = parameters.Thickness,
                Gap = parameters.Gap,
                Color = parameters.Color,
                OutlineColor = parameters.OutlineColor,
                ShowOutline = parameters.ShowOutline,
                OutlineThickness = parameters.OutlineThickness,
                ShowCenterDot = parameters.ShowCenterDot,
                CenterDotSize = parameters.CenterDotSize
            },
            Transform = new TransformSettings
            {
                OffsetX = parameters.OffsetX,
                OffsetY = parameters.OffsetY,
                Scale = parameters.Scale,
                Rotation = parameters.Rotation
            }
        };
    }
}

public class CrosshairTemplate
{
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public string Category { get; set; } = "General";
    public CrosshairConfig Config { get; set; } = new();
}

public class CrosshairGeneratorParams
{
    public string Name { get; set; } = "Custom Crosshair";
    public Models.CrosshairStyle Style { get; set; } = Models.CrosshairStyle.Cross;
    public int Size { get; set; } = 20;
    public int Thickness { get; set; } = 2;
    public int Gap { get; set; } = 4;
    public Models.ColorRGBA Color { get; set; } = new Models.ColorRGBA(0, 255, 0, 255);
    public Models.ColorRGBA OutlineColor { get; set; } = new Models.ColorRGBA(0, 0, 0, 200);
    public bool ShowOutline { get; set; } = true;
    public int OutlineThickness { get; set; } = 1;
    public bool ShowCenterDot { get; set; } = false;
    public int CenterDotSize { get; set; } = 2;
    public float OffsetX { get; set; } = 0;
    public float OffsetY { get; set; } = 0;
    public float Scale { get; set; } = 1.0f;
    public float Rotation { get; set; } = 0;
}
