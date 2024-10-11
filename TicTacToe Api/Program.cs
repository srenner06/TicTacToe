using System.Text.Json.Serialization;
using TicTacToe.Api.Hubs;
using TicTacToe.Api.Services;

namespace TicTacToe.Api;

public class Program
{
	public static void Main(string[] args)
	{
		var builder = WebApplication.CreateBuilder(args);

		// Add services to the container.

		builder.Services.AddControllers();
		// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
		builder.Services.AddEndpointsApiExplorer();
		builder.Services.AddSwaggerGen();
		builder.Services.AddSignalR()
		.AddJsonProtocol(options =>
		{
			options.PayloadSerializerOptions.IgnoreReadOnlyFields = false;
			options.PayloadSerializerOptions.IncludeFields = true;
			options.PayloadSerializerOptions.Converters.Add(new JsonStringEnumConverter());
		});
		builder.Services.AddSingleton<MatchmakingService>();

		var app = builder.Build();

		// Configure the HTTP request pipeline.
		if (app.Environment.IsDevelopment())
		{
			app.UseSwagger();
			app.UseSwaggerUI();
		}

		app.UseHttpsRedirection();

		app.UseAuthorization();

		app.MapHub<TicTacToeHub>("/tictactoehub");
		app.MapControllers();

		app.Run();
	}
}
