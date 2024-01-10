using TicTacToe.Lib.Enums;
using TicTacToe.Lib.Models;
using Utils.Extensions;

namespace TicTacToe.Lib.MoveCalculators;
public class EasyMoveCalculator : MoveCalculator
{
	public override Move CalculateBestMove(ShallowBord board, Player player)
	{
		var freeFields = board.GetFreeFields();
		var field = !freeFields.Any() ? 0 : freeFields.GetRandomElement();
		return new Move(player, field);
	}
}