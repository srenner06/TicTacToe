using TicTacToe.Lib.Enums;
using TicTacToe.Lib.Models;

namespace TicTacToe.Lib.BoardHandlers;

public abstract class BoardHandler
{
	protected Board _board = new();
	public virtual Board GetBoard()
		=> new Board(_board);


	public Player NextTurn { get; protected set; }
	public bool IsPlaying { get; protected set; } = false;

	public virtual void Start(Player startplayer)
	{
		IsPlaying = false;
		if (startplayer is Player.NoOne or default(Player))
			return;

		_board = new();
		NextTurn = startplayer;
		IsPlaying = true;
	}

	public abstract bool MakeMove(Move move);
	protected virtual bool ValidMove(Move move)
	{
		return IsPlaying &&
			   move.IsValid &&
			   move.Player == NextTurn &&
			   _board.GetByNum(move.Field) == Player.NoOne;
	}

	protected void RaiseUpdatedBoard()
		=> UpdatedBoard?.Invoke(this, EventArgs.Empty);
	protected void RaiseFinished(Result result)
		=> Finished?.Invoke(this, result);

	public event EventHandler? UpdatedBoard;
	public event EventHandler<Result>? Finished;

	public abstract record Result(Player Winner);

}
