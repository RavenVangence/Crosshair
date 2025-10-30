using GameOverlay.Drawing;
using GameOverlay.Windows;
using Graphics = GameOverlay.Drawing.Graphics;
using SolidBrush = GameOverlay.Drawing.SolidBrush;
using Crosshair.Models;
using CrosshairStyle = Crosshair.Models.CrosshairStyle;

namespace Crosshair;

/// <summary>
/// Manages the overlay window and rendering
/// </summary>
public class OverlayManager : IDisposable
{
    private readonly GraphicsWindow _window;
    private readonly Graphics _graphics;
    private readonly CrosshairSettings _settings;
    private bool _isVisible = true;
    private SolidBrush? _crosshairBrush;
    private SolidBrush? _outlineBrush;

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

    public OverlayManager(CrosshairSettings settings)
    {
        _settings = settings;

        // Get screen dimensions
        int screenWidth = System.Windows.Forms.Screen.PrimaryScreen?.Bounds.Width ?? 1920;
        int screenHeight = System.Windows.Forms.Screen.PrimaryScreen?.Bounds.Height ?? 1080;

        // Create graphics with DirectX
        _graphics = new Graphics()
        {
            MeasureFPS = false,
            PerPrimitiveAntiAliasing = true,
            TextAntiAliasing = true,
            UseMultiThreadedFactories = false,
            VSync = true,
            Width = screenWidth,
            Height = screenHeight
        };

        // Create overlay window
        _window = new GraphicsWindow(0, 0, screenWidth, screenHeight, _graphics)
        {
            FPS = 60,
            IsTopmost = true,
            IsVisible = true
        };

        // Setup event handlers
        _window.DestroyGraphics += OnDestroyGraphics;
        _window.DrawGraphics += OnDrawGraphics;
        _window.SetupGraphics += OnSetupGraphics;
    }

    private void OnSetupGraphics(object? sender, SetupGraphicsEventArgs e)
    {
        var gfx = e.Graphics;

        // Create brushes for crosshair
        _crosshairBrush = gfx.CreateSolidBrush(
            _settings.Color.R,
            _settings.Color.G,
            _settings.Color.B,
            _settings.Color.A
        );

        if (_settings.ShowOutline)
        {
            _outlineBrush = gfx.CreateSolidBrush(
                _settings.OutlineColor.R,
                _settings.OutlineColor.G,
                _settings.OutlineColor.B,
                _settings.OutlineColor.A
            );
        }
    }

    private void OnDrawGraphics(object? sender, DrawGraphicsEventArgs e)
    {
        if (!_isVisible || _crosshairBrush == null)
            return;

        var gfx = e.Graphics;
        gfx.ClearScene();

        // Get center of screen
        float centerX = gfx.Width / 2f;
        float centerY = gfx.Height / 2f;

        // Draw crosshair based on style
        switch (_settings.Style)
        {
            case CrosshairStyle.Cross:
                DrawCross(gfx, centerX, centerY);
                break;
            case CrosshairStyle.Dot:
                DrawDot(gfx, centerX, centerY);
                break;
            case CrosshairStyle.Circle:
                DrawCircle(gfx, centerX, centerY);
                break;
            case CrosshairStyle.TShape:
                DrawTShape(gfx, centerX, centerY);
                break;
            case CrosshairStyle.Square:
                DrawSquare(gfx, centerX, centerY);
                break;
        }
    }

    private void DrawCross(Graphics gfx, float centerX, float centerY)
    {
        float size = _settings.Size;
        float gap = _settings.Gap;
        float thickness = _settings.Thickness;

        // Draw outline first (if enabled)
        if (_settings.ShowOutline && _outlineBrush != null)
        {
            float outlineThickness = thickness + (_settings.OutlineThickness * 2);

            // Horizontal line
            gfx.DrawLine(_outlineBrush, centerX - size, centerY, centerX - gap, centerY, outlineThickness);
            gfx.DrawLine(_outlineBrush, centerX + gap, centerY, centerX + size, centerY, outlineThickness);

            // Vertical line
            gfx.DrawLine(_outlineBrush, centerX, centerY - size, centerX, centerY - gap, outlineThickness);
            gfx.DrawLine(_outlineBrush, centerX, centerY + gap, centerX, centerY + size, outlineThickness);
        }

        // Draw main crosshair
        // Horizontal line (left and right from center)
        gfx.DrawLine(_crosshairBrush!, centerX - size, centerY, centerX - gap, centerY, thickness);
        gfx.DrawLine(_crosshairBrush!, centerX + gap, centerY, centerX + size, centerY, thickness);

        // Vertical line (top and bottom from center)
        gfx.DrawLine(_crosshairBrush!, centerX, centerY - size, centerX, centerY - gap, thickness);
        gfx.DrawLine(_crosshairBrush!, centerX, centerY + gap, centerX, centerY + size, thickness);
    }

