using TikTakToe.Lib.Enums;
using TikTakToe.Lib.Models;
using Utils.Extensions;

namespace TikTakToe.Lib.MoveCalculators;
public class EasyMoveCalculator : MoveCalculator
{
	public override Move CalculateBestMove(ShallowBord board, Player player)
	{
		var freeFields = board.GetFreeFields();
		var field = !freeFields.Any() ? 0 : freeFields.GetRandomElement();
		return new Move(player, field);
	}
}