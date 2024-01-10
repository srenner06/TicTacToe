namespace TicTacToe.Lib.Models;

public class Game
{
	public string Id { get; set; }
	public RemotePlayer Player1 { get; set; }
	public RemotePlayer Player2 { get; set; }
	public ShallowBord Board { get; set; } = new ShallowBord();
	public string CurrentTurnPlayerId { get; set; }
}