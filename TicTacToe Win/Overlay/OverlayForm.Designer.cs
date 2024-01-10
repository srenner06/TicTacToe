namespace TicTacToe_Win;

partial class OverlayForm
{
	/// <summary>
	/// Required designer variable.
	/// </summary>
	private System.ComponentModel.IContainer components = null;

	/// <summary>
	/// Clean up any resources being used.
	/// </summary>
	/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
	protected override void Dispose(bool disposing)
	{
		if (disposing && (components != null))
		{
			components.Dispose();
		}
		base.Dispose(disposing);
	}

	#region Windows Form Designer generated code

	/// <summary>
	/// Required method for Designer support - do not modify
	/// the contents of this method with the code editor.
	/// </summary>
	private void InitializeComponent()
	{
		lblText = new Label();
		SuspendLayout();
		// 
		// lblText
		// 
		lblText.AutoSize = true;
		lblText.Location = new Point(51, 77);
		lblText.Name = "lblText";
		lblText.Size = new Size(50, 20);
		lblText.TabIndex = 0;
		lblText.Text = "label1";
		// 
		// OverlayForm
		// 
		AutoScaleDimensions = new SizeF(8F, 20F);
		AutoScaleMode = AutoScaleMode.Font;
		ClientSize = new Size(623, 172);
		ControlBox = false;
		Controls.Add(lblText);
		FormBorderStyle = FormBorderStyle.FixedSingle;
		MaximizeBox = false;
		MinimizeBox = false;
		Name = "OverlayForm";
		StartPosition = FormStartPosition.CenterParent;
		TopMost = true;
		ResumeLayout(false);
		PerformLayout();
	}

	#endregion

	private Label lblText;
}