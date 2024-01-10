using TikTakToe.Lib.Enums;
using TikTakToe.Lib.Models;

namespace TikTakToe.Lib.Board;

public abstract class BoardHandler
{
	protected ShallowBord _board = new();
	public virtual ShallowBord GetBoard()
		=> new ShallowBord(_board);


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

	public abstract bool MakeMove(Move moove);
	protected virtual bool ValidMove(Move move)
	{
		return IsPlaying &&
			   move.Player != Player.NoOne &&
			   move.Player == NextTurn &&
			   move.Field is >= 1 and <= 9 &&
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
