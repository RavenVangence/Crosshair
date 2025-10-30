using GameOverlay.Drawing;
using GameOverlay.Windows;
using Crosshair.Models;
using Crosshair.Utils;
using Graphics = GameOverlay.Drawing.Graphics;
using SolidBrush = GameOverlay.Drawing.SolidBrush;
using Image = GameOverlay.Drawing.Image;

namespace Crosshair.Rendering;

/// <summary>
/// Advanced overlay manager with multi-monitor, profiles, and image support
/// </summary>
public class AdvancedOverlayManager : IDisposable
{
    private readonly GraphicsWindow _window;
    private readonly Graphics _graphics;
    private readonly AdvancedSettings _settings;
    private readonly MonitorManager _monitorManager;
    private bool _isVisible = true;

    // Rendering resources
    private readonly Dictionary<string, SolidBrush> _brushCache = new();
    private readonly Dictionary<string, Image> _imageCache = new();

    public bool IsVisible
    {
        get => _isVisible;
        set
        {
            _isVisible = value;
            if (!_isVisible)
            {
                _window.Hide();
            }
            else
            {
                _window.Show();
            }
        }
    }

    public AdvancedSettings Settings => _settings;
    public MonitorManager MonitorManager => _monitorManager;

    public AdvancedOverlayManager(AdvancedSettings settings)
    {
        _settings = settings;
        _monitorManager = new MonitorManager();

        // Get target monitor
        var targetMonitor = GetTargetMonitor();

        // Create graphics with DirectX
        _graphics = new Graphics()
        {
            MeasureFPS = false,
            PerPrimitiveAntiAliasing = _settings.Rendering.AntiAliasing,
            TextAntiAliasing = true,
            UseMultiThreadedFactories = false,
            VSync = _settings.Rendering.VSync,
            Width = targetMonitor.Bounds.Width,
            Height = targetMonitor.Bounds.Height
        };

        // Create overlay window
        _window = new GraphicsWindow(
            targetMonitor.Bounds.X,
            targetMonitor.Bounds.Y,
            targetMonitor.Bounds.Width,
            targetMonitor.Bounds.Height,
            _graphics)
        {
            FPS = _settings.Rendering.TargetFPS,
            IsTopmost = true,
            IsVisible = true
        };

        // Setup event handlers
        _window.DestroyGraphics += OnDestroyGraphics;
        _window.DrawGraphics += OnDrawGraphics;
        _window.SetupGraphics += OnSetupGraphics;
    }

    private MonitorInfo GetTargetMonitor()
    {
        return _settings.Monitor.SelectionMode switch
        {
            Models.MonitorSelectionMode.Primary => _monitorManager.GetPrimaryMonitor()!,
            Models.MonitorSelectionMode.Active => _monitorManager.GetActiveMonitor()!,
            Models.MonitorSelectionMode.Index => _monitorManager.GetMonitorByIndex(_settings.Monitor.MonitorIndex) ?? _monitorManager.GetPrimaryMonitor()!,
            Models.MonitorSelectionMode.DeviceName => _monitorManager.GetMonitorByDeviceName(_settings.Monitor.MonitorDeviceName ?? "") ?? _monitorManager.GetPrimaryMonitor()!,
            _ => _monitorManager.GetPrimaryMonitor()!
        };
    }

    private void OnSetupGraphics(object? sender, SetupGraphicsEventArgs e)
    {
        var gfx = e.Graphics;

        // Clear existing caches
        _brushCache.Clear();
        _imageCache.Clear();

        // Pre-load resources for current profile
        var profile = _settings.GetCurrentProfile();
        if (profile != null)
        {
            foreach (var crosshair in profile.Crosshairs)
            {
                if (crosshair.Type == CrosshairType.Vector && crosshair.Vector != null)
                {
                    // Create brushes for vector crosshairs
                    CreateBrushIfNeeded(gfx, crosshair.Vector.Color);
                    if (crosshair.Vector.ShowOutline)
                    {
                        CreateBrushIfNeeded(gfx, crosshair.Vector.OutlineColor);
                    }
                }
                else if (crosshair.Type == CrosshairType.Image && crosshair.Image != null)
                {
                    // Load image crosshairs
                    LoadImageIfNeeded(gfx, crosshair.Image.ImagePath);
                }
            }
        }
    }

