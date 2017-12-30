using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Diagnostics;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Interactivity;
using DSharpPlus.Entities;

namespace DevChat
{
    public class BasicCommand
    {
        [Command("ping")]
        [Description(">ping\nTest bot.")]
        public async Task Ping(CommandContext ctx)
        {
            await ctx.RespondAsync("pong!");
        }

        [Command("botlink")]
        [Description(">botlink\n봇 초대 링크를 얻습니다.")]
        public async Task BotLink(CommandContext ctx)
        {
            var clientId = ctx.Client.Me.Id;
            await ctx.RespondAsync($"https://discordapp.com/oauth2/authorize?client_id={clientId}}&scope=bot");
        }
    }
}
