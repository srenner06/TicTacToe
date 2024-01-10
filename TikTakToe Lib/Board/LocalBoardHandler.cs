using TikTakToe.Lib.Enums;
using TikTakToe.Lib.Models;
using TikTakToe.Lib.MoveCalculators;

namespace TikTakToe.Lib.Board;

public class LocalBoardHandler : BoardHandler
{
	public record LocalResult(Player Winner, bool P2WasComputer) : Result(Winner);
	public bool P2IsComputer { get; private set; } = false;
	private MoveCalculator? _moveCalculator;

	public override void Start(Player startplayer)
	{
		base.Start(startplayer);
		if (startplayer == Player.Player2 && P2IsComputer)
			MakeComputerMove();
	}

	public void SetP2Computer(bool p2IsComputer, MoveCalculator? moveCalculator)
	{
		if (!p2IsComputer)
		{
			P2IsComputer = false;
			_moveCalculator = null;
		}
		else
		{
			P2IsComputer = true;
			ArgumentNullException.ThrowIfNull(moveCalculator);
			_moveCalculator = moveCalculator;
		}
	}

	public override bool MakeMove(Move move)
	{
		if (!ValidMove(move))
			return false;

		_board.SetMove(move);

		var winner = _board.CheckWin();
		if (winner != Player.NoOne || !_board.GetFreeFields().Any())
		{
			IsPlaying = false;
			NextTurn = Player.NoOne;
			RaiseUpdatedBoard();
			RaiseFinished(new LocalResult(winner, P2IsComputer));
		}
		else
		{
			NextTurn = move.Player == Player.Player1 ? Player.Player2 : Player.Player1;
			RaiseUpdatedBoard();

			if (NextTurn == Player.Player2 && P2IsComputer)
				MakeComputerMove();
		}

		return true;
	}

	private void MakeComputerMove()
	{
		var bestMove = _moveCalculator!.CalculateBestMove(_board, Player.Player2);
		MakeMove(bestMove);
	}
}
