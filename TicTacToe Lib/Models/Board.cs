using TicTacToe.Lib.Enums;

namespace TicTacToe.Lib.Models;

public sealed class Board
{
	private readonly Player[] _fields = new Player[9];
	public Player[] GetFields()
	{
		var temp = new Player[_fields.Length];
		_fields.CopyTo(temp, 0);
		return temp;
	}

	public Player Field1 => _fields[0];
	public Player Field2 => _fields[1];
	public Player Field3 => _fields[2];
	public Player Field4 => _fields[3];
	public Player Field5 => _fields[4];
	public Player Field6 => _fields[5];
	public Player Field7 => _fields[6];
	public Player Field8 => _fields[7];
	public Player Field9 => _fields[8];

	public Board()
	{
		Array.Fill(_fields, Player.NoOne);
	}

	public Board(Board original)
	{
		original._fields.CopyTo(_fields, 0);
	}

	public Board(Player[] _fields)  // Change parameter name to match field name
	{
		if (_fields.Length != 9)
			throw new ArgumentException($"{nameof(_fields)} must have a Length of 9", nameof(_fields));
		_fields.CopyTo(this._fields, 0);
	}

	public int GetPossibleWin(Player player)
	{
		if (CheckPossibleWin(Field1, Field2, Field3, player))
			return Field1 == Player.NoOne ? 1 : Field2 == Player.NoOne ? 2 : 3;

		if (CheckPossibleWin(Field4, Field5, Field6, player))
			return Field4 == Player.NoOne ? 4 : Field5 == Player.NoOne ? 5 : 6;

		if (CheckPossibleWin(Field7, Field8, Field9, player))
			return Field7 == Player.NoOne ? 7 : Field8 == Player.NoOne ? 8 : 9;

		if (CheckPossibleWin(Field1, Field4, Field7, player))
			return Field1 == Player.NoOne ? 1 : Field4 == Player.NoOne ? 4 : 7;

		if (CheckPossibleWin(Field2, Field5, Field8, player))
			return Field2 == Player.NoOne ? 2 : Field5 == Player.NoOne ? 5 : 8;

		if (CheckPossibleWin(Field3, Field6, Field9, player))
			return Field3 == Player.NoOne ? 3 : Field6 == Player.NoOne ? 6 : 9;

		if (CheckPossibleWin(Field1, Field5, Field9, player))
			return Field1 == Player.NoOne ? 1 : Field5 == Player.NoOne ? 5 : 9;

		if (CheckPossibleWin(Field3, Field5, Field7, player))
			return Field3 == Player.NoOne ? 3 : Field5 == Player.NoOne ? 5 : 7;

		return 0; // No possible win
	}

	private bool CheckPossibleWin(Player f1, Player f2, Player f3, Player player)
	{
		var opposite = player != Player.Player1 ? Player.Player1 : Player.Player2;

		if (f1 == opposite || f2 == opposite || f3 == opposite)
			return false;

		var count = 0;
		if (f1 == player)
			count++;
		if (f2 == player)
			count++;
		if (f3 == player)
			count++;

		return count >= 2;
	}

	public Player CheckWin()
	{
		var row1 = (Field1, Field2, Field3);
		var row2 = (Field4, Field5, Field6);
		var row3 = (Field7, Field8, Field9);

		var col1 = (Field1, Field4, Field7);
		var col2 = (Field2, Field5, Field8);
		var col3 = (Field3, Field6, Field9);

		var diagonal1 = (Field1, Field5, Field9);
		var diagonal2 = (Field3, Field5, Field7);

		(Player F1, Player F2, Player F3)[] possibleWins = [row1, row2, row3, col1, col2, col3, diagonal1, diagonal2];

		var win = possibleWins.FirstOrDefault(w => w.F1 == w.F2 && w.F2 == w.F3 && w.F1 != Player.NoOne);
		return win != default ? win.F1 : Player.NoOne;
	}

	public IEnumerable<int> GetFreeFields()
	{
		for (var i = 0; i < _fields.Length; i++)
		{
			var val = _fields[i];
			if (val == Player.NoOne)
				yield return i + 1;
		}
	}

	public Player GetByNum(int i)
	{
		var index = i - 1;
		return _fields[index];
	}
	private void MakeMove(Move move)
	{
		_fields[move.Field] = move.Player;
	}

	public bool TryMakeMove(Move move)
	{
		if (move.IsValid == false)
			return false;

		var field = GetByNum(move.Field);
		if (field == Player.NoOne)
			return false;

		MakeMove(move);
		return true;
	}
}
