using System.Collections.Concurrent;
using System.Text.Json.Serialization;
using Utils.Extensions;

namespace TicTacToe.Lib.Models;

public sealed record RemotePlayer
{
	public readonly string Id;
	public readonly string ConnectionId;

	private static ConcurrentBag<RemotePlayer> _players = new();

	[JsonConstructor]
	public RemotePlayer(string connectionId, string id = "")
	{
		if (connectionId.IsEmpty())
			throw new ArgumentException("ConnectionId cannot be empty", nameof(connectionId));

		if (id.IsEmpty())
			id = Guid.NewGuid().ToString();

		Id = id;
		ConnectionId = connectionId;

		_players.Add(this);
	}

	public static RemotePlayer? GetByConnectionId(string connId)
	{
		return _players.FirstOrDefault(p => p.ConnectionId == connId);
	}
	public static RemotePlayer? GetById(string id)
	{
		return _players.FirstOrDefault(p => p.Id == id);
	}
}
