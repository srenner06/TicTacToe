using Utils.Extensions;

namespace TicTacToe.Lib.Models;

public sealed class RemotePlayer
{
	public readonly string Id;
	public readonly string ConnectionId;

	public RemotePlayer(string connectionId)
	{
		if (connectionId.IsEmpty())
			throw new ArgumentException("ConnectionId cannot be empty", nameof(connectionId));

		Id = Guid.NewGuid().ToString();
		ConnectionId = connectionId;
	}
}
