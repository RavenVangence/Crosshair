using System.Windows.Forms;
using System.Drawing;

namespace Crosshair.UI;

public class InputDialog : Form
{
    private TextBox _textBox;
    public string InputText => _textBox.Text;

    public InputDialog(string prompt, string title, string defaultValue = "")
    {
        this.Text = title;
        this.Size = new Size(400, 150);
        this.StartPosition = FormStartPosition.CenterParent;
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;
        this.MinimizeBox = false;
        this.BackColor = Color.FromArgb(45, 45, 48);

        var promptLabel = new Label
        {
            Text = prompt,
            Location = new Point(20, 20),
            AutoSize = true,
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 10)
        };

        _textBox = new TextBox
        {
            Location = new Point(20, 50),
            Width = 340,
            Text = defaultValue,
            BackColor = Color.FromArgb(60, 60, 60),
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 10)
        };

        var okButton = new Button
        {
            Text = "OK",
            DialogResult = DialogResult.OK,
            Location = new Point(200, 80),
            Width = 75,
            BackColor = Color.FromArgb(0, 122, 204),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand
        };
        okButton.FlatAppearance.BorderSize = 0;

        var cancelButton = new Button
        {
            Text = "Cancel",
            DialogResult = DialogResult.Cancel,
            Location = new Point(285, 80),
            Width = 75,
            BackColor = Color.FromArgb(100, 100, 100),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand
        };
        cancelButton.FlatAppearance.BorderSize = 0;

        this.Controls.Add(promptLabel);
        this.Controls.Add(_textBox);
        this.Controls.Add(okButton);
        this.Controls.Add(cancelButton);

        this.AcceptButton = okButton;
        this.CancelButton = cancelButton;

        _textBox.SelectAll();
        _textBox.Focus();
    }
}
