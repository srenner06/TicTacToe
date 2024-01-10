using TicTacToe.Lib.Enums;
using TicTacToe.Lib.Models;

namespace TicTacToe.Lib.MoveCalculators;
public abstract class MoveCalculator
{
	public abstract Move CalculateBestMove(Models.Board board, Player player);
}
