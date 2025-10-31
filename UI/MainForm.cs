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
    private bool _isLoading = false; // Flag to prevent event handlers during load

    // UI Controls
    private Button _toggleOverlayButton = null!;
    private ComboBox _profileComboBox = null!;
    private ComboBox _crosshairComboBox = null!;
    private ComboBox _monitorComboBox = null!;
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
        this.Size = new Size(1200, 800);
        this.StartPosition = FormStartPosition.CenterScreen;
        this.FormBorderStyle = FormBorderStyle.Sizable;
        this.BackColor = Color.FromArgb(45, 45, 48);
        this.ForeColor = Color.White;
        
        // Handle form closing to properly cleanup overlay
        this.FormClosing += MainForm_FormClosing;

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
            AutoScroll = false,
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
        layout.Controls.Add(new Label { Height = 5, Width = 280 });

        // Exit button
        var exitButton = new Button
        {
            Text = "❌ Exit Application",
            Size = new Size(280, 35),
            Font = new Font("Segoe UI", 9),
            BackColor = Color.FromArgb(150, 50, 50),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand
        };
        exitButton.FlatAppearance.BorderSize = 0;
        exitButton.Click += (s, e) => Application.Exit();
        layout.Controls.Add(exitButton);

        // Spacer
        layout.Controls.Add(new Label { Height = 10, Width = 280 });

        // Profile Management Section
        var profileLabel = new Label
        {
            Text = "Profiles",
            Font = new Font("Segoe UI", 12, FontStyle.Bold),
            ForeColor = Color.White,
            AutoSize = true,
            Margin = new Padding(0, 10, 0, 5)
        };
        layout.Controls.Add(profileLabel);

        // Profile selection
        layout.Controls.Add(CreateLabel("Current Profile:"));
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

        // Profile buttons
        var profileButtonsPanel = new FlowLayoutPanel
        {
            AutoSize = true,
            FlowDirection = FlowDirection.LeftToRight,
            Margin = new Padding(0, 5, 0, 5)
        };

        var addProfileBtn = new Button
        {
            Text = "➕ New",
            Width = 85,
            Height = 30,
            BackColor = Color.FromArgb(0, 150, 0),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand,
            Font = new Font("Segoe UI", 9, FontStyle.Bold)
        };
        addProfileBtn.FlatAppearance.BorderSize = 0;
        addProfileBtn.Click += AddProfile_Click;

        var renameProfileBtn = new Button
        {
            Text = "✏️ Rename",
            Width = 90,
            Height = 30,
            BackColor = Color.FromArgb(100, 100, 100),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand,
            Font = new Font("Segoe UI", 9)
        };
        renameProfileBtn.FlatAppearance.BorderSize = 0;
        renameProfileBtn.Click += RenameProfile_Click;

        var deleteProfileBtn = new Button
        {
            Text = "🗑️ Delete",
            Width = 90,
            Height = 30,
            BackColor = Color.FromArgb(150, 50, 50),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand,
            Font = new Font("Segoe UI", 9)
        };
        deleteProfileBtn.FlatAppearance.BorderSize = 0;
        deleteProfileBtn.Click += DeleteProfile_Click;

        profileButtonsPanel.Controls.Add(addProfileBtn);
        profileButtonsPanel.Controls.Add(renameProfileBtn);
        profileButtonsPanel.Controls.Add(deleteProfileBtn);
        layout.Controls.Add(profileButtonsPanel);

        // Spacer
        layout.Controls.Add(new Label { Height = 10, Width = 280 });

        // Crosshair Management Section
        var crosshairLabel = new Label
        {
            Text = "Crosshairs",
            Font = new Font("Segoe UI", 12, FontStyle.Bold),
            ForeColor = Color.White,
            AutoSize = true,
            Margin = new Padding(0, 10, 0, 5)
        };
        layout.Controls.Add(crosshairLabel);

        // Crosshair selection
        layout.Controls.Add(CreateLabel("Current Crosshair:"));
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

        // Crosshair buttons
        var crosshairButtonsPanel = new FlowLayoutPanel
        {
            AutoSize = true,
            FlowDirection = FlowDirection.LeftToRight,
            Margin = new Padding(0, 5, 0, 5)
        };

        var addCrosshairBtn = new Button
        {
            Text = "➕ New",
            Width = 85,
            Height = 30,
            BackColor = Color.FromArgb(0, 150, 0),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand,
            Font = new Font("Segoe UI", 9, FontStyle.Bold)
        };
        addCrosshairBtn.FlatAppearance.BorderSize = 0;
        addCrosshairBtn.Click += AddCrosshair_Click;

        var renameCrosshairBtn = new Button
        {
            Text = "✏️ Rename",
            Width = 90,
            Height = 30,
            BackColor = Color.FromArgb(100, 100, 100),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand,
            Font = new Font("Segoe UI", 9)
        };
        renameCrosshairBtn.FlatAppearance.BorderSize = 0;
        renameCrosshairBtn.Click += RenameCrosshair_Click;

        var deleteCrosshairBtn = new Button
        {
            Text = "🗑️ Delete",
            Width = 90,
            Height = 30,
            BackColor = Color.FromArgb(150, 50, 50),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand,
            Font = new Font("Segoe UI", 9)
        };
        deleteCrosshairBtn.FlatAppearance.BorderSize = 0;
        deleteCrosshairBtn.Click += DeleteCrosshair_Click;

        crosshairButtonsPanel.Controls.Add(addCrosshairBtn);
        crosshairButtonsPanel.Controls.Add(renameCrosshairBtn);
        crosshairButtonsPanel.Controls.Add(deleteCrosshairBtn);
        layout.Controls.Add(crosshairButtonsPanel);

        // Spacer
        layout.Controls.Add(new Label { Height = 10, Width = 280 });

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

        panel.Controls.Add(layout);
        return panel;
    }

    private Panel CreateRightPanel()
    {
        var panel = new Panel
        {
            Dock = DockStyle.Fill,
            AutoScroll = false,
            Padding = new Padding(10)
        };

        // Main layout - two columns
        var mainLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1,
            Padding = new Padding(0)
        };
        mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));

        // Left column - Crosshair Settings
        var leftPanel = new Panel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(0, 0, 10, 0)
        };

        var leftLayout = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.TopDown,
            AutoScroll = false,
            WrapContents = false
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
        leftLayout.Controls.Add(titleLabel);

        // Style
        leftLayout.Controls.Add(CreateLabel("Style:"));
        _styleComboBox = new ComboBox
        {
            Width = 300,
            DropDownStyle = ComboBoxStyle.DropDownList,
            BackColor = Color.FromArgb(60, 60, 60),
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 11)
        };
        _styleComboBox.DataSource = Enum.GetValues(typeof(CrosshairStyle));
        _styleComboBox.SelectedIndexChanged += (s, e) => ApplyChangesImmediately();
        leftLayout.Controls.Add(_styleComboBox);

        // Size with +/- buttons
        leftLayout.Controls.Add(CreateLabel("Size:"));
        leftLayout.Controls.Add(CreateNumericWithButtons(1, 100, 20, "Size"));

        // Thickness with +/- buttons
        leftLayout.Controls.Add(CreateLabel("Thickness:"));
        leftLayout.Controls.Add(CreateNumericWithButtons(1, 10, 2, "Thickness"));

        // Gap with +/- buttons
        leftLayout.Controls.Add(CreateLabel("Gap:"));
        leftLayout.Controls.Add(CreateNumericWithButtons(0, 50, 4, "Gap"));

        // Color button
        _colorButton = new Button
        {
            Text = "🎨 Set Color",
            Width = 300,
            Height = 40,
            BackColor = Color.FromArgb(0, 122, 204),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand,
            Margin = new Padding(0, 10, 0, 10),
            Font = new Font("Segoe UI", 11, FontStyle.Bold)
        };
        _colorButton.FlatAppearance.BorderSize = 0;
        _colorButton.Click += ColorButton_Click;
        leftLayout.Controls.Add(_colorButton);

        // Checkboxes
        _showOutlineCheckBox = new CheckBox
        {
            Text = "Show Outline",
            ForeColor = Color.White,
            AutoSize = true,
            Checked = true,
            Font = new Font("Segoe UI", 10)
        };
        _showOutlineCheckBox.CheckedChanged += (s, e) => ApplyChangesImmediately();
        leftLayout.Controls.Add(_showOutlineCheckBox);

        _showCenterDotCheckBox = new CheckBox
        {
            Text = "Show Center Dot",
            ForeColor = Color.White,
            AutoSize = true,
            Font = new Font("Segoe UI", 10)
        };
        _showCenterDotCheckBox.CheckedChanged += (s, e) => ApplyChangesImmediately();
        leftLayout.Controls.Add(_showCenterDotCheckBox);

        leftPanel.Controls.Add(leftLayout);

        // Right column - Transform Settings
        var rightPanel = new Panel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(10, 0, 0, 0)
        };

        var rightLayout = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.TopDown,
            AutoScroll = false,
            WrapContents = false
        };

        // Transform section title
        var transformLabel = new Label
        {
            Text = "Transform",
            Font = new Font("Segoe UI", 14, FontStyle.Bold),
            ForeColor = Color.White,
            AutoSize = true,
            Margin = new Padding(0, 0, 0, 15)
        };
        rightLayout.Controls.Add(transformLabel);

        // Offset X with +/- buttons
        rightLayout.Controls.Add(CreateLabel("Offset X:"));
        rightLayout.Controls.Add(CreateNumericWithButtons(-500, 500, 0, "OffsetX"));

        // Offset Y with +/- buttons
        rightLayout.Controls.Add(CreateLabel("Offset Y:"));
        rightLayout.Controls.Add(CreateNumericWithButtons(-500, 500, 0, "OffsetY"));

        // Scale with +/- buttons
        rightLayout.Controls.Add(CreateLabel("Scale:"));
        _scaleNumeric = new NumericUpDown
        {
            Width = 150,
            Minimum = 0.1m,
            Maximum = 5.0m,
            DecimalPlaces = 1,
            Increment = 0.1m,
            Value = 1.0m,
            BackColor = Color.FromArgb(60, 60, 60),
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 11),
            UpDownAlign = LeftRightAlignment.Left
        };
        _scaleNumeric.ValueChanged += (s, e) => ApplyChangesImmediately();
        _scaleNumeric.Controls[0].Visible = false; // Hide up/down buttons

        var scalePanel = new FlowLayoutPanel { AutoSize = true, FlowDirection = FlowDirection.LeftToRight };
        var scaleMinusBtn = new Button
        {
            Text = "-",
            Width = 50,
            Height = 30,
            BackColor = Color.FromArgb(80, 80, 80),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 12, FontStyle.Bold),
            Cursor = Cursors.Hand
        };
        scaleMinusBtn.FlatAppearance.BorderSize = 0;
        scaleMinusBtn.Click += (s, e) => { _scaleNumeric.Value = Math.Max(_scaleNumeric.Minimum, _scaleNumeric.Value - 0.1m); };

        var scalePlusBtn = new Button
        {
            Text = "+",
            Width = 50,
            Height = 30,
            BackColor = Color.FromArgb(80, 80, 80),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 12, FontStyle.Bold),
            Cursor = Cursors.Hand
        };
        scalePlusBtn.FlatAppearance.BorderSize = 0;
        scalePlusBtn.Click += (s, e) => { _scaleNumeric.Value = Math.Min(_scaleNumeric.Maximum, _scaleNumeric.Value + 0.1m); };

        scalePanel.Controls.Add(scaleMinusBtn);
        scalePanel.Controls.Add(_scaleNumeric);
        scalePanel.Controls.Add(scalePlusBtn);
        rightLayout.Controls.Add(scalePanel);

        // Info label
        var infoLabel = new Label
        {
            Text = "Changes are applied immediately!",
            ForeColor = Color.FromArgb(100, 200, 100),
            AutoSize = true,
            Margin = new Padding(0, 30, 0, 0),
            Font = new Font("Segoe UI", 9, FontStyle.Italic)
        };
        rightLayout.Controls.Add(infoLabel);

        rightPanel.Controls.Add(rightLayout);

        mainLayout.Controls.Add(leftPanel, 0, 0);
        mainLayout.Controls.Add(rightPanel, 1, 0);

        panel.Controls.Add(mainLayout);
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

    private Panel CreateNumericWithButtons(decimal min, decimal max, decimal value, string name)
    {
        var panel = new FlowLayoutPanel
        {
            AutoSize = true,
            FlowDirection = FlowDirection.LeftToRight,
            Margin = new Padding(0, 5, 0, 5)
        };

        var numeric = new NumericUpDown
        {
            Width = 150,
            Minimum = min,
            Maximum = max,
            Value = value,
            BackColor = Color.FromArgb(60, 60, 60),
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 11),
            UpDownAlign = LeftRightAlignment.Left
        };
        numeric.ValueChanged += (s, e) => ApplyChangesImmediately();
        numeric.Controls[0].Visible = false; // Hide up/down buttons

        var minusBtn = new Button
        {
            Text = "-",
            Width = 50,
            Height = 30,
            BackColor = Color.FromArgb(80, 80, 80),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 12, FontStyle.Bold),
            Cursor = Cursors.Hand
        };
        minusBtn.FlatAppearance.BorderSize = 0;
        minusBtn.Click += (s, e) =>
        {
            if (numeric.Value > numeric.Minimum)
                numeric.Value -= 1;
        };

        var plusBtn = new Button
        {
            Text = "+",
            Width = 50,
            Height = 30,
            BackColor = Color.FromArgb(80, 80, 80),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 12, FontStyle.Bold),
            Cursor = Cursors.Hand
        };
        plusBtn.FlatAppearance.BorderSize = 0;
        plusBtn.Click += (s, e) =>
        {
            if (numeric.Value < numeric.Maximum)
                numeric.Value += 1;
        };

        panel.Controls.Add(minusBtn);
        panel.Controls.Add(numeric);
        panel.Controls.Add(plusBtn);

        // Store reference based on name
        if (name == "Size") _sizeNumeric = numeric;
        else if (name == "Thickness") _thicknessNumeric = numeric;
        else if (name == "Gap") _gapNumeric = numeric;
        else if (name == "OffsetX") _offsetXNumeric = numeric;
        else if (name == "OffsetY") _offsetYNumeric = numeric;

        return panel;
    }

    private void ApplyChangesImmediately()
    {
        // Don't apply changes if we're currently loading settings
        if (_isLoading) return;

        try
        {
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

                // Save settings immediately
                _settings.Save();
            }
        }
        catch
        {
            // Ignore errors during immediate updates
        }
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
        // Set flag to prevent event handlers during load
        _isLoading = true;

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

            LoadCurrentCrosshair();
        }
        finally
        {
            // Re-enable event handlers
            _isLoading = false;
        }
    }

    private void LoadCurrentCrosshair()
    {
        var profile = _settings.GetCurrentProfile();
        if (profile == null) return;

        // Set flag to prevent event handlers from saving during load
        _isLoading = true;

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
            // Re-enable event handlers
            _isLoading = false;
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
            _settings.Save();
            LoadCurrentCrosshair();
        }
    }

    private void CrosshairComboBox_SelectedIndexChanged(object? sender, EventArgs e)
    {
        var profile = _settings.GetCurrentProfile();
        if (profile != null && _crosshairComboBox.SelectedIndex >= 0)
        {
            profile.ActiveCrosshairIndex = _crosshairComboBox.SelectedIndex;
            _settings.Save();
            LoadCurrentCrosshair();
        }
    }

    private void MonitorComboBox_SelectedIndexChanged(object? sender, EventArgs e)
    {
        _settings.Monitor.MonitorIndex = _monitorComboBox.SelectedIndex;
        _settings.Monitor.SelectionMode = MonitorSelectionMode.Index;
        _settings.Save();
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
            _settings.Save();
        }
    }

    private void AddProfile_Click(object? sender, EventArgs e)
    {
        if (_settings.Profiles.Count >= 10)
        {
            MessageBox.Show("Maximum of 10 profiles allowed!", "Limit Reached", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var input = new InputDialog("Enter profile name:", "New Profile");
        if (input.ShowDialog() == DialogResult.OK && !string.IsNullOrWhiteSpace(input.InputText))
        {
            var profileName = input.InputText.Trim();

            // Check if name already exists
            if (_settings.Profiles.Any(p => p.Name == profileName))
            {
                MessageBox.Show("A profile with this name already exists!", "Duplicate Name", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var newProfile = new CrosshairProfile
            {
                Name = profileName,
                Crosshairs = new List<CrosshairConfig>
                {
                    new CrosshairConfig
                    {
                        Name = "Default Crosshair",
                        Type = CrosshairType.Vector,
                        Vector = new VectorCrosshairSettings
                        {
                            Style = CrosshairStyle.Cross,
                            Size = 20,
                            Thickness = 2,
                            Gap = 4,
                            Color = new ColorRGBA(0, 255, 0, 255),
                            OutlineColor = new ColorRGBA(0, 0, 0, 255),
                            ShowOutline = true,
                            ShowCenterDot = false,
                            CenterDotSize = 2
                        },
                        Transform = new TransformSettings()
                    }
                },
                ActiveCrosshairIndex = 0
            };

            _settings.Profiles.Add(newProfile);
            _settings.CurrentProfileName = profileName;
            _settings.Save();
            LoadSettings();
            MessageBox.Show($"Profile '{profileName}' created!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }

    private void RenameProfile_Click(object? sender, EventArgs e)
    {
        var currentProfile = _settings.GetCurrentProfile();
        if (currentProfile == null) return;

        var input = new InputDialog($"Rename profile '{currentProfile.Name}':", "Rename Profile", currentProfile.Name);
        if (input.ShowDialog() == DialogResult.OK && !string.IsNullOrWhiteSpace(input.InputText))
        {
            var newName = input.InputText.Trim();

            // Check if name already exists
            if (_settings.Profiles.Any(p => p.Name == newName && p != currentProfile))
            {
                MessageBox.Show("A profile with this name already exists!", "Duplicate Name", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var oldName = currentProfile.Name;
            currentProfile.Name = newName;
            _settings.CurrentProfileName = newName;
            _settings.Save();
            LoadSettings();
            MessageBox.Show($"Profile renamed from '{oldName}' to '{newName}'", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }

    private void DeleteProfile_Click(object? sender, EventArgs e)
    {
        if (_settings.Profiles.Count <= 1)
        {
            MessageBox.Show("Cannot delete the last profile!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var currentProfile = _settings.GetCurrentProfile();
        if (currentProfile == null) return;

        var result = MessageBox.Show(
            $"Are you sure you want to delete profile '{currentProfile.Name}'?",
            "Confirm Delete",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);

        if (result == DialogResult.Yes)
        {
            _settings.Profiles.Remove(currentProfile);
            _settings.CurrentProfileName = _settings.Profiles[0].Name;
            _settings.Save();
            LoadSettings();
            MessageBox.Show("Profile deleted!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }

    private void AddCrosshair_Click(object? sender, EventArgs e)
    {
        var profile = _settings.GetCurrentProfile();
        if (profile == null) return;

        var input = new InputDialog("Enter crosshair name:", "New Crosshair");
        if (input.ShowDialog() == DialogResult.OK && !string.IsNullOrWhiteSpace(input.InputText))
        {
            var crosshairName = input.InputText.Trim();

            var newCrosshair = new CrosshairConfig
            {
                Name = crosshairName,
                Type = CrosshairType.Vector,
                Vector = new VectorCrosshairSettings
                {
                    Style = CrosshairStyle.Cross,
                    Size = 20,
                    Thickness = 2,
                    Gap = 4,
                    Color = new ColorRGBA(0, 255, 0, 255),
                    OutlineColor = new ColorRGBA(0, 0, 0, 255),
                    ShowOutline = true,
                    ShowCenterDot = false,
                    CenterDotSize = 2
                },
                Transform = new TransformSettings()
            };

            profile.Crosshairs.Add(newCrosshair);
            profile.ActiveCrosshairIndex = profile.Crosshairs.Count - 1;
            _settings.Save();
            LoadCurrentCrosshair();
            MessageBox.Show($"Crosshair '{crosshairName}' created!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }

    private void RenameCrosshair_Click(object? sender, EventArgs e)
    {
        var profile = _settings.GetCurrentProfile();
        var crosshair = profile?.GetActiveCrosshair();
        if (crosshair == null) return;

        var input = new InputDialog($"Rename crosshair '{crosshair.Name}':", "Rename Crosshair", crosshair.Name);
        if (input.ShowDialog() == DialogResult.OK && !string.IsNullOrWhiteSpace(input.InputText))
        {
            var oldName = crosshair.Name;
            crosshair.Name = input.InputText.Trim();
            _settings.Save();
            LoadCurrentCrosshair();
            MessageBox.Show($"Crosshair renamed from '{oldName}' to '{crosshair.Name}'", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }

    private void DeleteCrosshair_Click(object? sender, EventArgs e)
    {
        var profile = _settings.GetCurrentProfile();
        if (profile == null || profile.Crosshairs.Count <= 1)
        {
            MessageBox.Show("Cannot delete the last crosshair!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var crosshair = profile.GetActiveCrosshair();
        if (crosshair == null) return;

        var result = MessageBox.Show(
            $"Are you sure you want to delete crosshair '{crosshair.Name}'?",
            "Confirm Delete",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);

        if (result == DialogResult.Yes)
        {
            var index = profile.ActiveCrosshairIndex;
            profile.Crosshairs.Remove(crosshair);
            profile.ActiveCrosshairIndex = Math.Max(0, index - 1);
            _settings.Save();
            LoadCurrentCrosshair();
            MessageBox.Show("Crosshair deleted!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
