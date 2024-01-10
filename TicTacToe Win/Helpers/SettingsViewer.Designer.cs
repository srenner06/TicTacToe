namespace TikTakToe.Helpers;

partial class SettingsViewer
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
		label1 = new Label();
		label2 = new Label();
		btnSave = new Button();
		colorDialog1 = new ColorDialog();
		pbP1Color = new PictureBox();
		pbP2Color = new PictureBox();
		tableLayoutPanel1 = new TableLayoutPanel();
		btnP2ChangeColor = new Button();
		btnP1ChangeColor = new Button();
		label3 = new Label();
		((System.ComponentModel.ISupportInitialize)pbP1Color).BeginInit();
		((System.ComponentModel.ISupportInitialize)pbP2Color).BeginInit();
		tableLayoutPanel1.SuspendLayout();
		SuspendLayout();
		// 
		// label1
		// 
		label1.AutoSize = true;
		label1.Location = new Point(30, 42);
		label1.Name = "label1";
		label1.Size = new Size(53, 20);
		label1.TabIndex = 0;
		label1.Text = "Farben";
		// 
		// label2
		// 
		label2.AutoSize = true;
		label2.Dock = DockStyle.Fill;
		label2.Location = new Point(3, 3);
		label2.Margin = new Padding(3);
		label2.Name = "label2";
		label2.Size = new Size(94, 32);
		label2.TabIndex = 1;
		label2.Text = "Spieler 1";
		label2.TextAlign = ContentAlignment.MiddleCenter;
		// 
		// btnSave
		// 
		btnSave.Location = new Point(655, 42);
		btnSave.Name = "btnSave";
		btnSave.Size = new Size(94, 29);
		btnSave.TabIndex = 2;
		btnSave.Text = "Speichern";
		btnSave.UseVisualStyleBackColor = true;
		btnSave.Click += btnSave_Click;
		// 
		// pbP1Color
		// 
		pbP1Color.Dock = DockStyle.Fill;
		pbP1Color.Location = new Point(103, 3);
		pbP1Color.Name = "pbP1Color";
		pbP1Color.Size = new Size(110, 32);
		pbP1Color.TabIndex = 3;
		pbP1Color.TabStop = false;
		// 
		// pbP2Color
		// 
		pbP2Color.Dock = DockStyle.Fill;
		pbP2Color.Location = new Point(103, 41);
		pbP2Color.Name = "pbP2Color";
		pbP2Color.Size = new Size(110, 33);
		pbP2Color.TabIndex = 4;
		pbP2Color.TabStop = false;
		// 
		// tableLayoutPanel1
		// 
		tableLayoutPanel1.ColumnCount = 3;
		tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100F));
		tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle());
		tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle());
		tableLayoutPanel1.Controls.Add(btnP2ChangeColor, 2, 1);
		tableLayoutPanel1.Controls.Add(label2, 0, 0);
		tableLayoutPanel1.Controls.Add(btnP1ChangeColor, 2, 0);
		tableLayoutPanel1.Controls.Add(pbP2Color, 1, 1);
		tableLayoutPanel1.Controls.Add(label3, 0, 1);
		tableLayoutPanel1.Controls.Add(pbP1Color, 1, 0);
		tableLayoutPanel1.Location = new Point(30, 65);
		tableLayoutPanel1.Name = "tableLayoutPanel1";
		tableLayoutPanel1.RowCount = 2;
		tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
		tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
		tableLayoutPanel1.Size = new Size(318, 77);
		tableLayoutPanel1.TabIndex = 5;
		// 
		// btnP2ChangeColor
		// 
		btnP2ChangeColor.Dock = DockStyle.Fill;
		btnP2ChangeColor.Location = new Point(219, 41);
		btnP2ChangeColor.Name = "btnP2ChangeColor";
		btnP2ChangeColor.Size = new Size(96, 33);
		btnP2ChangeColor.TabIndex = 7;
		btnP2ChangeColor.Text = "Ändern";
		btnP2ChangeColor.UseVisualStyleBackColor = true;
		btnP2ChangeColor.Click += btnP2ChangeColor_Click;
		// 
		// btnP1ChangeColor
		// 
		btnP1ChangeColor.Dock = DockStyle.Fill;
		btnP1ChangeColor.Location = new Point(219, 3);
		btnP1ChangeColor.Name = "btnP1ChangeColor";
		btnP1ChangeColor.Size = new Size(96, 32);
		btnP1ChangeColor.TabIndex = 6;
		btnP1ChangeColor.Text = "Ändern";
		btnP1ChangeColor.UseVisualStyleBackColor = true;
		btnP1ChangeColor.Click += btnP1ChangeColor_Click;
		// 
		// label3
		// 
		label3.AutoSize = true;
		label3.Dock = DockStyle.Fill;
		label3.Location = new Point(3, 41);
		label3.Margin = new Padding(3);
		label3.Name = "label3";
		label3.Size = new Size(94, 33);
		label3.TabIndex = 2;
		label3.Text = "Spieler 2";
		label3.TextAlign = ContentAlignment.MiddleCenter;
		// 
		// SettingsViewer
		// 
		AutoScaleDimensions = new SizeF(8F, 20F);
		AutoScaleMode = AutoScaleMode.Font;
		ClientSize = new Size(800, 450);
		Controls.Add(tableLayoutPanel1);
		Controls.Add(btnSave);
		Controls.Add(label1);
		Name = "SettingsViewer";
		Text = "SettingsViewer";
		FormClosing += this.SettingsViewer_FormClosing;
		((System.ComponentModel.ISupportInitialize)pbP1Color).EndInit();
		((System.ComponentModel.ISupportInitialize)pbP2Color).EndInit();
		tableLayoutPanel1.ResumeLayout(false);
		tableLayoutPanel1.PerformLayout();
		ResumeLayout(false);
		PerformLayout();
	}

	#endregion

	private Label label1;
	private Label label2;
	private Button btnSave;
	private ColorDialog colorDialog1;
	private PictureBox pbP1Color;
	private PictureBox pbP2Color;
	private TableLayoutPanel tableLayoutPanel1;
	private Button btnP2ChangeColor;
	private Button btnP1ChangeColor;
	private Label label3;
}