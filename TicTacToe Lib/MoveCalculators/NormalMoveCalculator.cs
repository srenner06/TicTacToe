using TicTacToe.Lib.Enums;
using TicTacToe.Lib.Models;
using Utils.Extensions;

namespace TicTacToe.Lib.MoveCalculators;
public sealed class NormalMoveCalculator : MoveCalculator
{
	private static int[] _cornerFields => [1, 3, 7, 9];
	public override Move CalculateBestMove(Models.Board board, Player player)
	{
		var freeFields = board.GetFreeFields().ToArray();

		if (freeFields.Length == 0)
			return new Move(player, 0);

		var fieldToVictory = board.GetPossibleWin(player);
		if (fieldToVictory != 0)
			return new Move(player, fieldToVictory);

		var fieldToDefeat = board.GetPossibleWin(player == Player.Player1 ? Player.Player2 : Player.Player1);
		if (fieldToDefeat != 0)
			return new Move(player, fieldToDefeat);

		// Center
		if (freeFields.Contains(5))
			return new Move(player, 5);

		var freeCorners = freeFields.Where(_cornerFields.Contains).ToList();
		if (freeCorners.Count > 0)
			return new Move(player, freeCorners.GetRandomElement());

		// Just something random
		return new Move(player, freeFields.GetRandomElement());
	}
}