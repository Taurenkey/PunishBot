using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                _ => throw new NotImplementedException(),
            };

            await command.RespondAsync(embed: embed.Build());
        }

        private static EmbedBuilder BuildInstallEmbed()
        {
            EmbedBuilder embed = new();
            embed.WithImageUrl("https://puni.sh/_next/image?url=%2Fdownload-instructions.png&w=640&q=100");
            embed.WithDescription($"Open the Dalamud Settings menu in game and follow the steps below. This can be done through the button at the bottom of the plugin installer or by typing /xlsettings in the chat.\n\n" +
                $"1. Under Custom Plugin Repositories, enter https://love.puni.sh/ment.json into the empty box at the bottom.\n" +
                $"2. Click the \"+\" button.\n" +
                $"3. Click the \"Save and Close\" button.");
            embed.WithTitle("Installing Puni.sh Plugins");
            return embed;
        }

        private async Task SetupCommands()
        {
            await _client.Rest.DeleteAllGlobalCommandsAsync();
            foreach (var guild in  _client.Guilds)
            {
                await guild.DeleteApplicationCommandsAsync();
            }

            var faqCommand = new SlashCommandBuilder()
                 .WithName("faq")
                 .WithDescription("Find out answers to frequently asked questions")
                 .AddOption("install", ApplicationCommandOptionType.SubCommand, "How to install our repo and plugins");

            await _client.CreateGlobalApplicationCommandAsync(faqCommand.Build());
        }
    }
}
