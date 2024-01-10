using System.Text.Json.Serialization;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using TikTakToe.Lib.Enums;
using TikTakToe.Lib.Models;
using Utils.Extensions;

namespace TikTakToe.Lib.Board;

public class RemoteBoardHandler : BoardHandler, IDisposable
{
	public record RemoteResult(Player Winner, Player MyPlayer, bool OpponentLeft = false, bool Canceled = false) : Result(Winner);

	public event EventHandler? JoinedMatchmaking;
	public event EventHandler? FoundOpponent;
	public Player MyPlayer { get; private set; } = Player.NoOne;
	private readonly HubConnection hubConnection;
	private string playerId = "";
	private string gameId = "";
	private bool disposedValue;

	public RemoteBoardHandler(string hubUrl)
	{
		hubConnection = new HubConnectionBuilder()
			.WithUrl(hubUrl)
			.AddJsonProtocol(options =>
			{
				options.PayloadSerializerOptions.Converters.Add(new JsonStringEnumConverter());
			})
			.Build();

		hubConnection.On<Player[]>("UpdateGameState", OnUpdateGameState);
		hubConnection.On<Player>("GameFinished", OnGameFinished);
		hubConnection.On<string, Player>("StartGame", OnStartGame);
		hubConnection.On("OpponentLeft", () =>
		{
			RaiseFinished(new RemoteResult(Player.NoOne, MyPlayer, true, false));
		});

	}

	private void OnStartGame(string gameId, Player myPlayer)
	{
		_board = new();
		this.gameId = gameId;
		MyPlayer = myPlayer;
		NextTurn = Player.Player1;
		IsPlaying = true;
		FoundOpponent?.Invoke(this, EventArgs.Empty);
	}
	private void OnGameFinished(Player winner)
	{
		IsPlaying = false;
		NextTurn = Player.NoOne;
		hubConnection.StopAsync().AwaitSync();
		RaiseFinished(new RemoteResult(winner, MyPlayer));
	}
	private void OnUpdateGameState(Player[] state)
	{
		_board = new ShallowBord(state);
		NextTurn = _board.GetFreeFields().Count() % 2 == 1 ? Player.Player1 : Player.Player2;
		RaiseUpdatedBoard();
	}

	public override void Start(Player startPlayer)
	{
		IsPlaying = false;
		hubConnection.StartAsync().AwaitSync();
		JoinedMatchmaking?.Invoke(this, EventArgs.Empty);
		playerId = hubConnection.InvokeAsync<RemotePlayer>("JoinMatchmaking").AwaitSync().Id;
	}
	public void LeaveMatchmaking()
	{
		hubConnection.InvokeAsync("LeaveMatchmaking", playerId).AwaitSync();
		RaiseFinished(new RemoteResult(Player.NoOne, MyPlayer, false, true));
	}

	public override bool MakeMove(Move move)
	{
		move = new Move(MyPlayer, move.Field);
		if (ValidMove(move))
		{
			hubConnection.InvokeAsync("MakeMove", playerId, gameId, move.Field).AwaitSync();
			return true;
		}

		return false;
	}

	protected virtual void Dispose(bool disposing)
	{
		if (!disposedValue)
		{
			if (disposing)
			{
				try
				{
					if (hubConnection.State == HubConnectionState.Connected)
					{
						if (IsPlaying)
							hubConnection.InvokeAsync("LeaveMatch", playerId, gameId).AwaitSync();
						else
							hubConnection.InvokeAsync("LeaveMatchmaking", playerId).AwaitSync();
					}
				}
				catch { }
				finally
				{
					hubConnection.DisposeAsync();
				}
			}

			// TODO: Nicht verwaltete Ressourcen (nicht verwaltete Objekte) freigeben und Finalizer überschreiben
			// TODO: Große Felder auf NULL setzen
			disposedValue = true;
		}
	}

	~RemoteBoardHandler()
	{
		// Ändern Sie diesen Code nicht. Fügen Sie Bereinigungscode in der Methode "Dispose(bool disposing)" ein.
		Dispose(disposing: false);
	}

	public void Dispose()
	{
		// Ändern Sie diesen Code nicht. Fügen Sie Bereinigungscode in der Methode "Dispose(bool disposing)" ein.
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

}