    private void OnDrawGraphics(object? sender, DrawGraphicsEventArgs e)
    {
        if (!_isVisible)
            return;

        var gfx = e.Graphics;
        gfx.ClearScene();

        // Get current profile and active crosshair
        var profile = _settings.GetCurrentProfile();
        if (profile == null)
            return;

        var crosshair = profile.GetActiveCrosshair();
        if (crosshair == null)
            return;

        // Get monitor center
        var targetMonitor = GetTargetMonitor();
        var (baseCenterX, baseCenterY) = _monitorManager.GetMonitorCenter(targetMonitor);

        // Adjust for monitor offset if using multi-monitor
        baseCenterX -= targetMonitor.Bounds.X;
        baseCenterY -= targetMonitor.Bounds.Y;

        // Apply transform
        float centerX = baseCenterX + crosshair.Transform.OffsetX;
        float centerY = baseCenterY + crosshair.Transform.OffsetY;

        // Draw crosshair based on type
        if (crosshair.Type == CrosshairType.Vector && crosshair.Vector != null)
        {
            DrawVectorCrosshair(gfx, crosshair.Vector, centerX, centerY, crosshair.Transform);
        }
        else if (crosshair.Type == CrosshairType.Image && crosshair.Image != null)
        {
            DrawImageCrosshair(gfx, crosshair.Image, centerX, centerY, crosshair.Transform);
        }
    }

    private void DrawVectorCrosshair(Graphics gfx, VectorCrosshairSettings vector, float centerX, float centerY, TransformSettings transform)
    {
        // Apply scale and rotation
        float scale = transform.Scale;
        float rotation = transform.Rotation;

        // Get brushes
        var mainBrush = GetBrush(vector.Color);
        var outlineBrush = vector.ShowOutline ? GetBrush(vector.OutlineColor) : null;

        if (mainBrush == null)
            return;

        // Save current transform state (if we implement rotation)
        // For now, we'll implement basic scaling

        // Draw based on style
        switch (vector.Style)
        {
            case Models.CrosshairStyle.Cross:
                DrawCross(gfx, centerX, centerY, vector, scale, outlineBrush, mainBrush);
                break;
            case Models.CrosshairStyle.Dot:
                DrawDot(gfx, centerX, centerY, vector, scale, outlineBrush, mainBrush);
                break;
            case Models.CrosshairStyle.Circle:
                DrawCircle(gfx, centerX, centerY, vector, scale, outlineBrush, mainBrush);
                break;
            case Models.CrosshairStyle.TShape:
                DrawTShape(gfx, centerX, centerY, vector, scale, outlineBrush, mainBrush);
                break;
            case Models.CrosshairStyle.Square:
                DrawSquare(gfx, centerX, centerY, vector, scale, outlineBrush, mainBrush);
                break;
            case Models.CrosshairStyle.Plus:
                DrawPlus(gfx, centerX, centerY, vector, scale, outlineBrush, mainBrush);
                break;
            case Models.CrosshairStyle.XShape:
                DrawXShape(gfx, centerX, centerY, vector, scale, outlineBrush, mainBrush);
                break;
            case Models.CrosshairStyle.Bracket:
                DrawBracket(gfx, centerX, centerY, vector, scale, outlineBrush, mainBrush);
                break;
        }

        // Draw center dot if enabled
        if (vector.ShowCenterDot)
        {
            float dotSize = vector.CenterDotSize * scale;
            if (outlineBrush != null)
            {
                gfx.FillCircle(outlineBrush, centerX, centerY, dotSize + 1);
            }
            gfx.FillCircle(mainBrush, centerX, centerY, dotSize);
        }
    }

