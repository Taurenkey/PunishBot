using Discord;
using Discord.WebSocket;
using PunishBot.Extensions;
using System.Timers;
using Timer = System.Timers.Timer;

namespace PunishBot
{
    public class StatusHandler(DiscordSocketClient client)
    {
        private readonly DiscordSocketClient _client = client;
        private readonly Timer _timer = new();
        private string currentStatus = "";

        private readonly HashSet<string> statusList =
        [
            "AFKing AutoRetainer",
            "Crafting Artisan List",
            "Opening Pandora's Box",
            "Farming MGP with Saucy",
            "Generating Splatoon layout",
            "Saying YesAlready",
            "Necromancing with PalacePal",
            "S P A C E G L I D I N G | O R B W A L K E R",
            "Needing all the LazyLoot",
            "Smashing Positionals with Avarice"
        ];

        public async Task Setup()
        {
            _timer.Interval = TimeSpan.FromMinutes(1).TotalMilliseconds;
            _timer.Elapsed += SetNextStatus;
            _timer.Start();
          
            currentStatus = statusList.PickRandom();
            await _client.SetCustomStatusAsync(currentStatus);
        }

        private async void SetNextStatus(object? sender, ElapsedEventArgs e)
        {
            if (new Random().Next(1, 100) == 1)
            {
                await Logger.Log(LogSeverity.Info, "StatusHandler", "Super secret Cupcake status striggered");
                await _client.SetCustomStatusAsync("Vibing with Cupcake");
                return;
            }

            var nextStatus = statusList.Where(x => x != currentStatus).PickRandom();
            await Logger.Log(LogSeverity.Info, "StatusHandler", $"Updating Status {currentStatus} -> {nextStatus}");
            currentStatus = nextStatus;
            await _client.SetCustomStatusAsync(currentStatus);
        }
    }
}
