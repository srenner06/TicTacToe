using Utils.Extensions;
using TikTakToe.Lib.Enums;
using TikTakToe.Lib.Models;

namespace TikTakToe.Lib.MoveCalculators;
public class NormalMoveCalculator : MoveCalculator
{
	private static readonly int[] CornerFields = [1,3,7,9];
	public override Move CalculateBestMove(ShallowBord board, Player player)
	{
		var freeFields = board.GetFreeFields().ToArray();

		if (freeFields.Length == 0)
			return new Move(player, 0);

		var fieldToVictory = board.GetPossibleWin(player);
		if (fieldToVictory != 0)
			return new Move(player, fieldToVictory);

		var fieldToDefeat = board.GetPossibleWin(player == Player.Player1 ? Player.Player1 : Player.Player2);
		if (fieldToDefeat != 0)
			return new Move(player, fieldToDefeat);

		// Center
		if (freeFields.Contains(5))
			return new Move(player, 5);

		var freeCorners = freeFields.Where(CornerFields.Contains).ToList();
		if (freeCorners.Count > 0)
			return new Move(player, freeCorners.GetRandomElement());

		// Just something random
		return new Move(player, freeFields.GetRandomElement());
	}
}