    private void DrawCross(Graphics gfx, float centerX, float centerY, VectorCrosshairSettings v, float scale, SolidBrush? outline, SolidBrush main)
    {
        float size = v.Size * scale;
        float gap = v.Gap * scale;
        float thickness = v.Thickness * scale;

        // Draw outline
        if (outline != null && v.ShowOutline)
        {
            float outlineThickness = thickness + (v.OutlineThickness * 2 * scale);
            gfx.DrawLine(outline, centerX - size, centerY, centerX - gap, centerY, outlineThickness);
            gfx.DrawLine(outline, centerX + gap, centerY, centerX + size, centerY, outlineThickness);
            gfx.DrawLine(outline, centerX, centerY - size, centerX, centerY - gap, outlineThickness);
            gfx.DrawLine(outline, centerX, centerY + gap, centerX, centerY + size, outlineThickness);
        }

        // Draw main crosshair
        gfx.DrawLine(main, centerX - size, centerY, centerX - gap, centerY, thickness);
        gfx.DrawLine(main, centerX + gap, centerY, centerX + size, centerY, thickness);
        gfx.DrawLine(main, centerX, centerY - size, centerX, centerY - gap, thickness);
        gfx.DrawLine(main, centerX, centerY + gap, centerX, centerY + size, thickness);
    }

    private void DrawDot(Graphics gfx, float centerX, float centerY, VectorCrosshairSettings v, float scale, SolidBrush? outline, SolidBrush main)
    {
        float radius = (v.Size / 2f) * scale;

        if (outline != null && v.ShowOutline)
        {
            gfx.FillCircle(outline, centerX, centerY, radius + (v.OutlineThickness * scale));
        }
        gfx.FillCircle(main, centerX, centerY, radius);
    }

    private void DrawCircle(Graphics gfx, float centerX, float centerY, VectorCrosshairSettings v, float scale, SolidBrush? outline, SolidBrush main)
    {
        float radius = v.Size * scale;
        float thickness = v.Thickness * scale;

        if (outline != null && v.ShowOutline)
        {
            gfx.DrawCircle(outline, centerX, centerY, radius, thickness + (v.OutlineThickness * 2 * scale));
        }
        gfx.DrawCircle(main, centerX, centerY, radius, thickness);
    }

    private void DrawTShape(Graphics gfx, float centerX, float centerY, VectorCrosshairSettings v, float scale, SolidBrush? outline, SolidBrush main)
    {
        float size = v.Size * scale;
        float gap = v.Gap * scale;
        float thickness = v.Thickness * scale;

        if (outline != null && v.ShowOutline)
        {
            float outlineThickness = thickness + (v.OutlineThickness * 2 * scale);
            gfx.DrawLine(outline, centerX - size, centerY - gap, centerX + size, centerY - gap, outlineThickness);
            gfx.DrawLine(outline, centerX, centerY - gap, centerX, centerY + size, outlineThickness);
        }

        gfx.DrawLine(main, centerX - size, centerY - gap, centerX + size, centerY - gap, thickness);
        gfx.DrawLine(main, centerX, centerY - gap, centerX, centerY + size, thickness);
    }

    private void DrawSquare(Graphics gfx, float centerX, float centerY, VectorCrosshairSettings v, float scale, SolidBrush? outline, SolidBrush main)
    {
        float size = v.Size * scale;
        float thickness = v.Thickness * scale;

        float left = centerX - size;
        float right = centerX + size;
        float top = centerY - size;
        float bottom = centerY + size;

        if (outline != null && v.ShowOutline)
        {
            float outlineThickness = thickness + (v.OutlineThickness * 2 * scale);
            gfx.DrawRectangle(outline, left, top, right, bottom, outlineThickness);
        }
        gfx.DrawRectangle(main, left, top, right, bottom, thickness);
    }

    private void DrawPlus(Graphics gfx, float centerX, float centerY, VectorCrosshairSettings v, float scale, SolidBrush? outline, SolidBrush main)
    {
        // Plus is like Cross but no gap
        float size = v.Size * scale;
        float thickness = v.Thickness * scale;

        if (outline != null && v.ShowOutline)
        {
            float outlineThickness = thickness + (v.OutlineThickness * 2 * scale);
            gfx.DrawLine(outline, centerX - size, centerY, centerX + size, centerY, outlineThickness);
            gfx.DrawLine(outline, centerX, centerY - size, centerX, centerY + size, outlineThickness);
        }

        gfx.DrawLine(main, centerX - size, centerY, centerX + size, centerY, thickness);
        gfx.DrawLine(main, centerX, centerY - size, centerX, centerY + size, thickness);
    }

