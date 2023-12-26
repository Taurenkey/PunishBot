using Discord;
using Discord.Commands;
using Discord.WebSocket;
using PunishBot.Commands;
using System.Reflection;

public class Program
{
    private DiscordSocketClient _client;
    private readonly CommandHandler _commandHandler;
    public static Task Main(string[] args) => new Program().MainAsync();

    private Program()
    {
        _client = new DiscordSocketClient(new DiscordSocketConfig()
        {
            GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.MessageContent
        });
        _client.Log += Log;
        _commandHandler = new CommandHandler(_client, new CommandService());
        _commandHandler._commands.Log += Log;
    }

    public async Task MainAsync()
    {
        string token = "";
        await _commandHandler.InstallCommandsAsync();
        await _client.LoginAsync(TokenType.Bot, token);
        await _client.StartAsync();

        // Block this task until the program is closed.
        await Task.Delay(-1);
    }

    private Task Log(LogMessage msg)
    {
        Console.WriteLine(msg.ToString());
        return Task.CompletedTask;
    }
}