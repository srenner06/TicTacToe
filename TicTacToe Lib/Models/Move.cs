using System.Text.Json.Serialization;
using TicTacToe.Lib.Enums;

namespace TicTacToe.Lib.Models;

public sealed record Move
{
	public readonly bool IsValid;
	public readonly int Field;
	public readonly Player Player;
	[JsonConstructor]
	public Move(Player player, int field)
	{
		this.Player = player;
		this.Field = field;

		IsValid = Player != Player.NoOne && (Field is >= 1 and <= 9);
	}
}