using TikTakToe.Lib.Models;

namespace TikTakToe.Api.Services;

public class MatchmakingService
{
	private static List<RemotePlayer> _players = new List<RemotePlayer>();
	private static List<Game> games = new List<Game>();
	public event EventHandler<Game>? MadeMatch;
	private static readonly object _lock = new();
	public RemotePlayer AddPlayer(string connectionId)
	{
		var player = new RemotePlayer { Id = Guid.NewGuid().ToString(), ConnectionId = connectionId };

		lock (_lock)
			_players.Add(player);


		MatchPlayers();
		return player;
	}

	private void MatchPlayers()
	{
		lock (_lock)
			while (_players.Count % 2 == 0 && _players.Count > 0)
			{
				var players = _players.Take(2).ToArray();
				_players.RemoveRange(0, 2);
				var p1 = players[0];
				var p2 = players[1];

				var game = new Game() { Id = Guid.NewGuid().ToString() };
				(game.Player1, game.Player2) = Random.Shared.Next(0, 2) == 0
														? (p1, p2)
														: (p2, p1);

				game.CurrentTurnPlayerId = game.Player1.Id;
				games.Add(game);

				MadeMatch?.Invoke(this, game);
			}
	}

	public bool RemovePlayer(string playerId)
	{
		var result = false;
		lock (_lock)
		{
			var player = _players.FirstOrDefault(p => p.Id == playerId);
			if (player is not null)
			{
				_players.Remove(player);
				result = true;
			}

		}
		return result;
	}

	public Game? GetGame(string gameId)
	{
		return games.FirstOrDefault(g => g.Id == gameId);
	}
	public bool RemoveGame(string gameId)
	{
		var result = false;
		lock (_lock)
		{
			var game = games.FirstOrDefault(g => g.Id == gameId);
			if (game is not null)
			{
				games.Remove(game);
				result = true;
			}
		}
		return result;
	}
}
