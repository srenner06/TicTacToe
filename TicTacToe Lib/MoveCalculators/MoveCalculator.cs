using TicTacToe.Lib.Enums;
using TicTacToe.Lib.Models;

namespace TicTacToe.Lib.MoveCalculators;
public abstract class MoveCalculator
{
	public abstract Move CalculateBestMove(ShallowBord board, Player player);
}
