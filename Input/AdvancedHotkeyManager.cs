using System.Windows.Forms;
using Crosshair.Rendering;

namespace Crosshair.Input;

/// <summary>
/// Advanced hotkey manager with profile and crosshair switching
/// </summary>
public class AdvancedHotkeyManager : Form
{
    private const int HOTKEY_TOGGLE = 1;
    private const int HOTKEY_EXIT = 2;
    private const int HOTKEY_NEXT_PROFILE = 3;
    private const int HOTKEY_PREV_PROFILE = 4;
    private const int HOTKEY_NEXT_CROSSHAIR = 5;
    private const int HOTKEY_PREV_CROSSHAIR = 6;
    private const int HOTKEY_OPEN_UI = 7;

    private readonly AdvancedOverlayManager _overlayManager;
    private Action? _openUIAction;

    public AdvancedHotkeyManager(AdvancedOverlayManager overlayManager)
    {
        _overlayManager = overlayManager;

        // Make form invisible
        this.FormBorderStyle = FormBorderStyle.None;
        this.ShowInTaskbar = false;
        this.WindowState = FormWindowState.Minimized;
        this.Opacity = 0;
    }

    public void SetOpenUIAction(Action action)
    {
        _openUIAction = action;
    }

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        // Hide the form immediately
        this.Visible = false;
        this.Hide();

        // Register hotkeys
        RegisterHotkeys();
    }

    private void RegisterHotkeys()
    {
        bool success = true;

        // Toggle visibility (Insert)
        success &= NativeMethods.RegisterHotKey(
            this.Handle,
            HOTKEY_TOGGLE,
            NativeMethods.MOD_NOREPEAT,
            NativeMethods.VK_INSERT
        );

        // Exit (Ctrl+Q)
        success &= NativeMethods.RegisterHotKey(
            this.Handle,
            HOTKEY_EXIT,
            NativeMethods.MOD_CONTROL | NativeMethods.MOD_NOREPEAT,
            NativeMethods.VK_Q
        );

        // Next Profile (Ctrl+Right)
        success &= NativeMethods.RegisterHotKey(
            this.Handle,
            HOTKEY_NEXT_PROFILE,
            NativeMethods.MOD_CONTROL | NativeMethods.MOD_NOREPEAT,
            NativeMethods.VK_RIGHT
        );

        // Previous Profile (Ctrl+Left)
        success &= NativeMethods.RegisterHotKey(
            this.Handle,
            HOTKEY_PREV_PROFILE,
            NativeMethods.MOD_CONTROL | NativeMethods.MOD_NOREPEAT,
            NativeMethods.VK_LEFT
        );

        // Next Crosshair (Ctrl+Up)
        success &= NativeMethods.RegisterHotKey(
            this.Handle,
            HOTKEY_NEXT_CROSSHAIR,
            NativeMethods.MOD_CONTROL | NativeMethods.MOD_NOREPEAT,
            NativeMethods.VK_UP
        );

        // Previous Crosshair (Ctrl+Down)
        success &= NativeMethods.RegisterHotKey(
            this.Handle,
            HOTKEY_PREV_CROSSHAIR,
            NativeMethods.MOD_CONTROL | NativeMethods.MOD_NOREPEAT,
            NativeMethods.VK_DOWN
        );

        // Open UI (Ctrl+Shift+C)
        success &= NativeMethods.RegisterHotKey(
            this.Handle,
            HOTKEY_OPEN_UI,
            NativeMethods.MOD_CONTROL | NativeMethods.MOD_SHIFT | NativeMethods.MOD_NOREPEAT,
            0x43 // 'C' key
        );

        if (!success)
        {
            MessageBox.Show(
                "Failed to register some hotkeys. The application may not function correctly.\n\n" +
                "Hotkeys:\n" +
                "  Insert         - Toggle crosshair\n" +
                "  Ctrl+Q         - Exit application\n" +
                "  Ctrl+Left/Right - Switch profiles\n" +
                "  Ctrl+Up/Down   - Switch crosshairs\n" +
                "  Ctrl+Shift+C   - Open UI",
                "Hotkey Registration Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning
            );
        }
        else
        {
            Console.WriteLine("✓ Hotkeys registered successfully:");
            Console.WriteLine("  Insert          - Toggle crosshair visibility");
            Console.WriteLine("  Ctrl+Q          - Exit application");
            Console.WriteLine("  Ctrl+Left/Right - Switch profiles");
            Console.WriteLine("  Ctrl+Up/Down    - Switch crosshairs");
            Console.WriteLine("  Ctrl+Shift+C    - Open configuration UI");
        }
    }

    protected override void WndProc(ref Message m)
    {
        base.WndProc(ref m);

        if (m.Msg == NativeMethods.WM_HOTKEY)
        {
            int id = m.WParam.ToInt32();

            switch (id)
            {
                case HOTKEY_TOGGLE:
                    _overlayManager.IsVisible = !_overlayManager.IsVisible;
                    Console.WriteLine($"Crosshair visibility: {(_overlayManager.IsVisible ? "ON" : "OFF")}");
                    break;

                case HOTKEY_EXIT:
                    Console.WriteLine("Exiting application...");
                    _overlayManager.Settings.Save();
                    Application.Exit();
                    break;

                case HOTKEY_NEXT_PROFILE:
                    _overlayManager.SwitchToNextProfile();
                    break;

                case HOTKEY_PREV_PROFILE:
                    _overlayManager.SwitchToPreviousProfile();
                    break;

                case HOTKEY_NEXT_CROSSHAIR:
                    _overlayManager.SwitchToNextCrosshair();
                    break;

                case HOTKEY_PREV_CROSSHAIR:
                    _overlayManager.SwitchToPreviousCrosshair();
                    break;

                case HOTKEY_OPEN_UI:
                    Console.WriteLine("Opening configuration UI...");
                    _openUIAction?.Invoke();
                    break;
            }
        }
    }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        // Unregister all hotkeys
        NativeMethods.UnregisterHotKey(this.Handle, HOTKEY_TOGGLE);
        NativeMethods.UnregisterHotKey(this.Handle, HOTKEY_EXIT);
        NativeMethods.UnregisterHotKey(this.Handle, HOTKEY_NEXT_PROFILE);
        NativeMethods.UnregisterHotKey(this.Handle, HOTKEY_PREV_PROFILE);
        NativeMethods.UnregisterHotKey(this.Handle, HOTKEY_NEXT_CROSSHAIR);
        NativeMethods.UnregisterHotKey(this.Handle, HOTKEY_PREV_CROSSHAIR);
        NativeMethods.UnregisterHotKey(this.Handle, HOTKEY_OPEN_UI);
        base.OnFormClosing(e);
    }

    protected override void SetVisibleCore(bool value)
    {
        base.SetVisibleCore(false);
    }
}
