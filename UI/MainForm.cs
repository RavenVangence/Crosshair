using System.Windows.Forms;
using System.Drawing;
using Crosshair.Models;
using Crosshair.Rendering;
using Crosshair.Utils;
using Crosshair.Templates;

namespace Crosshair.UI;

public class MainForm : Form
{
    private readonly AdvancedSettings _settings;
    private readonly AdvancedOverlayManager _overlayManager;
    private Thread? _overlayThread;

    // UI Controls
    private Button _toggleOverlayButton = null!;
    private Button _saveButton = null!;
    private ComboBox _profileComboBox = null!;
    private ComboBox _crosshairComboBox = null!;
    private ComboBox _monitorComboBox = null!;
    private ListBox _templateListBox = null!;
    private Panel _settingsPanel = null!;
    private NotifyIcon _trayIcon = null!;

    // Crosshair settings controls
    private ComboBox _styleComboBox = null!;
    private NumericUpDown _sizeNumeric = null!;
    private NumericUpDown _thicknessNumeric = null!;
    private NumericUpDown _gapNumeric = null!;
    private NumericUpDown _offsetXNumeric = null!;
    private NumericUpDown _offsetYNumeric = null!;
    private NumericUpDown _scaleNumeric = null!;
    private Button _colorButton = null!;
    private CheckBox _showOutlineCheckBox = null!;
    private CheckBox _showCenterDotCheckBox = null!;

    public MainForm(AdvancedSettings settings, AdvancedOverlayManager overlayManager)
    {
        _settings = settings;
        _overlayManager = overlayManager;

        InitializeComponent();
        SetupTrayIcon();
        LoadSettings();
        StartOverlay();
    }