    private void DrawDot(Graphics gfx, float centerX, float centerY)
    {
        float radius = _settings.Size / 2f;

        // Draw outline
        if (_settings.ShowOutline && _outlineBrush != null)
        {
            gfx.FillCircle(_outlineBrush, centerX, centerY, radius + _settings.OutlineThickness);
        }

        // Draw dot
        gfx.FillCircle(_crosshairBrush!, centerX, centerY, radius);
    }

    private void DrawCircle(Graphics gfx, float centerX, float centerY)
    {
        float radius = _settings.Size;
        float thickness = _settings.Thickness;

        // Draw outline
        if (_settings.ShowOutline && _outlineBrush != null)
        {
            gfx.DrawCircle(_outlineBrush, centerX, centerY, radius, thickness + (_settings.OutlineThickness * 2));
        }

        // Draw circle
        gfx.DrawCircle(_crosshairBrush!, centerX, centerY, radius, thickness);

        // Optional center dot
        if (_settings.Gap > 0)
        {
            gfx.FillCircle(_crosshairBrush!, centerX, centerY, _settings.Gap / 2f);
        }
    }

    private void DrawTShape(Graphics gfx, float centerX, float centerY)
    {
        float size = _settings.Size;
        float gap = _settings.Gap;
        float thickness = _settings.Thickness;

        // Draw outline
        if (_settings.ShowOutline && _outlineBrush != null)
        {
            float outlineThickness = thickness + (_settings.OutlineThickness * 2);

            // Horizontal line (full width)
            gfx.DrawLine(_outlineBrush, centerX - size, centerY - gap, centerX + size, centerY - gap, outlineThickness);

            // Vertical line (down only)
            gfx.DrawLine(_outlineBrush, centerX, centerY - gap, centerX, centerY + size, outlineThickness);
        }

        // Draw T-shape
        // Horizontal line at top
        gfx.DrawLine(_crosshairBrush!, centerX - size, centerY - gap, centerX + size, centerY - gap, thickness);

        // Vertical line down from center
        gfx.DrawLine(_crosshairBrush!, centerX, centerY - gap, centerX, centerY + size, thickness);
    }

    private void DrawSquare(Graphics gfx, float centerX, float centerY)
    {
        float size = _settings.Size;
        float thickness = _settings.Thickness;
        float gap = _settings.Gap;

        // Calculate square corners
        float left = centerX - size;
        float right = centerX + size;
        float top = centerY - size;
        float bottom = centerY + size;

        // Draw outline
        if (_settings.ShowOutline && _outlineBrush != null)
        {
            float outlineThickness = thickness + (_settings.OutlineThickness * 2);
            gfx.DrawRectangle(_outlineBrush, left, top, right, bottom, outlineThickness);
        }

        // Draw square
        gfx.DrawRectangle(_crosshairBrush!, left, top, right, bottom, thickness);

        // Optional center dot
        if (gap > 0)
        {
            gfx.FillCircle(_crosshairBrush!, centerX, centerY, gap / 2f);
        }
    }

    private void OnDestroyGraphics(object? sender, DestroyGraphicsEventArgs e)
    {
        _crosshairBrush?.Dispose();
        _outlineBrush?.Dispose();
    }

    public void Run()
    {
        _window.Create();
        _window.Join();
    }

    public void Dispose()
    {
        _window?.Dispose();
        _graphics?.Dispose();
        GC.SuppressFinalize(this);
    }
}
