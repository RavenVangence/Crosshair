using System.Windows;
using System.Windows.Forms;
using Crosshair.Models;
using Crosshair.Rendering;
using Crosshair.Input;
using Application = System.Windows.Forms.Application;

namespace Crosshair;

class AdvancedProgram
{
    [STAThread]
    static void Main(string[] args)
    {
        Console.WriteLine("╔═══════════════════════════════════════════════════╗");
        Console.WriteLine("║    🎯 Crosshair Overlay - Advanced v2.0          ║");
        Console.WriteLine("╚═══════════════════════════════════════════════════╝");
        Console.WriteLine();

        // Load advanced settings
        Console.WriteLine("Loading advanced settings...");
        var settings = AdvancedSettings.Load();

        var currentProfile = settings.GetCurrentProfile();
        var activeCrosshair = currentProfile?.GetActiveCrosshair();

        Console.WriteLine($"Profile: {currentProfile?.Name ?? "None"}");
        Console.WriteLine($"Active Crosshair: {activeCrosshair?.Name ?? "None"}");
        Console.WriteLine($"Crosshairs in Profile: {currentProfile?.Crosshairs.Count ?? 0}");
        Console.WriteLine();

        // Create overlay manager
        Console.WriteLine("Initializing advanced overlay...");
        using var overlayManager = new AdvancedOverlayManager(settings);

        // Show monitor information
        var monitors = overlayManager.MonitorManager.GetAllMonitors();
        Console.WriteLine($"Detected {monitors.Count} monitor(s):");
        foreach (var monitor in monitors)
        {
            Console.WriteLine($"  {monitor}");
        }
        Console.WriteLine();

        // Create hotkey manager with UI support
        AdvancedHotkeyManager? hotkeyManager = null;
        Thread? hotkeyThread = null;

        // Create and run hotkey manager in a separate thread
        hotkeyThread = new Thread(() =>
        {
            System.Windows.Forms.Application.EnableVisualStyles();
            System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);

            hotkeyManager = new AdvancedHotkeyManager(overlayManager);

            // Set up UI action
            hotkeyManager.SetOpenUIAction(() =>
            {
                Console.WriteLine("Configuration UI - Opening settings file...");
                try
                {
                    settings.Save("crosshair-advanced.json");
                    // Open the JSON file in default editor
                    var process = new System.Diagnostics.Process();
                    process.StartInfo = new System.Diagnostics.ProcessStartInfo("crosshair-advanced.json")
                    {
                        UseShellExecute = true
                    };
                    process.Start();
                    Console.WriteLine("Settings file opened. Edit and save, then restart the application.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error opening settings: {ex.Message}");
                }
            });

            System.Windows.Forms.Application.Run(hotkeyManager);
        });

        hotkeyThread.SetApartmentState(ApartmentState.STA);
        hotkeyThread.IsBackground = true;
        hotkeyThread.Start();

        Console.WriteLine("✓ Advanced overlay started successfully!");
        Console.WriteLine();
        Console.WriteLine("═══════════════════════════════════════════════════");
        Console.WriteLine("Controls:");
        Console.WriteLine("  Insert           - Toggle crosshair visibility");
        Console.WriteLine("  Ctrl+Q           - Exit application");
        Console.WriteLine("  Ctrl+Left/Right  - Switch profiles");
        Console.WriteLine("  Ctrl+Up/Down     - Switch crosshairs");
        Console.WriteLine("  Ctrl+Shift+C     - Open configuration UI");
        Console.WriteLine("═══════════════════════════════════════════════════");
        Console.WriteLine();
        Console.WriteLine("Features:");
        Console.WriteLine("  ✅ Multi-monitor support");
        Console.WriteLine("  ✅ Profile system");
        Console.WriteLine("  ✅ Multiple crosshairs per profile");
        Console.WriteLine("  ✅ Image crosshair support");
        Console.WriteLine("  ✅ Offset, scale, rotation");
        Console.WriteLine("  ✅ Built-in templates");
        Console.WriteLine("  ✅ Configuration UI");
        Console.WriteLine();
        Console.WriteLine("Press Ctrl+Shift+C to open the configuration UI");
        Console.WriteLine("Press Ctrl+Q to exit...");
        Console.WriteLine();

        // Run the overlay (blocking call)
        try
        {
            overlayManager.Run();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            Console.WriteLine(ex.StackTrace);
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
        finally
        {
            // Save settings on exit
            settings.Save();
        }
    }
}
