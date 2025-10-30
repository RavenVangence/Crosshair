using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Crosshair.Utils;

/// <summary>
/// Multi-monitor management and selection
/// </summary>
public class MonitorManager
{
    private readonly List<MonitorInfo> _monitors = new();

    public MonitorManager()
    {
        RefreshMonitors();
    }

    public void RefreshMonitors()
    {
        _monitors.Clear();

        var screens = Screen.AllScreens;
        for (int i = 0; i < screens.Length; i++)
        {
            var screen = screens[i];
            _monitors.Add(new MonitorInfo
            {
                Index = i,
                DeviceName = screen.DeviceName,
                IsPrimary = screen.Primary,
                Bounds = new MonitorBounds
                {
                    X = screen.Bounds.X,
                    Y = screen.Bounds.Y,
                    Width = screen.Bounds.Width,
                    Height = screen.Bounds.Height
                },
                WorkingArea = new MonitorBounds
                {
                    X = screen.WorkingArea.X,
                    Y = screen.WorkingArea.Y,
                    Width = screen.WorkingArea.Width,
                    Height = screen.WorkingArea.Height
                }
            });
        }
    }

    public List<MonitorInfo> GetAllMonitors() => _monitors.ToList();

    public MonitorInfo? GetPrimaryMonitor()
    {
        return _monitors.FirstOrDefault(m => m.IsPrimary);
    }

    public MonitorInfo? GetMonitorByIndex(int index)
    {
        return _monitors.FirstOrDefault(m => m.Index == index);
    }

    public MonitorInfo? GetMonitorByDeviceName(string deviceName)
    {
        return _monitors.FirstOrDefault(m => m.DeviceName == deviceName);
    }

    public MonitorInfo? GetActiveMonitor()
    {
        // Get the monitor containing the active window
        IntPtr foregroundWindow = NativeMethods.GetForegroundWindow();
        if (foregroundWindow == IntPtr.Zero)
            return GetPrimaryMonitor();

        RECT rect;
        if (GetWindowRect(foregroundWindow, out rect))
        {
            int centerX = (rect.Left + rect.Right) / 2;
            int centerY = (rect.Top + rect.Bottom) / 2;

            foreach (var monitor in _monitors)
            {
                if (centerX >= monitor.Bounds.X &&
                    centerX < monitor.Bounds.X + monitor.Bounds.Width &&
                    centerY >= monitor.Bounds.Y &&
                    centerY < monitor.Bounds.Y + monitor.Bounds.Height)
                {
                    return monitor;
                }
            }
        }

        return GetPrimaryMonitor();
    }

    public (float centerX, float centerY) GetMonitorCenter(MonitorInfo monitor)
    {
        float centerX = monitor.Bounds.X + (monitor.Bounds.Width / 2f);
        float centerY = monitor.Bounds.Y + (monitor.Bounds.Height / 2f);
        return (centerX, centerY);
    }

    [DllImport("user32.dll")]
    private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

    [StructLayout(LayoutKind.Sequential)]
    private struct RECT
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }
}

public class MonitorInfo
{
    public int Index { get; set; }
    public string DeviceName { get; set; } = "";
    public bool IsPrimary { get; set; }
    public MonitorBounds Bounds { get; set; } = new();
    public MonitorBounds WorkingArea { get; set; } = new();

    public override string ToString()
    {
        string primary = IsPrimary ? " (Primary)" : "";
        return $"Monitor {Index + 1}: {Bounds.Width}x{Bounds.Height}{primary}";
    }
}

public class MonitorBounds
{
    public int X { get; set; }
    public int Y { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
}