    private void DrawXShape(Graphics gfx, float centerX, float centerY, VectorCrosshairSettings v, float scale, SolidBrush? outline, SolidBrush main)
    {
        float size = v.Size * scale;
        float gap = v.Gap * scale;
        float thickness = v.Thickness * scale;
        float diagonal = (float)(size / Math.Sqrt(2));
        float gapDiag = (float)(gap / Math.Sqrt(2));

        if (outline != null && v.ShowOutline)
        {
            float outlineThickness = thickness + (v.OutlineThickness * 2 * scale);
            // Top-left to center
            gfx.DrawLine(outline, centerX - diagonal, centerY - diagonal, centerX - gapDiag, centerY - gapDiag, outlineThickness);
            // Top-right to center
            gfx.DrawLine(outline, centerX + diagonal, centerY - diagonal, centerX + gapDiag, centerY - gapDiag, outlineThickness);
            // Bottom-left to center
            gfx.DrawLine(outline, centerX - diagonal, centerY + diagonal, centerX - gapDiag, centerY + gapDiag, outlineThickness);
            // Bottom-right to center
            gfx.DrawLine(outline, centerX + diagonal, centerY + diagonal, centerX + gapDiag, centerY + gapDiag, outlineThickness);
        }

        gfx.DrawLine(main, centerX - diagonal, centerY - diagonal, centerX - gapDiag, centerY - gapDiag, thickness);
        gfx.DrawLine(main, centerX + diagonal, centerY - diagonal, centerX + gapDiag, centerY - gapDiag, thickness);
        gfx.DrawLine(main, centerX - diagonal, centerY + diagonal, centerX - gapDiag, centerY + gapDiag, thickness);
        gfx.DrawLine(main, centerX + diagonal, centerY + diagonal, centerX + gapDiag, centerY + gapDiag, thickness);
    }

    private void DrawBracket(Graphics gfx, float centerX, float centerY, VectorCrosshairSettings v, float scale, SolidBrush? outline, SolidBrush main)
    {
        float size = v.Size * scale;
        float gap = v.Gap * scale;
        float thickness = v.Thickness * scale;
        float bracketLength = size * 0.4f;

        if (outline != null && v.ShowOutline)
        {
            float outlineThickness = thickness + (v.OutlineThickness * 2 * scale);
            // Top-left bracket
            gfx.DrawLine(outline, centerX - size, centerY - size, centerX - size + bracketLength, centerY - size, outlineThickness);
            gfx.DrawLine(outline, centerX - size, centerY - size, centerX - size, centerY - size + bracketLength, outlineThickness);
            // Top-right bracket
            gfx.DrawLine(outline, centerX + size, centerY - size, centerX + size - bracketLength, centerY - size, outlineThickness);
            gfx.DrawLine(outline, centerX + size, centerY - size, centerX + size, centerY - size + bracketLength, outlineThickness);
            // Bottom-left bracket
            gfx.DrawLine(outline, centerX - size, centerY + size, centerX - size + bracketLength, centerY + size, outlineThickness);
            gfx.DrawLine(outline, centerX - size, centerY + size, centerX - size, centerY + size - bracketLength, outlineThickness);
            // Bottom-right bracket
            gfx.DrawLine(outline, centerX + size, centerY + size, centerX + size - bracketLength, centerY + size, outlineThickness);
            gfx.DrawLine(outline, centerX + size, centerY + size, centerX + size, centerY + size - bracketLength, outlineThickness);
        }

        // Top-left
        gfx.DrawLine(main, centerX - size, centerY - size, centerX - size + bracketLength, centerY - size, thickness);
        gfx.DrawLine(main, centerX - size, centerY - size, centerX - size, centerY - size + bracketLength, thickness);
        // Top-right
        gfx.DrawLine(main, centerX + size, centerY - size, centerX + size - bracketLength, centerY - size, thickness);
        gfx.DrawLine(main, centerX + size, centerY - size, centerX + size, centerY - size + bracketLength, thickness);
        // Bottom-left
        gfx.DrawLine(main, centerX - size, centerY + size, centerX - size + bracketLength, centerY + size, thickness);
        gfx.DrawLine(main, centerX - size, centerY + size, centerX - size, centerY + size - bracketLength, thickness);
        // Bottom-right
        gfx.DrawLine(main, centerX + size, centerY + size, centerX + size - bracketLength, centerY + size, thickness);
        gfx.DrawLine(main, centerX + size, centerY + size, centerX + size, centerY + size - bracketLength, thickness);
    }

