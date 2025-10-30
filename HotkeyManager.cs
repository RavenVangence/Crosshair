using System.Windows.Forms;

namespace Crosshair;

/// <summary>
/// Manages global hotkeys for the overlay
/// </summary>
public class HotkeyManager : Form
{
    private const int HOTKEY_TOGGLE = 1;
    private const int HOTKEY_EXIT = 2;

    private readonly OverlayManager _overlayManager;

    public HotkeyManager(OverlayManager overlayManager)
    {
        _overlayManager = overlayManager;

        // Make form invisible
        this.FormBorderStyle = FormBorderStyle.None;
        this.ShowInTaskbar = false;
        this.WindowState = FormWindowState.Minimized;
        this.Opacity = 0;
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
        // Register Insert key to toggle visibility
        bool success1 = NativeMethods.RegisterHotKey(
            this.Handle,
            HOTKEY_TOGGLE,
            NativeMethods.MOD_NOREPEAT,
            NativeMethods.VK_INSERT
        );

        // Register Ctrl+Q to exit
        bool success2 = NativeMethods.RegisterHotKey(
            this.Handle,
            HOTKEY_EXIT,
            NativeMethods.MOD_CONTROL | NativeMethods.MOD_NOREPEAT,
            NativeMethods.VK_Q
        );

        if (!success1 || !success2)
        {
            MessageBox.Show(
                "Failed to register hotkeys. The application may not function correctly.\n\n" +
                "Insert: Toggle crosshair\n" +
                "Ctrl+Q: Exit application",
                "Hotkey Registration Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning
            );
        }
        else
        {
            Console.WriteLine("Hotkeys registered successfully:");
            Console.WriteLine("  Insert: Toggle crosshair visibility");
            Console.WriteLine("  Ctrl+Q: Exit application");
        }
    }

    protected override void WndProc(ref Message m)
    {
        base.WndProc(ref m);

        // Handle hotkey messages
        if (m.Msg == NativeMethods.WM_HOTKEY)
        {
            int id = m.WParam.ToInt32();

            switch (id)
            {
                case HOTKEY_TOGGLE:
                    // Toggle crosshair visibility
                    _overlayManager.IsVisible = !_overlayManager.IsVisible;
                    Console.WriteLine($"Crosshair visibility: {(_overlayManager.IsVisible ? "ON" : "OFF")}");
                    break;

                case HOTKEY_EXIT:
                    // Exit application
                    Console.WriteLine("Exiting application...");
                    Application.Exit();
                    break;
            }
        }
    }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        // Unregister hotkeys
        NativeMethods.UnregisterHotKey(this.Handle, HOTKEY_TOGGLE);
        NativeMethods.UnregisterHotKey(this.Handle, HOTKEY_EXIT);
        base.OnFormClosing(e);
    }

    protected override void SetVisibleCore(bool value)
    {
        // Keep the form hidden
        base.SetVisibleCore(false);
    }
}
