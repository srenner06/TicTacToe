using TicTacToe.Lib.Enums;
using TicTacToe.Lib.Models;

namespace TicTacToe.Lib.MoveCalculators;

public sealed class ImpossibleMoveCalculator : MoveCalculator
{
	public override Move CalculateBestMove(Board board, Player player)
	{
		var freeFields = board.GetFreeFields();
		if (freeFields.Any() == false)
			return new Move(player, 0); // No free Fields

		//Check if Player can win
		var fieldToVictory = board.GetPossibleWin(player);
		if (fieldToVictory != 0)
			return new Move(player, fieldToVictory);

		//Check if other Player can win
		var fieldToDefeat = board.GetPossibleWin(player == Player.Player1 ? Player.Player2 : Player.Player1);
		if (fieldToDefeat != 0)
			return new Move(player, fieldToDefeat);

		var field = FindBestMove(board, player).Field;
		return new Move(player, field);
	}

	private const int ComputerWin = 1;
	private const int PlayerWin = -1;
	private const int Draw = 0;

	private record MoveScore(int Field, int Score);

	private MoveScore FindBestMove(Board board, Player player, int count = 0)
	{
		var bestMoveScore = new MoveScore(0, player == Player.Player2 ? int.MinValue : int.MaxValue);

		foreach (var field in board.GetFreeFields())
		{
			var copy = new Board(board);
			var move = new Move(player, field);
			copy.TryMakeMove(move);
			var score = GetScore(copy, player, count);

			if ((player == Player.Player2 && score > bestMoveScore.Score) ||
				(player == Player.Player1 && score < bestMoveScore.Score))
			{
				bestMoveScore = new MoveScore(field, score);
			}
		}

		return bestMoveScore;
	}

	private int GetScore(Board board, Player player, int count)
	{
		var winner = board.GetWinner();

		return winner switch
		{
			Player.Player1 => PlayerWin,
			Player.Player2 => ComputerWin,
			_ when board.GetFreeFields().Any() == false => Draw,
			_ when player == Player.Player1 => FindBestMove(board, Player.Player2, count + 1).Score,
			_ => FindBestMove(board, Player.Player1, count + 1).Score
		};
	}
}