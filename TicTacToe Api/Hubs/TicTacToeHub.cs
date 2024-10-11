using Microsoft.AspNetCore.SignalR;
using TicTacToe.Api.Services;
using TicTacToe.Lib.Enums;
using TicTacToe.Lib.Models;
using Utils.Extensions;

namespace TicTacToe.Api.Hubs;

public class TicTacToeHub(MatchmakingService matchmakingService) : Hub
{
#pragma warning disable IDE1006
	public override async Task OnDisconnectedAsync(Exception? exception)
	{
		var playerId = Context.ConnectionId;

		matchmakingService.RemovePlayerFromWaiting(playerId);
		var game = matchmakingService.GetRunningGameByPlayer(playerId);
		if (game is not null)
			await LeaveMatch(game.Id);

		await base.OnDisconnectedAsync(exception);
	}

	public async Task JoinMatchmaking()
	{
		var (player, game) = matchmakingService.AddPlayer(Context.ConnectionId);

		if (game.ReadyToStart)
			await OnMadeMatch(game);
	}

	public void LeaveMatchmaking()
	{
		var player = RemotePlayer.GetByConnectionId(Context.ConnectionId);
		if (player is not null)
			matchmakingService.RemovePlayerFromWaiting(player.Id);
	}


	public async Task LeaveMatch(string gameId)
	{
		var game = matchmakingService.GetRunningGame(gameId);
		if (game is null)
			return;

		var player = RemotePlayer.GetByConnectionId(Context.ConnectionId);
		if (player is null)
			return;

		var playerToNotify = game.GetOpponentPlayerId(player.Id);

		if (playerToNotify.IsEmpty() == false)
			await Clients.Client(playerToNotify).SendAsync("OpponentLeft");
	}

	public async Task MakeMove(string gameId, int fieldNum)
	{
		var player = RemotePlayer.GetByConnectionId(Context.ConnectionId);
		if (player is null)
			return;

		var game = matchmakingService.GetRunningGame(gameId);
		if (game is null)
			return;

		if (player.Id != game.CurrentTurnPlayerId)
			return;

		var move = new Move(player.Id == game.Player1!.Id ? Player.Player1 : Player.Player2, fieldNum);
		var changed = game.Board.TryMakeMove(move);
		if (changed)
		{
			game.SwitchTurn();
			await Clients.Group(game.Id).SendAsync("UpdateGameState", game.Board.GetFields());
			await CheckGameEnded(game);
		}
	}


	public async Task OnMadeMatch(Game game)
	{
		await Groups.AddToGroupAsync(game.Player1!.ConnectionId, game.Id);
		await Groups.AddToGroupAsync(game.Player2!.ConnectionId, game.Id);

		var startingPlayer = game.CurrentTurnPlayerId == game.Player1.Id ? Player.Player1 : Player.Player2;
		await Clients.Client(game.Player1.ConnectionId).SendAsync("StartGame", game.Id, Player.Player1, startingPlayer);
		await Clients.Client(game.Player2.ConnectionId).SendAsync("StartGame", game.Id, Player.Player2, startingPlayer);
	}
	public async Task CheckGameEnded(Game game)
	{
		if (game.Board.CheckIsFinished(out var winner))
		{
			await Clients.Group(game.Id).SendAsync("GameFinished", winner);
			await Groups.RemoveFromGroupAsync(game.Player1!.ConnectionId, game.Id);
			await Groups.RemoveFromGroupAsync(game.Player2!.ConnectionId, game.Id);

			matchmakingService.RemoveRunningGame(game.Id);
		}
	}
#pragma warning restore IDE1006
}