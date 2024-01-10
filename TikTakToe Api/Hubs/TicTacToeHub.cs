using System.Formats.Asn1;
using Microsoft.AspNetCore.SignalR;
using TikTakToe.Api.Services;
using TikTakToe.Lib.Enums;
using TikTakToe.Lib.Models;
using Utils.Extensions;

namespace TikTakToe.Api.Hubs;

public class TicTacToeHub : Hub
{
	private readonly MatchmakingService matchmakingService;

	public TicTacToeHub(MatchmakingService matchmakingService)
	{
		this.matchmakingService = matchmakingService;
		this.matchmakingService.MadeMatch += OnMadeMatch;
	}

	public RemotePlayer JoinMatchmaking()
	{
		var player = matchmakingService.AddPlayer(Context.ConnectionId);
		return player;
	}

	public void LeaveMatchmaking(string playerId)
	{
		matchmakingService.RemovePlayer(playerId);
	}
	public async Task LeaveMatch(string playerId, string gameId)
	{
		var game = matchmakingService.GetGame(gameId);
		if (game is null)
			return;

		var playerToNotify = GetOpponentConnectionId(game, playerId);

		if (playerToNotify.IsEmpty() == false)
		await Clients.Client(playerToNotify).SendAsync("OpponentLeft");
	}
	private string GetOpponentConnectionId(Game game, string playerId)
	{
		return playerId switch
		{
			var id when id == game.Player1.Id => game.Player2.Id,
			var id when id == game.Player2.Id => game.Player1.Id,
			_ => string.Empty
		};
	}
	private async void OnMadeMatch(object? sender, Game game)
	{
		await Groups.AddToGroupAsync(game.Player1.ConnectionId, game.Id);
		await Groups.AddToGroupAsync(game.Player2.ConnectionId, game.Id);

		await Clients.Client(game.Player1.ConnectionId).SendAsync("StartGame", game.Id, Player.Player1);
		await Clients.Client(game.Player2.ConnectionId).SendAsync("StartGame", game.Id, Player.Player2);
	}

	public async Task MakeMove(string playerId, string gameId, int fieldNum)
	{
		var game = matchmakingService.GetGame(gameId);
		if (game is null)
			return;

		if (playerId != game.CurrentTurnPlayerId)
			return;

		var player = playerId == game.Player1.Id ? Player.Player1 : Player.Player2;
		var changed = game.Board.SetByNum(fieldNum, player, false);
		if (changed)
		{
			game.CurrentTurnPlayerId = GetOpponentConnectionId(game, playerId);
			await Clients.Group(game.Id).SendAsync("UpdateGameState", game.Board.GetFields());
			await CheckGameEnded(game);
		}
	}
	private async Task CheckGameEnded(Game game)
	{
		var winner = game.Board.CheckWin();
		if (winner != Player.NoOne || game.Board.GetFreeFields().Any() == false)
		{
			await Clients.Group(game.Id).SendAsync("GameFinished", winner);

			await Groups.RemoveFromGroupAsync(game.Player1.Id, game.Id);
			await Groups.RemoveFromGroupAsync(game.Player2.Id, game.Id);
		}
	}
}
