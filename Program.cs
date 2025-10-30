using System.Windows.Forms;

namespace Crosshair;

class Program
{
    [STAThread]
    static void Main(string[] args)
    {
        Console.WriteLine("╔════════════════════════════════════════╗");
        Console.WriteLine("║    🎯 Crosshair Overlay - v1.0        ║");
        Console.WriteLine("╚════════════════════════════════════════╝");
        Console.WriteLine();

        // Load settings
        Console.WriteLine("Loading settings...");
        var settings = CrosshairSettings.Load();
        Console.WriteLine($"Style: {settings.Style}");
        Console.WriteLine($"Size: {settings.Size}");
        Console.WriteLine($"Color: RGB({settings.Color.R}, {settings.Color.G}, {settings.Color.B})");
        Console.WriteLine();

        // Create overlay manager
        Console.WriteLine("Initializing overlay...");
        using var overlayManager = new OverlayManager(settings);

        // Create and run hotkey manager in a separate thread
        Thread hotkeyThread = new Thread(() =>
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            using var hotkeyManager = new HotkeyManager(overlayManager);
            Application.Run(hotkeyManager);
        });

        hotkeyThread.SetApartmentState(ApartmentState.STA);
        hotkeyThread.IsBackground = true;
        hotkeyThread.Start();

        Console.WriteLine();
        Console.WriteLine("✓ Overlay started successfully!");
        Console.WriteLine();
        Console.WriteLine("Controls:");
        Console.WriteLine("  Insert    - Toggle crosshair visibility");
        Console.WriteLine("  Ctrl+Q    - Exit application");
        Console.WriteLine();
        Console.WriteLine("Edit 'crosshair-settings.json' to customize appearance.");
        Console.WriteLine();
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
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
