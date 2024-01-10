using TicTacToe.Win.Board;

namespace TicTacToe_Win;

partial class MainForm
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
		groupBox1 = new GroupBox();
		rbEnemyOnline = new RadioButton();
		rbEnemyComputer = new RadioButton();
		rbEnemyHuman = new RadioButton();
		btnStart = new Button();
		gbStart = new GroupBox();
		rbStartP2 = new RadioButton();
		rbStartP1 = new RadioButton();
		label2 = new Label();
		gbDifficulty = new GroupBox();
		rbDifficultyImpossible = new RadioButton();
		rbDifficultyNormal = new RadioButton();
		rbDifficultyEasy = new RadioButton();
		label3 = new Label();
		tlpStartSettings = new TableLayoutPanel();
		btnSettings = new Button();
		boardView1 = new BoardView();
		groupBox1.SuspendLayout();
		gbStart.SuspendLayout();
		gbDifficulty.SuspendLayout();
		tlpStartSettings.SuspendLayout();
		SuspendLayout();
		// 
		// label1
		// 
		label1.AutoSize = true;
		label1.Dock = DockStyle.Fill;
		label1.Location = new Point(3, 3);
		label1.Margin = new Padding(3);
		label1.Name = "label1";
		label1.Size = new Size(113, 56);
		label1.TabIndex = 0;
		label1.Text = "Gegner";
		label1.TextAlign = ContentAlignment.MiddleCenter;
		// 
		// groupBox1
		// 
		groupBox1.Controls.Add(rbEnemyOnline);
		groupBox1.Controls.Add(rbEnemyComputer);
		groupBox1.Controls.Add(rbEnemyHuman);
		groupBox1.Dock = DockStyle.Fill;
		groupBox1.Location = new Point(122, 3);
		groupBox1.Name = "groupBox1";
		groupBox1.Size = new Size(375, 56);
		groupBox1.TabIndex = 1;
		groupBox1.TabStop = false;
		// 
		// rbEnemyOnline
		// 
		rbEnemyOnline.AutoSize = true;
		rbEnemyOnline.Location = new Point(246, 23);
		rbEnemyOnline.Name = "rbEnemyOnline";
		rbEnemyOnline.Size = new Size(73, 24);
		rbEnemyOnline.TabIndex = 2;
		rbEnemyOnline.Text = "Online";
		rbEnemyOnline.UseVisualStyleBackColor = true;
		rbEnemyOnline.CheckedChanged += rbEnemyOnline_CheckedChanged;
		// 
		// rbEnemyComputer
		// 
		rbEnemyComputer.AutoSize = true;
		rbEnemyComputer.Location = new Point(119, 23);
		rbEnemyComputer.Name = "rbEnemyComputer";
		rbEnemyComputer.Size = new Size(96, 24);
		rbEnemyComputer.TabIndex = 1;
		rbEnemyComputer.Text = "Computer";
		rbEnemyComputer.UseVisualStyleBackColor = true;
		rbEnemyComputer.CheckedChanged += rbEnemyComputer_CheckedChanged;
		// 
		// rbEnemyHuman
		// 
		rbEnemyHuman.AutoSize = true;
		rbEnemyHuman.Checked = true;
		rbEnemyHuman.Location = new Point(15, 21);
		rbEnemyHuman.Name = "rbEnemyHuman";
		rbEnemyHuman.Size = new Size(80, 24);
		rbEnemyHuman.TabIndex = 0;
		rbEnemyHuman.TabStop = true;
		rbEnemyHuman.Text = "Mensch";
		rbEnemyHuman.UseVisualStyleBackColor = true;
		rbEnemyHuman.CheckedChanged += rbEnemyHuman_CheckedChanged;
		// 
		// btnStart
		// 
		btnStart.Dock = DockStyle.Fill;
		btnStart.Location = new Point(122, 189);
		btnStart.Name = "btnStart";
		btnStart.Size = new Size(375, 58);
		btnStart.TabIndex = 2;
		btnStart.Text = "Starten";
		btnStart.UseVisualStyleBackColor = true;
		btnStart.Click += btnStart_Click;
		// 
		// gbStart
		// 
		gbStart.Controls.Add(rbStartP2);
		gbStart.Controls.Add(rbStartP1);
		gbStart.Dock = DockStyle.Fill;
		gbStart.Location = new Point(122, 65);
		gbStart.Name = "gbStart";
		gbStart.Size = new Size(375, 56);
		gbStart.TabIndex = 5;
		gbStart.TabStop = false;
		// 
		// rbStartP2
		// 
		rbStartP2.AutoSize = true;
		rbStartP2.Location = new Point(119, 23);
		rbStartP2.Name = "rbStartP2";
		rbStartP2.Size = new Size(96, 24);
		rbStartP2.TabIndex = 1;
		rbStartP2.Text = "Computer";
		rbStartP2.UseVisualStyleBackColor = true;
		// 
		// rbStartP1
		// 
		rbStartP1.AutoSize = true;
		rbStartP1.Checked = true;
		rbStartP1.Location = new Point(15, 21);
		rbStartP1.Name = "rbStartP1";
		rbStartP1.Size = new Size(88, 24);
		rbStartP1.TabIndex = 0;
		rbStartP1.TabStop = true;
		rbStartP1.Text = "Spieler 1";
		rbStartP1.UseVisualStyleBackColor = true;
		// 
		// label2
		// 
		label2.AutoSize = true;
		label2.Dock = DockStyle.Fill;
		label2.Location = new Point(3, 65);
		label2.Margin = new Padding(3);
		label2.Name = "label2";
		label2.Size = new Size(113, 56);
		label2.TabIndex = 4;
		label2.Text = "Start";
		label2.TextAlign = ContentAlignment.MiddleCenter;
		// 
		// gbDifficulty
		// 
		gbDifficulty.Controls.Add(rbDifficultyImpossible);
		gbDifficulty.Controls.Add(rbDifficultyNormal);
		gbDifficulty.Controls.Add(rbDifficultyEasy);
		gbDifficulty.Dock = DockStyle.Fill;
		gbDifficulty.Location = new Point(122, 127);
		gbDifficulty.Name = "gbDifficulty";
		gbDifficulty.Size = new Size(375, 56);
		gbDifficulty.TabIndex = 7;
		gbDifficulty.TabStop = false;
		// 
		// rbDifficultyImpossible
		// 
		rbDifficultyImpossible.AutoSize = true;
		rbDifficultyImpossible.Location = new Point(246, 23);
		rbDifficultyImpossible.Name = "rbDifficultyImpossible";
		rbDifficultyImpossible.Size = new Size(102, 24);
		rbDifficultyImpossible.TabIndex = 2;
		rbDifficultyImpossible.Text = "Unmöglich";
		rbDifficultyImpossible.UseVisualStyleBackColor = true;
		// 
		// rbDifficultyNormal
		// 
		rbDifficultyNormal.AutoSize = true;
		rbDifficultyNormal.Checked = true;
		rbDifficultyNormal.Location = new Point(119, 23);
		rbDifficultyNormal.Name = "rbDifficultyNormal";
		rbDifficultyNormal.Size = new Size(80, 24);
		rbDifficultyNormal.TabIndex = 1;
		rbDifficultyNormal.TabStop = true;
		rbDifficultyNormal.Text = "Normal";
		rbDifficultyNormal.UseVisualStyleBackColor = true;
		// 
		// rbDifficultyEasy
		// 
		rbDifficultyEasy.AutoSize = true;
		rbDifficultyEasy.Location = new Point(15, 21);
		rbDifficultyEasy.Name = "rbDifficultyEasy";
		rbDifficultyEasy.Size = new Size(78, 24);
		rbDifficultyEasy.TabIndex = 0;
		rbDifficultyEasy.Text = "Einfach";
		rbDifficultyEasy.UseVisualStyleBackColor = true;
		// 
		// label3
		// 
		label3.AutoSize = true;
		label3.Dock = DockStyle.Fill;
		label3.Location = new Point(3, 127);
		label3.Margin = new Padding(3);
		label3.MaximumSize = new Size(100, 0);
		label3.Name = "label3";
		label3.Size = new Size(100, 56);
		label3.TabIndex = 6;
		label3.Text = "Schwierigkeit (Computer)";
		label3.TextAlign = ContentAlignment.MiddleCenter;
		// 
		// tlpStartSettings
		// 
		tlpStartSettings.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
		tlpStartSettings.BackColor = Color.LightGray;
		tlpStartSettings.ColumnCount = 2;
		tlpStartSettings.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 23.8095245F));
		tlpStartSettings.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 76.1904755F));
		tlpStartSettings.Controls.Add(btnSettings, 0, 3);
		tlpStartSettings.Controls.Add(label3, 0, 2);
		tlpStartSettings.Controls.Add(label1, 0, 0);
		tlpStartSettings.Controls.Add(groupBox1, 1, 0);
		tlpStartSettings.Controls.Add(label2, 0, 1);
		tlpStartSettings.Controls.Add(gbDifficulty, 1, 2);
		tlpStartSettings.Controls.Add(gbStart, 1, 1);
		tlpStartSettings.Controls.Add(btnStart, 1, 3);
		tlpStartSettings.Location = new Point(12, 47);
		tlpStartSettings.MinimumSize = new Size(500, 250);
		tlpStartSettings.Name = "tlpStartSettings";
		tlpStartSettings.RowCount = 4;
		tlpStartSettings.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
		tlpStartSettings.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
		tlpStartSettings.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
		tlpStartSettings.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
		tlpStartSettings.Size = new Size(500, 250);
		tlpStartSettings.TabIndex = 11;
		// 
		// btnSettings
		// 
		btnSettings.Dock = DockStyle.Fill;
		btnSettings.Location = new Point(3, 189);
		btnSettings.Name = "btnSettings";
		btnSettings.Size = new Size(113, 58);
		btnSettings.TabIndex = 8;
		btnSettings.Text = "Einstellungen";
		btnSettings.UseVisualStyleBackColor = true;
		btnSettings.Click += btnSettings_Click;
		// 
		// boardView1
		// 
		boardView1.Location = new Point(685, 12);
		boardView1.Name = "boardView2";
		boardView1.Size = new Size(625, 659);
		boardView1.TabIndex = 12;
		// 
		// MainForm
		// 
		AutoScaleDimensions = new SizeF(8F, 20F);
		AutoScaleMode = AutoScaleMode.Font;
		BackColor = SystemColors.GradientInactiveCaption;
		ClientSize = new Size(1547, 735);
		Controls.Add(boardView1);
		Controls.Add(tlpStartSettings);
		Name = "MainForm";
		Text = "MainForm";
		groupBox1.ResumeLayout(false);
		groupBox1.PerformLayout();
		gbStart.ResumeLayout(false);
		gbStart.PerformLayout();
		gbDifficulty.ResumeLayout(false);
		gbDifficulty.PerformLayout();
		tlpStartSettings.ResumeLayout(false);
		tlpStartSettings.PerformLayout();
		ResumeLayout(false);
	}

	#endregion

	private Label label1;
	private GroupBox groupBox1;
	private RadioButton rbEnemyOnline;
	private RadioButton rbEnemyComputer;
	private RadioButton rbEnemyHuman;
	private Button btnStart;
	private GroupBox gbStart;
	private RadioButton rbStartP2;
	private RadioButton rbStartP1;
	private Label label2;
	private GroupBox gbDifficulty;
	private RadioButton rbDifficultyImpossible;
	private RadioButton rbDifficultyNormal;
	private RadioButton rbDifficultyEasy;
	private Label label3;
	private TableLayoutPanel tlpStartSettings;
	private Button btnSettings;
	private BoardView boardView1;
}