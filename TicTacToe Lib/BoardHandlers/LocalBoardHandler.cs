﻿using TicTacToe.Lib.Enums;
using TicTacToe.Lib.Models;
using TicTacToe.Lib.MoveCalculators;

namespace TicTacToe.Lib.BoardHandlers;

public sealed class LocalBoardHandler : BoardHandler
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

		var valid = _board.TryMakeMove(move);
		if (!valid)
			return false;

		if (_board.CheckIsFinished(out var winner))
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
