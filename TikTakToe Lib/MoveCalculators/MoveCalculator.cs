using TikTakToe.Lib.Enums;
using TikTakToe.Lib.Models;

namespace TikTakToe.Lib.MoveCalculators;
public abstract class MoveCalculator
{
	public abstract Move CalculateBestMove(ShallowBord board, Player player);
}
