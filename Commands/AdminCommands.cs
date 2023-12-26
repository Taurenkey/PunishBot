using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PunishBot.Commands
{
    public class AdminCommands : ModuleBase<SocketCommandContext>
    {
        [Command("logout")]
        [RequireUserPermission(Discord.GuildPermission.Administrator)]
        public async Task LogoutCommand()
        {
            await Logger.Log(Discord.LogSeverity.Critical, "LogOut Command", "Attempting to Logout");
            await Context.Client.LogoutAsync();
        }


    }
}
