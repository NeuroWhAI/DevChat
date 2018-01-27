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
        [Description("ping\nTest bot.")]
        public async Task Ping(CommandContext ctx)
        {
            await ctx.RespondAsync("pong!");
        }

        [Command("kill"), RequireOwner]
        [Description("kill {Name}\nKill recent {Name} process.")]
        public async Task Kill(CommandContext ctx, string name)
        {
            DateTime recentTime = DateTime.MinValue;
            Process recentProc = null;

            foreach (var proc in Process.GetProcessesByName(name))
            {
                if (proc.StartTime > recentTime)
                {
                    recentTime = proc.StartTime;
                    recentProc = proc;
                }
            }

            if (recentProc == null)
            {
                await ctx.RespondAsync("There is no process with such a name.");
            }
            else {
                try
                {
                    recentProc.Kill();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    Console.WriteLine(e.StackTrace);
                }

                await ctx.RespondAsync("Process terminated!");
            }
        }
    }
}
