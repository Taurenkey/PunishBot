using Discord;
using Discord.WebSocket;

namespace PunishBot.Commands
{
    public class SlashCommandHandler(DiscordSocketClient client)
    {
        private readonly DiscordSocketClient _client = client;

        public async Task InstallCommandsAsync()
        {
            _client.Ready += SetupCommands;
            _client.SlashCommandExecuted += ExecuteCommands;
            await Task.CompletedTask;
        }

        private async Task ExecuteCommands(SocketSlashCommand command)
        {
            if (command.CommandName == "faq")
            {
                await ExecuteFaqCommand(command);
            }
        }

        private static async Task ExecuteFaqCommand(SocketSlashCommand command)
        {
            EmbedBuilder embed = command.Data.Options.First().Name switch
            {
                "install" => BuildInstallEmbed(),
                "migration" => BuildMigrationEmbed(),
                "logs" => BuildLogsEmbed(false),
                "crashes" => BuildLogsEmbed(true),
                "repos" => BuildReposEmbed(command),
                _ => throw new NotImplementedException(),
            };

            await command.RespondAsync(embed: embed.Build());
        }

        private static EmbedBuilder BuildReposEmbed(SocketSlashCommand command)
        {
            throw new NotImplementedException();
        }

        private static EmbedBuilder BuildLogsEmbed(bool crashLogs)
        {
            throw new NotImplementedException();
        }

        private static EmbedBuilder BuildMigrationEmbed()
        {
            EmbedBuilder embed = new();
            embed.WithTitle("Migrating Plugins");
            embed.WithDescription($"Sometimes a plugin author will decide to move their plugin repository elsewhere. In this circumstance, any plugins you may have installed from them will be classed as \"orphaned\" and can no longer be updated.\n\n" +
                $"If this occurs, you should do the following steps:\n\n" +
                $"1. Find out the new repository link from the author. **Do not uninstall the author's plugins until you can confirm this step!!**\n" +
                $"2. Disable the author's plugins and uninstall them.\n" +
                $"3. Open the `/xlsettings` menu and click the Experimental tab.\n" +
                $"4. Under Custom Plugin repositories, click the trash can next to the old repository.\n" +
                $"5. Paste the new repository link from step 1 into the empty box.\n" +
                $"6. Click the \"+\" button.\n" +
                $"7. Click the \"Save and Close\" button.\n" +
                $"8. Reinstall the plugins you previously uninstalled.\n\n" +
                $"If the plugin installer was open between steps 3-7, you will have to re-open it to refresh the plugin list.");
            return embed;
        }

        private static EmbedBuilder BuildInstallEmbed()
        {
            EmbedBuilder embed = new();
            embed.WithImageUrl("https://puni.sh/_next/image?url=%2Fdownload-instructions.png&w=640&q=100");
            embed.WithDescription($"Open the Dalamud Settings menu in game and follow the steps below. This can be done through the button at the bottom of the plugin installer or by typing /xlsettings in the chat.\n\n" +
                $"1. Click on the Experimental tab and under Custom Plugin Repositories, enter https://love.puni.sh/ment.json into the empty box at the bottom.\n" +
                $"2. Click the \"+\" button.\n" +
                $"3. Click the \"Save and Close\" button.");
            embed.WithTitle("Installing Puni.sh Plugins");
            return embed;
        }

        private async Task SetupCommands()
        {
            await _client.Rest.DeleteAllGlobalCommandsAsync();

            var faqCommand = new SlashCommandBuilder()
                 .WithName("faq")
                 .WithDescription("Find out answers to frequently asked questions")
                 .AddOption("install", ApplicationCommandOptionType.SubCommand, "How to install our repo and plugins")
                 .AddOption("migration", ApplicationCommandOptionType.SubCommand, "What to do when plugin repos migrate")
                 .AddOption("logs", ApplicationCommandOptionType.SubCommand, "Sending a developer logs")
                 .AddOption("crashes", ApplicationCommandOptionType.SubCommand, "Sending a developer crash logs")
                 .AddOption(new SlashCommandOptionBuilder()
                    .WithName("repos")
                    .WithDescription("Links to various repos")
                    .WithType(ApplicationCommandOptionType.SubCommand)
                    .AddOption(new SlashCommandOptionBuilder()
                        .WithName("user")
                        .WithDescription("The user the repo belongs to")
                        .WithType(ApplicationCommandOptionType.Integer)
                        .WithRequired(true)
                        .AddChoice("Taurenkey", 0)
                        .AddChoice("Kawaii", 1)
                        .AddChoice("Veyn", 2)
                        .AddChoice("Cupcake", 3)
                        .AddChoice("Croizat", 4)
                        .AddChoice("Liza", 5)
                    ));

            foreach (var guild in _client.Guilds)
            {
                await guild.BulkOverwriteApplicationCommandAsync([faqCommand.Build()]);
            }
        }
    }
}
