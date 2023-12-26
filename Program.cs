using Discord;
using Discord.Commands;
using Discord.Net;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using PunishBot;
using PunishBot.Commands;
using System.Reflection;

namespace PunishBot;
public class Program
{
    private readonly DiscordSocketClient _client;
    private readonly CommandHandler _commandHandler;
    private readonly SlashCommandHandler _slashCommandHandler;
    private readonly StatusHandler _statusHandler;
    private readonly Logger _logger;
    private readonly IConfiguration _builder;
    public static Task Main() => new Program().MainAsync();

    private Program()
    {
        _builder = new ConfigurationBuilder()
        .AddUserSecrets(Assembly.GetExecutingAssembly(), true)
        .Build();

        _client = new DiscordSocketClient(new DiscordSocketConfig()
        {
            GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.MessageContent
        });
        _logger = new Logger(_client);
        _commandHandler = new CommandHandler(_client, _logger);
        _slashCommandHandler = new SlashCommandHandler(_client);
        _statusHandler = new StatusHandler(_client);    

    }

    public async Task MainAsync()
    {
        string token = _builder["TOKEN"]!;
        await _commandHandler.Setup();
        await _slashCommandHandler.InstallCommandsAsync(); 
        await _client.LoginAsync(TokenType.Bot, token);
        await _client.StartAsync();
        await _statusHandler.Setup();
        await _logger.Setup();

        // Block this task until the program is closed.
        await Task.Delay(-1);
    }
}