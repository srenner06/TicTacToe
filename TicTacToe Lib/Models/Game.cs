namespace TicTacToe.Lib.Models;

public sealed class Game
{
	public readonly string Id;
	public readonly RemotePlayer Player1;
	public readonly RemotePlayer Player2;
	public readonly Board Board;
	public string CurrentTurnPlayerId { get; private set; }

	public Game(RemotePlayer p1, RemotePlayer p2, bool? p1Starts = null)
	{
		if (p1 == p2 || p1.Id == p2.Id || p1.ConnectionId == p2.ConnectionId)
			throw new ArgumentException("Player1 and Player2 cannot be the same Player, have the same Id or ConnectionId", nameof(p2));

		p1Starts ??= Random.Shared.Next(0, 2) == 0;
		Id = Guid.NewGuid().ToString();
		Player1 = p1;
		Player2 = p2;
		Board = new Board();
		CurrentTurnPlayerId = p1Starts.Value ? p1.Id : p2.Id;
	}

	public void SwitchTurn()
	{
		CurrentTurnPlayerId = CurrentTurnPlayerId == Player1.Id ? Player2.Id : Player1.Id;
	}
}