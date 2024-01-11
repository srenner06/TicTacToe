using TicTacToe.Lib.Models;

namespace TicTacToe.Api.Services;

public class MatchmakingService
{
	private static readonly List<RemotePlayer> _players = [];
	private static readonly List<Game> _games = [];
	public event EventHandler<Game>? MadeMatch;
	private static readonly object _lock = new();
	public RemotePlayer AddPlayer(string connectionId)
	{
		var player = new RemotePlayer(connectionId);

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

				if (Random.Shared.Next(0, 2) == 1)
					(p1, p2) = (p2, p1);

				var game = new Game(p1, p2);
				_games.Add(game);

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
		return _games.FirstOrDefault(g => g.Id == gameId);
	}
	public bool RemoveGame(string gameId)
	{
		var result = false;
		lock (_lock)
		{
			var game = _games.FirstOrDefault(g => g.Id == gameId);
			if (game is not null)
			{
				_games.Remove(game);
				result = true;
			}
		}
		return result;
	}
}
