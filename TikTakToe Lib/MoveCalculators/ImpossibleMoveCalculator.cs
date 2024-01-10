using TikTakToe.Lib.Enums;
using TikTakToe.Lib.Models;

namespace TikTakToe.Lib.MoveCalculators;

public class ImpossibleMoveCalculator : MoveCalculator
{
	public override Move CalculateBestMove(ShallowBord board, Player player)
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

	private MoveScore FindBestMove(ShallowBord board, Player player, int count = 0)
	{
		var bestMoveScore = new MoveScore(0, player == Player.Player2 ? int.MinValue : int.MaxValue);

		foreach (var move in board.GetFreeFields())
		{
			board.SetByNum(move, player);
			var score = GetScore(board, player, count);
			board.SetByNum(move, Player.NoOne, true);

			if (player == Player.Player2 && score > bestMoveScore.Score ||
				player == Player.Player1 && score < bestMoveScore.Score)
			{
				bestMoveScore = new MoveScore(move, score);
			}
		}

		return bestMoveScore;
	}

	private int GetScore(ShallowBord board, Player player, int count)
	{
		var winner = board.CheckWin();

		return winner != Player.NoOne
			? winner == Player.Player2 ? ComputerWin : PlayerWin
			: board.GetFreeFields().Any() == false
			? Draw
			: player == Player.Player2
			? FindBestMove(board, Player.Player1, count + 1).Score
			: FindBestMove(board, Player.Player2, count + 1).Score;
	}
}