using TikTakToe.Lib.Board;
using TikTakToe.Lib.MoveCalculators;
using TikTakToe.Helpers;
using TikTakToe.Lib.Enums;
using static TikTakToe.Lib.Board.BoardHandler;
namespace TikTakToe;
public partial class MainForm : Form
{
	public MainForm()
	{
		InitializeComponent();
		AllowTransparency = true;

		rbEnemyHuman.Checked = true;
		rbEnemyHuman_CheckedChanged(rbEnemyHuman, EventArgs.Empty);

		boardView1.Finished += OnGameFinished;
	}

	private void btnStart_Click(object sender, EventArgs e)
	{
		if (rbEnemyHuman.Checked)
			StartLocalMatch();
		else if (rbEnemyComputer.Checked)
			StartComputerMatch();
		else if (rbEnemyOnline.Checked)
			StartOnlineMatch();

		tlpStartSettings.Enabled = false;
	}

	private void StartLocalMatch()
	{
		var start = rbStartP1.Checked
							? Player.Player1
							: Player.Player2;

		boardView1.SetP2Computer(false, null);
		boardView1.SetHandler(new LocalBoardHandler());
		boardView1.Start(start);
	}
	private void StartComputerMatch()
	{
		var start = rbStartP1.Checked
							? Player.Player1
							: Player.Player2;

		MoveCalculator moveCalculator = rbDifficultyEasy.Checked
													? new EasyMoveCalculator()
													: rbDifficultyNormal.Checked
																		  ? new NormalMoveCalculator()
																		  : new ImpossibleMoveCalculator();
		boardView1.SetP2Computer(true, moveCalculator);
		boardView1.SetHandler(new LocalBoardHandler());
		boardView1.Start(start);
	}
	private void StartOnlineMatch()
	{
		boardView1.SetHandler(new RemoteBoardHandler("https://localhost:7135/tictactoehub"));
		boardView1.Start(Player.Player1);
	}


	private void OnGameFinished(object? sender, Result winner)
	{
		tlpStartSettings.Enabled = true;
	}

	private void btnSettings_Click(object sender, EventArgs e)
	{
		var frm = new SettingsViewer();
		frm.ShowDialog();
	}

	private void rbEnemyComputer_CheckedChanged(object? sender, EventArgs e)
	{
		if (rbEnemyComputer.Checked)
		{
			rbStartP2.Text = "Computer";
			gbStart.Enabled = true;
			gbDifficulty.Enabled = true;
		}
	}
	private void rbEnemyOnline_CheckedChanged(object sender, EventArgs e)
	{
		if (rbEnemyOnline.Checked)
		{
			gbStart.Enabled = false;
			rbStartP2.Text = "Spieler 2";
			rbStartP1.Checked = true;
			gbDifficulty.Enabled = false;
		}
	}

	private void rbEnemyHuman_CheckedChanged(object sender, EventArgs e)
	{
		if (rbEnemyHuman.Checked)
		{
			gbDifficulty.Enabled = false;
			rbStartP2.Text = "Spieler 2";
			gbStart.Enabled = true;
		}
	}

}
