namespace TicTacToe.Lib.Models;

public sealed class Game
{
	public readonly string Id;
	public RemotePlayer? Player1 { get; private set; }
	public RemotePlayer? Player2 { get; private set; }
	public readonly Board Board;
	public string CurrentTurnPlayerId { get; private set; }

	public bool ReadyToStart => Player1 != null && Player2 != null;

	public bool AddPlayer(RemotePlayer player)
	{
		if (player == Player1 || player == Player2)
			return false;

		if (Player1 == null)
		{
			Player1 = player;
			return true;
		}
		if (Player2 == null)
		{
			Player2 = player;
			CurrentTurnPlayerId = Random.Shared.Next(0, 2) == 0 ? Player1.Id : Player2.Id;
			return true;
		}
		return false;
	}

	public Game()
	{
		Id = Guid.NewGuid().ToString();
		Board = new Board();
		CurrentTurnPlayerId = "";
	}
	public Game(RemotePlayer p1, RemotePlayer p2)
	{
		if (p1 == p2 || p1.Id == p2.Id || p1.ConnectionId == p2.ConnectionId)
			throw new ArgumentException("Player1 and Player2 cannot be the same Player, have the same Id or ConnectionId", nameof(p2));

		Id = Guid.NewGuid().ToString();
		Player1 = p1;
		Player2 = p2;
		Board = new Board();
		CurrentTurnPlayerId = p1.Id;
	}

	public void SwitchTurn()
	{
		CurrentTurnPlayerId = GetOpponentPlayerId(CurrentTurnPlayerId);
	}

	public string GetOpponentPlayerId(string connectionId)
	{
		if (ReadyToStart == false)
			return "";

		if (connectionId == Player1!.Id)
			return Player2!.Id;
		if (connectionId == Player2!.Id)
			return Player1.Id;
		return "";
	}
}