    private void DrawImageCrosshair(Graphics gfx, ImageCrosshairSettings image, float centerX, float centerY, TransformSettings transform)
    {
        var img = GetImage(image.ImagePath);
        if (img == null)
            return;

        float width = image.Width * transform.Scale;
        float height = image.Height * transform.Scale;

        float x = centerX - (width / 2f);
        float y = centerY - (height / 2f);

        gfx.DrawImage(img, x, y, width, height, image.Opacity);
    }

    private void CreateBrushIfNeeded(Graphics gfx, ColorRGBA color)
    {
        string key = $"{color.R}_{color.G}_{color.B}_{color.A}";
        if (!_brushCache.ContainsKey(key))
        {
            _brushCache[key] = gfx.CreateSolidBrush(color.R, color.G, color.B, color.A);
        }
    }

    private SolidBrush? GetBrush(ColorRGBA color)
    {
        string key = $"{color.R}_{color.G}_{color.B}_{color.A}";
        return _brushCache.GetValueOrDefault(key);
    }

    private void LoadImageIfNeeded(Graphics gfx, string imagePath)
    {
        if (string.IsNullOrEmpty(imagePath) || _imageCache.ContainsKey(imagePath))
            return;

        try
        {
            if (System.IO.File.Exists(imagePath))
            {
                _imageCache[imagePath] = gfx.CreateImage(imagePath);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to load image {imagePath}: {ex.Message}");
        }
    }

    private Image? GetImage(string imagePath)
    {
        return _imageCache.GetValueOrDefault(imagePath);
    }

    private void OnDestroyGraphics(object? sender, DestroyGraphicsEventArgs e)
    {
        foreach (var brush in _brushCache.Values)
        {
            brush?.Dispose();
        }
        _brushCache.Clear();

        foreach (var image in _imageCache.Values)
        {
            image?.Dispose();
        }
        _imageCache.Clear();
    }

    public void SwitchToNextProfile()
    {
        var profiles = _settings.Profiles;
        var currentIndex = profiles.FindIndex(p => p.Name == _settings.CurrentProfileName);
        var nextIndex = (currentIndex + 1) % profiles.Count;
        _settings.CurrentProfileName = profiles[nextIndex].Name;
        Console.WriteLine($"Switched to profile: {_settings.CurrentProfileName}");
    }

    public void SwitchToPreviousProfile()
    {
        var profiles = _settings.Profiles;
        var currentIndex = profiles.FindIndex(p => p.Name == _settings.CurrentProfileName);
        var prevIndex = currentIndex - 1;
        if (prevIndex < 0) prevIndex = profiles.Count - 1;
        _settings.CurrentProfileName = profiles[prevIndex].Name;
        Console.WriteLine($"Switched to profile: {_settings.CurrentProfileName}");
    }

    public void SwitchToNextCrosshair()
    {
        var profile = _settings.GetCurrentProfile();
        if (profile == null || profile.Crosshairs.Count == 0)
            return;

        profile.ActiveCrosshairIndex = (profile.ActiveCrosshairIndex + 1) % profile.Crosshairs.Count;
        var activeCrosshair = profile.GetActiveCrosshair();
        Console.WriteLine($"Switched to crosshair: {activeCrosshair?.Name}");
    }

    public void SwitchToPreviousCrosshair()
    {
        var profile = _settings.GetCurrentProfile();
        if (profile == null || profile.Crosshairs.Count == 0)
            return;

        profile.ActiveCrosshairIndex--;
        if (profile.ActiveCrosshairIndex < 0)
            profile.ActiveCrosshairIndex = profile.Crosshairs.Count - 1;

        var activeCrosshair = profile.GetActiveCrosshair();
        Console.WriteLine($"Switched to crosshair: {activeCrosshair?.Name}");
    }

    public void Run()
    {
        _window.Create();
        _window.Join();
    }

    public void Dispose()
    {
        OnDestroyGraphics(null, null!);
        _window?.Dispose();
        _graphics?.Dispose();
        GC.SuppressFinalize(this);
    }
}
