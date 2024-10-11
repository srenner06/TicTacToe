using System.Collections.Concurrent;
using TicTacToe.Lib.Models;

namespace TicTacToe.Api.Services;

public class MatchmakingService
{
	private static readonly ConcurrentDictionary<string, RemotePlayer> _players = [];
	private static readonly ConcurrentDictionary<string, Game> _waitingGames = [];
	private static readonly ConcurrentDictionary<string, Game> _runningGames = [];
	private static readonly object _lock = new();

	public (RemotePlayer, Game) AddPlayer(string connectionId)
	{
		var player = new RemotePlayer(connectionId);
		lock (_lock)
		{
			Game game;
			if (_waitingGames.IsEmpty)
			{
				game = new Game();
				game.AddPlayer(player);
				_waitingGames.TryAdd(game.Id, game);
				return (player, game);
			}

			var gameKey = _waitingGames.Keys.First();
			_waitingGames.TryRemove(gameKey, out game!);
			game.AddPlayer(player);
			_runningGames.TryAdd(game.Id, game);
			return (player, game);
		}
	}
	public bool RemovePlayerFromWaiting(string playerId)
	{
		lock (_lock)
		{
			var game = _waitingGames.Values.FirstOrDefault(g => g?.Player1?.Id == playerId || g?.Player2?.Id == playerId);
			if (game is null)
				return false;
			return _waitingGames.TryRemove(game.Id, out _);
		}
	}

	public Game? GetRunningGame(string gameId)
	{
		return _runningGames.TryGetValue(gameId, out var game) ? game : null;
	}
	public bool RemoveRunningGame(string gameId)
	{
		return _runningGames.TryRemove(gameId, out _);
	}

	public Game? GetRunningGameByPlayer(string connectionId)
	{
		return _runningGames.Values.FirstOrDefault(g => g?.Player1?.ConnectionId == connectionId || g?.Player2?.ConnectionId == connectionId);
	}
}
