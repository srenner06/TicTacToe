using TicTacToe.Lib.BoardHandlers;
using TicTacToe.Lib.Enums;
using TicTacToe.Lib.MoveCalculators;
using TicTacToe.Win.Helpers;
using Utils.Extensions;
using Utils.Windows.Extensions;
using Utils.Windows.Forms;
using Utils.Windows.Helpers;
using static TicTacToe.Lib.BoardHandlers.BoardHandler;
using static TicTacToe.Lib.BoardHandlers.LocalBoardHandler;
using static TicTacToe.Lib.BoardHandlers.RemoteBoardHandler;

namespace TicTacToe.Win.Board;
public partial class BoardView : UserControl
{
	enum State
	{
		Default = 0,
		Started = 1,
		Finished = 2,
		SearchingOpponent = 3,
	}

	public event EventHandler<Result>? Finished;

	private BoardHandler? _boardHandler;
	private State _state;
	private State CurrentState
	{
		get => _state;
		set
		{
			_state = value;
			UpdateText();
		}
	}

	public Color P1Color { get; private set; } = Color.Blue;
	public Color P2Color { get; private set; } = Color.Red;
	public Color NoPColor { get; private set; } = Color.WhiteSmoke;
	public BoardView()
	{
		InitializeComponent();
		CurrentState = State.Default;

		foreach (var pb in GetControlHierarchy(this).OfType<PictureBox>())
			pb.Click += OnClick;
	}
	private string GetStartedText()
	{
		return "";
	}
	private void UpdateText()
	{
		var text = "";
		switch (CurrentState)
		{
			case State.Default:
				text = "Spiel noch nicht gestartet";
				break;
			case State.Finished:
				var winner = _boardHandler?.GetBoard().GetWinner() ?? Player.NoOne;
				if (winner == Player.NoOne)
					text = "Unentschieden";
				else
				{
					if (_boardHandler is RemoteBoardHandler remote)
					{
						if (remote.MyPlayer == winner)
							text = "Sie haben gewonnen";
						else
							text = "Der Gegner hat gewonnen";
					}
					else if (_boardHandler is LocalBoardHandler local)
					{
						if (local.P2IsComputer == false)
							text = $"Spieler {(int)local.NextTurn} hat gewonnen";
						else if (winner == Player.Player1)
							text = "Sie haben gewonnen";
						else if (winner == Player.Player2)
							text = "Der Computer hat gewonnen";
					}
				}
				break;
			case State.Started:
				if (_boardHandler is RemoteBoardHandler r)
				{
					if (r.MyPlayer == r.NextTurn)
						text = "Sie sind am Zug";
					else
						text = "Der Gegner ist am Zug";
				}
				else if (_boardHandler is LocalBoardHandler local)
				{
					if (local.P2IsComputer == false)
						text = $"Spieler {(int)local.NextTurn} ist am Zug";
					else if (local.NextTurn == Player.Player1)
						text = "Sie sind am Zug";
					else if (local.NextTurn == Player.Player2)
						text = "Der Computer ist am Zug";
				}
				break;
			case State.SearchingOpponent:
				text = "Gegner wird gesucht ...";
				break;
			default:
				throw new NotImplementedException();
		}
		lblStatus.SafeInvoke(() => lblStatus.Text = text);
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
		if (_boardHandler is { })
		{
			_boardHandler.UpdatedBoard -= OnUpdateBoard;
			_boardHandler.Finished -= OnFinished;
			if (_boardHandler is RemoteBoardHandler r)
			{
				r.FoundOpponent -= Remote_OnFoundOpponent;
				r.JoinedMatchmaking -= Remote_OnJoinedMatchmaking;
			}
		}

		_boardHandler = handler;
		handler.UpdatedBoard += OnUpdateBoard;
		handler.Finished += OnFinished;
		if (handler is RemoteBoardHandler remote)
		{
			remote.FoundOpponent += Remote_OnFoundOpponent;
			remote.JoinedMatchmaking += Remote_OnJoinedMatchmaking;
		}

		void OnUpdateBoard(object? sender, EventArgs e)
		{
			SyncViewToBoard();
			UpdateText();
		}
	}
	private LoadingDialog _loadingDialog = new();
	private void Remote_OnJoinedMatchmaking(object? sender, EventArgs e)
	{
		CurrentState = State.SearchingOpponent;
		_loadingDialog = new();
		_loadingDialog.Canceled += Remote_OnCancelMatchmakingAsync;
		_loadingDialog.SetText("Gegner wird gesucht ...");
		_loadingDialog.StartPosition = FormStartPosition.CenterParent;
		_loadingDialog.ShowDialogOnNewThread(this.FindForm());
	}
	private void Remote_OnFoundOpponent(object? sender, EventArgs e)
	{
		CurrentState = State.Started;

		_loadingDialog.Close();
		var text = "Es wurde ein Gegner gefunden.\n";
		var iStart = (_boardHandler as RemoteBoardHandler)!.MyPlayer == Player.Player1;

		if (iStart)
			text += "Sie beginnen";
		else
			text += "Der Genger beginnt";

		var popUp = new PopUpForm(text);
		popUp.PopUp(owner: FindForm());
	}
	private async void Remote_OnCancelMatchmakingAsync(object? sender, EventArgs e)
	{
		var remote = (_boardHandler as RemoteBoardHandler)!;
		await remote.LeaveMatchmakingAsync();
		_loadingDialog.Close();
		CurrentState = State.Default;
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
		LoadSettings();

		_boardHandler.Start(firstTurn);
		if (CurrentState != State.SearchingOpponent)
			CurrentState = State.Started;
		SyncViewToBoard();
	}

	private void SyncViewToBoard()
	{
		var board = _boardHandler?.GetBoard() ?? new Lib.Models.Board();
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
		var newState = State.Finished;
		if (result is RemoteResult remoteResult)
		{
			HandleRemoteResult(remoteResult);
			if (remoteResult.Canceled)
				newState = State.Default;
		}
		else if (result is LocalResult localResult)
			HandleLocalResult(localResult);

		
		CurrentState = newState;
		this.SafeInvoke(() => Enabled = false);
		Finished?.Invoke(sender, result);
	}
	private void HandleRemoteResult(RemoteResult result)
	{
		if (result.OpponentLeft)
			Msg.Error("Der Gegner hat das Spiel verlassen", "Sieg", ParentForm);
		else if (result.Canceled) //dont show a message if you yourself canceled the game
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
			Msg.Info(text, caption, FindForm());
		}

	}
	private void HandleLocalResult(LocalResult result)
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
				caption = "";
			}
		}
		Msg.Info(text, caption, FindForm());
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
			control.Enabled = Enabled;
	}
}
