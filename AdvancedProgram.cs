using System.Windows.Forms;
using Crosshair.Models;
using Crosshair.Rendering;
using Crosshair.UI;

namespace Crosshair;

class AdvancedProgram
{
    [STAThread]
    static void Main(string[] args)
    {
        try
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Load or create settings
            var settings = AdvancedSettings.Load();

            // Create overlay manager
            var overlayManager = new AdvancedOverlayManager(settings);

            // Create and run the main form
            var mainForm = new MainForm(settings, overlayManager);
            Application.Run(mainForm);

            // Cleanup on exit
            settings.Save();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Fatal Error:\n\n{ex.Message}\n\nStack Trace:\n{ex.StackTrace}",
                "Application Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }
}