    private void InitializeComponent()
    {
        // Form setup
        this.Text = "🎯 Crosshair Overlay Manager";
        this.Size = new Size(900, 700);
        this.StartPosition = FormStartPosition.CenterScreen;
        this.FormBorderStyle = FormBorderStyle.FixedSingle;
        this.MaximizeBox = false;
        this.BackColor = Color.FromArgb(45, 45, 48);
        this.ForeColor = Color.White;

        // Main layout
        var mainLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1,
            Padding = new Padding(10)
        };
        mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 300));
        mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

        // Left panel - Quick controls
        var leftPanel = CreateLeftPanel();
        mainLayout.Controls.Add(leftPanel, 0, 0);

        // Right panel - Settings
        var rightPanel = CreateRightPanel();
        mainLayout.Controls.Add(rightPanel, 1, 0);

        this.Controls.Add(mainLayout);

        // Handle form closing
        this.FormClosing += MainForm_FormClosing;
    }

    private Panel CreateLeftPanel()
    {
        var panel = new Panel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(5)
        };

        var layout = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.TopDown,
            AutoScroll = true,
            WrapContents = false
        };

        // Title
        var titleLabel = new Label
        {
            Text = "Quick Controls",
            Font = new Font("Segoe UI", 14, FontStyle.Bold),
            ForeColor = Color.White,
            AutoSize = true,
            Margin = new Padding(0, 0, 0, 15)
        };
        layout.Controls.Add(titleLabel);

        // Toggle Overlay Button
        _toggleOverlayButton = new Button
        {
            Text = "✓ Overlay ON",
            Size = new Size(280, 50),
            Font = new Font("Segoe UI", 12, FontStyle.Bold),
            BackColor = Color.FromArgb(0, 122, 204),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand
        };
        _toggleOverlayButton.FlatAppearance.BorderSize = 0;
        _toggleOverlayButton.Click += ToggleOverlay_Click;
        layout.Controls.Add(_toggleOverlayButton);

        // Spacer
        layout.Controls.Add(new Label { Height = 10, Width = 280 });

        // Profile selection
        layout.Controls.Add(CreateLabel("Profile:"));
        _profileComboBox = new ComboBox
        {
            Width = 280,
            DropDownStyle = ComboBoxStyle.DropDownList,
            BackColor = Color.FromArgb(60, 60, 60),
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 10)
        };
        _profileComboBox.SelectedIndexChanged += ProfileComboBox_SelectedIndexChanged;
        layout.Controls.Add(_profileComboBox);

        // Crosshair selection
        layout.Controls.Add(CreateLabel("Crosshair:"));
        _crosshairComboBox = new ComboBox
        {
            Width = 280,
            DropDownStyle = ComboBoxStyle.DropDownList,
            BackColor = Color.FromArgb(60, 60, 60),
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 10)
        };
        _crosshairComboBox.SelectedIndexChanged += CrosshairComboBox_SelectedIndexChanged;
        layout.Controls.Add(_crosshairComboBox);

        // Monitor selection
        layout.Controls.Add(CreateLabel("Monitor:"));
        _monitorComboBox = new ComboBox
        {
            Width = 280,
            DropDownStyle = ComboBoxStyle.DropDownList,
            BackColor = Color.FromArgb(60, 60, 60),
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 10)
        };
        _monitorComboBox.SelectedIndexChanged += MonitorComboBox_SelectedIndexChanged;
        layout.Controls.Add(_monitorComboBox);

        // Spacer
        layout.Controls.Add(new Label { Height = 10, Width = 280 });

        // Templates
        layout.Controls.Add(CreateLabel("Templates:"));
        _templateListBox = new ListBox
        {
            Width = 280,
            Height = 200,
            BackColor = Color.FromArgb(60, 60, 60),
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 9)
        };
        _templateListBox.DoubleClick += TemplateListBox_DoubleClick;
        layout.Controls.Add(_templateListBox);

        // Save button
        _saveButton = new Button
        {
            Text = "💾 Save Settings",
            Size = new Size(280, 40),
            BackColor = Color.FromArgb(16, 124, 16),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 11, FontStyle.Bold),
            Cursor = Cursors.Hand,
            Margin = new Padding(0, 10, 0, 0)
        };
        _saveButton.FlatAppearance.BorderSize = 0;
        _saveButton.Click += SaveButton_Click;
        layout.Controls.Add(_saveButton);

        panel.Controls.Add(layout);
        return panel;
    }

    private Panel CreateRightPanel()
    {
        var panel = new Panel
        {
            Dock = DockStyle.Fill,
            AutoScroll = true,
            Padding = new Padding(5)
        };

        _settingsPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.TopDown,
            AutoScroll = true,
            WrapContents = false,
            Padding = new Padding(10)
        };

        // Title
        var titleLabel = new Label
        {
            Text = "Crosshair Settings",
            Font = new Font("Segoe UI", 14, FontStyle.Bold),
            ForeColor = Color.White,
            AutoSize = true,
            Margin = new Padding(0, 0, 0, 15)
        };
        _settingsPanel.Controls.Add(titleLabel);

        // Style
        _settingsPanel.Controls.Add(CreateLabel("Style:"));
        _styleComboBox = new ComboBox
        {
            Width = 200,
            DropDownStyle = ComboBoxStyle.DropDownList,
            BackColor = Color.FromArgb(60, 60, 60),
            ForeColor = Color.White
        };
        _styleComboBox.DataSource = Enum.GetValues(typeof(CrosshairStyle));
        _settingsPanel.Controls.Add(_styleComboBox);

        // Size
        _settingsPanel.Controls.Add(CreateLabel("Size:"));
        _sizeNumeric = CreateNumericUpDown(1, 100, 20);
        _settingsPanel.Controls.Add(_sizeNumeric);

        // Thickness
        _settingsPanel.Controls.Add(CreateLabel("Thickness:"));
        _thicknessNumeric = CreateNumericUpDown(1, 10, 2);
        _settingsPanel.Controls.Add(_thicknessNumeric);

        // Gap
        _settingsPanel.Controls.Add(CreateLabel("Gap:"));
        _gapNumeric = CreateNumericUpDown(0, 50, 4);
        _settingsPanel.Controls.Add(_gapNumeric);

        // Color button
        _colorButton = new Button
        {
            Text = "🎨 Set Color",
            Width = 200,
            Height = 35,
            BackColor = Color.FromArgb(0, 122, 204),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand,
            Margin = new Padding(0, 10, 0, 10)
        };
        _colorButton.FlatAppearance.BorderSize = 0;
        _colorButton.Click += ColorButton_Click;
        _settingsPanel.Controls.Add(_colorButton);

        // Checkboxes
        _showOutlineCheckBox = new CheckBox
        {
            Text = "Show Outline",
            ForeColor = Color.White,
            AutoSize = true,
            Checked = true
        };
        _settingsPanel.Controls.Add(_showOutlineCheckBox);

        _showCenterDotCheckBox = new CheckBox
        {
            Text = "Show Center Dot",
            ForeColor = Color.White,
            AutoSize = true
        };
        _settingsPanel.Controls.Add(_showCenterDotCheckBox);

        // Transform section
        var transformLabel = new Label
        {
            Text = "Transform",
            Font = new Font("Segoe UI", 12, FontStyle.Bold),
            ForeColor = Color.White,
            AutoSize = true,
            Margin = new Padding(0, 20, 0, 10)
        };
        _settingsPanel.Controls.Add(transformLabel);

        // Offset X
        _settingsPanel.Controls.Add(CreateLabel("Offset X:"));
        _offsetXNumeric = CreateNumericUpDown(-500, 500, 0);
        _settingsPanel.Controls.Add(_offsetXNumeric);

        // Offset Y
        _settingsPanel.Controls.Add(CreateLabel("Offset Y:"));
        _offsetYNumeric = CreateNumericUpDown(-500, 500, 0);
        _settingsPanel.Controls.Add(_offsetYNumeric);

        // Scale
        _settingsPanel.Controls.Add(CreateLabel("Scale:"));
        _scaleNumeric = new NumericUpDown
        {
            Width = 200,
            Minimum = 0.1m,
            Maximum = 5.0m,
            DecimalPlaces = 1,
            Increment = 0.1m,
            Value = 1.0m,
            BackColor = Color.FromArgb(60, 60, 60),
            ForeColor = Color.White
        };
        _settingsPanel.Controls.Add(_scaleNumeric);

        // Info label
        var infoLabel = new Label
        {
            Text = "Changes are applied when you save settings.",
            ForeColor = Color.FromArgb(200, 200, 200),
            AutoSize = true,
            Margin = new Padding(0, 20, 0, 0),
            Font = new Font("Segoe UI", 9, FontStyle.Italic)
        };
        _settingsPanel.Controls.Add(infoLabel);

        panel.Controls.Add(_settingsPanel);
        return panel;
    }

    private Label CreateLabel(string text)
    {
        return new Label
        {
            Text = text,
            ForeColor = Color.White,
            AutoSize = true,
            Font = new Font("Segoe UI", 10),
            Margin = new Padding(0, 5, 0, 5)
        };
    }

    private NumericUpDown CreateNumericUpDown(decimal min, decimal max, decimal value)
    {
        return new NumericUpDown
        {
            Width = 200,
            Minimum = min,
            Maximum = max,
            Value = value,
            BackColor = Color.FromArgb(60, 60, 60),
            ForeColor = Color.White
        };
    }

    private void SetupTrayIcon()
    {
        _trayIcon = new NotifyIcon
        {
            Icon = SystemIcons.Application,
            Text = "Crosshair Overlay",
            Visible = true
        };

        var trayMenu = new ContextMenuStrip();
        trayMenu.Items.Add("Show", null, (s, e) => { this.Show(); this.WindowState = FormWindowState.Normal; });
        trayMenu.Items.Add("Toggle Overlay", null, (s, e) => ToggleOverlay());
        trayMenu.Items.Add("-");
        trayMenu.Items.Add("Exit", null, (s, e) => Application.Exit());

        _trayIcon.ContextMenuStrip = trayMenu;
        _trayIcon.DoubleClick += (s, e) => { this.Show(); this.WindowState = FormWindowState.Normal; };
    }

    private void LoadSettings()
    {
        // Temporarily disable event handlers
        _profileComboBox.SelectedIndexChanged -= ProfileComboBox_SelectedIndexChanged;

        try
        {
            // Load profiles
            _profileComboBox.Items.Clear();
            foreach (var profile in _settings.Profiles)
            {
                _profileComboBox.Items.Add(profile.Name);
            }

            var currentProfile = _settings.GetCurrentProfile();
            if (currentProfile != null)
            {
                _profileComboBox.SelectedItem = currentProfile.Name;
            }

            // Load monitors
            _monitorComboBox.Items.Clear();
            var monitors = _overlayManager.MonitorManager.GetAllMonitors();
            foreach (var monitor in monitors)
            {
                _monitorComboBox.Items.Add(monitor.ToString());
            }
            if (monitors.Count > 0)
            {
                _monitorComboBox.SelectedIndex = 0;
            }

            // Load templates
            _templateListBox.Items.Clear();
            var templates = CrosshairTemplates.GetAllTemplates();
            foreach (var template in templates)
            {
                _templateListBox.Items.Add($"{template.Name} - {template.Description}");
            }

            LoadCurrentCrosshair();
        }
        finally
        {
            // Re-enable event handler
            _profileComboBox.SelectedIndexChanged += ProfileComboBox_SelectedIndexChanged;
        }
    }

    private void LoadCurrentCrosshair()
    {
        var profile = _settings.GetCurrentProfile();
        if (profile == null) return;

        // Temporarily disable event handlers to prevent infinite loop
        _crosshairComboBox.SelectedIndexChanged -= CrosshairComboBox_SelectedIndexChanged;

        try
        {
            // Load crosshairs for current profile
            _crosshairComboBox.Items.Clear();
            foreach (var crosshair in profile.Crosshairs)
            {
                _crosshairComboBox.Items.Add(crosshair.Name);
            }

            if (profile.ActiveCrosshairIndex < profile.Crosshairs.Count)
            {
                _crosshairComboBox.SelectedIndex = profile.ActiveCrosshairIndex;
            }

            var activeCrosshair = profile.GetActiveCrosshair();
            if (activeCrosshair?.Vector != null)
            {
                _styleComboBox.SelectedItem = activeCrosshair.Vector.Style;
                _sizeNumeric.Value = activeCrosshair.Vector.Size;
                _thicknessNumeric.Value = activeCrosshair.Vector.Thickness;
                _gapNumeric.Value = activeCrosshair.Vector.Gap;
                _showOutlineCheckBox.Checked = activeCrosshair.Vector.ShowOutline;
                _showCenterDotCheckBox.Checked = activeCrosshair.Vector.ShowCenterDot;

                _offsetXNumeric.Value = (decimal)activeCrosshair.Transform.OffsetX;
                _offsetYNumeric.Value = (decimal)activeCrosshair.Transform.OffsetY;
                _scaleNumeric.Value = (decimal)activeCrosshair.Transform.Scale;
            }
        }
        finally
        {
            // Re-enable event handler
            _crosshairComboBox.SelectedIndexChanged += CrosshairComboBox_SelectedIndexChanged;
        }
    }

    private void StartOverlay()
    {
        _overlayThread = new Thread(() =>
        {
            try
            {
                _overlayManager.Run();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Overlay error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        });
        _overlayThread.IsBackground = true;
        _overlayThread.Start();
    }

    private void ToggleOverlay()
    {
        _overlayManager.IsVisible = !_overlayManager.IsVisible;
        UpdateToggleButton();
    }

    private void UpdateToggleButton()
    {
        if (_overlayManager.IsVisible)
        {
            _toggleOverlayButton.Text = "✓ Overlay ON";
            _toggleOverlayButton.BackColor = Color.FromArgb(0, 122, 204);
        }
        else
        {
            _toggleOverlayButton.Text = "✗ Overlay OFF";
            _toggleOverlayButton.BackColor = Color.FromArgb(150, 150, 150);
        }
    }

    private void ToggleOverlay_Click(object? sender, EventArgs e)
    {
        ToggleOverlay();
    }

    private void ProfileComboBox_SelectedIndexChanged(object? sender, EventArgs e)
    {
        if (_profileComboBox.SelectedItem != null)
        {
            _settings.CurrentProfileName = _profileComboBox.SelectedItem.ToString() ?? "Default";
            LoadCurrentCrosshair();
        }
    }

    private void CrosshairComboBox_SelectedIndexChanged(object? sender, EventArgs e)
    {
        var profile = _settings.GetCurrentProfile();
        if (profile != null && _crosshairComboBox.SelectedIndex >= 0)
        {
            profile.ActiveCrosshairIndex = _crosshairComboBox.SelectedIndex;
            LoadCurrentCrosshair();
        }
    }

    private void MonitorComboBox_SelectedIndexChanged(object? sender, EventArgs e)
    {
        _settings.Monitor.MonitorIndex = _monitorComboBox.SelectedIndex;
        _settings.Monitor.SelectionMode = MonitorSelectionMode.Index;
    }

    private void ColorButton_Click(object? sender, EventArgs e)
    {
        var profile = _settings.GetCurrentProfile();
        var crosshair = profile?.GetActiveCrosshair();
        if (crosshair?.Vector == null) return;

        using var colorDialog = new ColorDialog();
        colorDialog.Color = Color.FromArgb(
            crosshair.Vector.Color.A,
            crosshair.Vector.Color.R,
            crosshair.Vector.Color.G,
            crosshair.Vector.Color.B
        );

        if (colorDialog.ShowDialog() == DialogResult.OK)
        {
            crosshair.Vector.Color = new ColorRGBA(
                colorDialog.Color.R,
                colorDialog.Color.G,
                colorDialog.Color.B,
                colorDialog.Color.A
            );
            MessageBox.Show("Color updated! Click 'Save Settings' to apply.", "Color Changed", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }

    private void TemplateListBox_DoubleClick(object? sender, EventArgs e)
    {
        if (_templateListBox.SelectedIndex < 0) return;

        var templates = CrosshairTemplates.GetAllTemplates();
        var template = templates[_templateListBox.SelectedIndex];
        var profile = _settings.GetCurrentProfile();

        if (profile != null)
        {
            // Clone the template config
            var json = System.Text.Json.JsonSerializer.Serialize(template.Config);
            var newCrosshair = System.Text.Json.JsonSerializer.Deserialize<CrosshairConfig>(json);

            if (newCrosshair != null)
            {
                profile.Crosshairs.Add(newCrosshair);
                LoadCurrentCrosshair();
                MessageBox.Show($"Template '{template.Name}' added to profile!", "Template Added", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }

    private void SaveButton_Click(object? sender, EventArgs e)
    {
        try
        {
            // Save current crosshair settings
            var profile = _settings.GetCurrentProfile();
            var crosshair = profile?.GetActiveCrosshair();

            if (crosshair?.Vector != null)
            {
                crosshair.Vector.Style = (CrosshairStyle)(_styleComboBox.SelectedItem ?? CrosshairStyle.Cross);
                crosshair.Vector.Size = (int)_sizeNumeric.Value;
                crosshair.Vector.Thickness = (int)_thicknessNumeric.Value;
                crosshair.Vector.Gap = (int)_gapNumeric.Value;
                crosshair.Vector.ShowOutline = _showOutlineCheckBox.Checked;
                crosshair.Vector.ShowCenterDot = _showCenterDotCheckBox.Checked;

                crosshair.Transform.OffsetX = (float)_offsetXNumeric.Value;
                crosshair.Transform.OffsetY = (float)_offsetYNumeric.Value;
                crosshair.Transform.Scale = (float)_scaleNumeric.Value;
            }

            _settings.Save();
            MessageBox.Show("Settings saved successfully!\n\nRestart the application to see changes.", "Saved", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error saving settings: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void MainForm_FormClosing(object? sender, FormClosingEventArgs e)
    {
        if (e.CloseReason == CloseReason.UserClosing)
        {
            e.Cancel = true;
            this.Hide();
            _trayIcon.ShowBalloonTip(2000, "Crosshair Overlay", "Application minimized to tray", ToolTipIcon.Info);
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _trayIcon?.Dispose();
            _settings.Save();
        }
        base.Dispose(disposing);
    }
}
