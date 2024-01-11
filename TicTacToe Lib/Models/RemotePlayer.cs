using System.Text.Json.Serialization;
using Utils.Extensions;

namespace TicTacToe.Lib.Models;

public sealed class RemotePlayer
{
	public readonly string Id;
	public readonly string ConnectionId;

	[JsonConstructor]
	public RemotePlayer(string connectionId, string id = "")
	{
		if (connectionId.IsEmpty())
			throw new ArgumentException("ConnectionId cannot be empty", nameof(connectionId));

		if (id.IsEmpty())
			id = Guid.NewGuid().ToString();

		Id = id;
		ConnectionId = connectionId;
	}
}
