using TicTacToe.Lib.Board;
using TicTacToe.Lib.Enums;
using TicTacToe.Lib.MoveCalculators;
using TicTacToe_WIn.Helpers;
using Utils.Windows.Forms;
using Utils.Windows.Helpers;
using static TicTacToe.Lib.Board.BoardHandler;
using static TicTacToe.Lib.Board.LocalBoardHandler;
using static TicTacToe.Lib.Board.RemoteBoardHandler;

namespace TikTakToe;
public partial class BoardView : UserControl
{
	private const string NotStartedText = "Spiel noch nicht gestartet";
	private const string StartedText = "Spiel läuft";
	private const string FinishedText = "Spiel beendet. Sieger: {0}";

	public event EventHandler<Result>? Finished;

	private BoardHandler? _boardHandler;

	public Color P1Color { get; private set; } = Color.Blue;
	public Color P2Color { get; private set; } = Color.Red;
	public Color NoPColor { get; private set; } = Color.WhiteSmoke;
	public BoardView()
	{
		InitializeComponent();
		lblStatus.Text = NotStartedText;

		foreach (var pb in GetControlHierarchy(this).OfType<PictureBox>())
			pb.Click += OnClick;
	}
	private IEnumerable<Control> GetControlHierarchy(Control root)
	{
		var queue = new Queue<Control>();

		queue.Enqueue(root);

		do
		{
			var control = queue.Dequeue();

			yield return control;

			foreach (var child in control.Controls.OfType<Control>())
				queue.Enqueue(child);

		} while (queue.Count > 0);

	}

	public void SetHandler(BoardHandler handler)
	{
		_boardHandler = handler;
		handler.UpdatedBoard += (sender, e) => SyncViewToBoard();
		handler.Finished += OnFinished;
		if (handler is RemoteBoardHandler remote)
		{
			remote.FoundOpponent += Remote_OnFoundOpponent;
			remote.JoinedMatchmaking += Remote_OnJoinedMatchmaking;
		}
	}
	private LoadingDialog _loadingDialog = new();
	private void Remote_OnJoinedMatchmaking(object? sender, EventArgs e)
	{
		_loadingDialog = new();
		_loadingDialog.Canceled += Remote_OnCancelMatchmaking;
		_loadingDialog.SetText("Gegner wird gesucht ...");
		_loadingDialog.StartPosition = FormStartPosition.CenterParent;
		_loadingDialog.Show(this);
	}
	private void Remote_OnFoundOpponent(object? sender, EventArgs e)
	{
		_loadingDialog.Close();
	}
	private void Remote_OnCancelMatchmaking(object? sender, EventArgs e)
	{
		var remote = (_boardHandler as RemoteBoardHandler)!;
		remote.LeaveMatchmaking();
		_loadingDialog.Close();
	}
	private void Remote_OnOpponentLeft(object? snder, EventArgs e)
	{
		Msg.Error("Der Gegner hat das Spiel verlassen");
	}
	public void SetP2Computer(bool p2IsComputer, MoveCalculator? moveCalculator)
	{
		if (_boardHandler is LocalBoardHandler handler)
			handler.SetP2Computer(p2IsComputer, moveCalculator);
	}
	private void LoadSettings()
	{
		var settings = Settings.Load();
		P1Color = settings.P1Color;
		P2Color = settings.P2Color;
		NoPColor = settings.NoPColor;
	}

	public void Start(Player firstTurn)
	{
		if (firstTurn == Player.NoOne)
			return;

		if (_boardHandler is null)
			return;

		Enabled = true;
		lblStatus.Text = StartedText;
		LoadSettings();

		_boardHandler.Start(firstTurn);
		SyncViewToBoard();
	}

	private void SyncViewToBoard()
	{
		var board = _boardHandler?.GetBoard() ?? new ShallowBord();
		pb1.BackColor = GetPlayerColor(board.Field1);
		pb2.BackColor = GetPlayerColor(board.Field2);
		pb3.BackColor = GetPlayerColor(board.Field3);
		pb4.BackColor = GetPlayerColor(board.Field4);
		pb5.BackColor = GetPlayerColor(board.Field5);
		pb6.BackColor = GetPlayerColor(board.Field6);
		pb7.BackColor = GetPlayerColor(board.Field7);
		pb8.BackColor = GetPlayerColor(board.Field8);
		pb9.BackColor = GetPlayerColor(board.Field9);
	}
	private void OnFinished(object? sender, Result result)
	{
		lblStatus.SafeInvoke(() => lblStatus.Text = String.Format(FinishedText, result.Winner.ToString()));
		this.SafeInvoke(() => this.Enabled = false);

		if (result is RemoteResult remoteResult)
			HandleRemoteResult(remoteResult);
		else if (result is LocalResult localResult)
			HandleLocalResult(localResult);

		Finished?.Invoke(sender, result);
	}
	private static void HandleRemoteResult(RemoteResult result)
	{
		if (result.OpponentLeft)
			Msg.Error("Der Gegner hat das Spiel verlassen");
		else if (result.Canceled)
			return;
		else
		{
			string text;
			string caption;
			if (result.Winner == Player.NoOne)
			{
				text = "Keiner hat gewonnen";
				caption = "Unentschieden";
			}
			else if (result.Winner == result.MyPlayer)
			{
				text = "Sie haben gewonnen";
				caption = "Sieg";
			}
			else
			{
				text = "Sie haben verloren";
				caption = "Niederlage";
			}
			Msg.Warning(text, caption);
		}

	}
	private static void HandleLocalResult(LocalResult result)
	{
		string text;
		string caption;

		if (result.Winner == Player.NoOne)
		{
			text = "Keiner hat gewonnen";
			caption = "Unentschieden";
		}
		else
		{
			if (result.P2WasComputer)
			{
				if (result.Winner == Player.Player1)
				{
					text = "Sie haben gewonnen";
					caption = "Sieg";
				}
				else
				{
					text = "Sie haben verloren";
					caption = "Niederlage";
				}
			}
			else
			{
				text = $"Spieler {(int)result.Winner} hat gewonnen";
				caption = "Sieg";
			}
		}
		Msg.Warning(text, caption);
	}

	private Color GetPlayerColor(Player player)
	{
		// TODO: Load from ini
		return player switch
		{
			Player.Player1 => P1Color,
			Player.Player2 => P2Color,
			Player.NoOne or _ => NoPColor
		};
	}

	private void OnClick(object? sender, EventArgs e)
	{
		if (_boardHandler is null)
			return;

		var field = GetField(sender);
		if (field == 0)
			return;

		var player = _boardHandler.NextTurn;
		if (_boardHandler is LocalBoardHandler localHandler && localHandler.P2IsComputer)
			player = Player.Player1; // On local against the computer all clicks are from the player 1
		else if (_boardHandler is RemoteBoardHandler remoteHandler)
			player = remoteHandler.MyPlayer;

		var move = new Move(player, field);
		_boardHandler?.MakeMove(move);
	}

	private int GetField(object? pb)
	{
		if (pb == pb1)
			return 1;
		else if (pb == pb2)
			return 2;
		else if (pb == pb3)
			return 3;
		else if (pb == pb4)
			return 4;
		else if (pb == pb5)
			return 5;
		else if (pb == pb6)
			return 6;
		else if (pb == pb7)
			return 7;
		else if (pb == pb8)
			return 8;
		else if (pb == pb9)
			return 9;

		return 0;
	}

	private void BoardView_EnabledChanged(object sender, EventArgs e)
	{
		foreach (var control in GetControlHierarchy(this))
			control.Enabled = this.Enabled;
	}
}
