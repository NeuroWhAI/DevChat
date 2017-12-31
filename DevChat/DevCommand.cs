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
    public class DevCommand
    {
        [Command("create"), Aliases("new"), RequireOwner]
        [Description("create {Name} {Git URL}\nCreate a project.")]
        public async Task Create(CommandContext ctx, string name, string gitUrl)
        {
            await NotifyWorking(ctx, "Create " + name);

            var interactor = new Interactor(ctx);
            interactor.Start();

            ProjectMgr.CreateProject(name, gitUrl, interactor);

            interactor.StopAndWait();

            await NotifyFinish(ctx);
        }

        [Command("delete"), Aliases("del"), RequireOwner]
        [Description("delete {Name}\nDelete a project.")]
        public async Task Delete(CommandContext ctx, string name)
        {
            await NotifyWorking(ctx, "Delete " + name);

            var interactor = new Interactor(ctx);
            interactor.Start();

            ProjectMgr.DeleteProject(name, interactor);

            interactor.StopAndWait();

            await NotifyFinish(ctx);
        }

        [Command("info"), RequireOwner]
        [Description("info {Name}\nShow properties of {Name}.")]
        public async Task Info(CommandContext ctx, string name)
        {
            await NotifyWorking(ctx, "Info " + name);

            var interactor = new Interactor(ctx);
            interactor.Start();

            ProjectMgr.InfoProject(name, interactor);

            interactor.StopAndWait();

            await NotifyFinish(ctx);
        }

        [Command("build"), RequireOwner]
        [Description("build {Name} [{Script}]\nBuild {Name} with script file at {Script}.")]
        public async Task Build(CommandContext ctx, string name, string scriptPath = "")
        {
            await NotifyWorking(ctx, "Build " + name);

            await NotifyFinish(ctx);
        }

        [Command("run"), RequireOwner]
        [Description("run {Name} [{File}]\nExecute {Name}\'s file at {Script}.")]
        public async Task Run(CommandContext ctx, string name, string filePath = "")
        {
            await NotifyWorking(ctx, "Run " + name);

            await NotifyFinish(ctx);
        }

        [Command("config"), Aliases("cfg"), RequireOwner]
        [Description("config {Name} {Property} {Value}\nSet {Property} of {Name} to {Value}.")]
        public async Task Config(CommandContext ctx, string name, string prop, string data)
        {
            await NotifyWorking(ctx, "Config " + name);

            await NotifyFinish(ctx);
        }

        private async Task NotifyWorking(CommandContext ctx, string job)
        {
            await ctx.TriggerTypingAsync();
            await ctx.Client.UpdateStatusAsync(game: new DiscordGame(job));
        }

        private async Task NotifyFinish(CommandContext ctx)
        {
            await ctx.Client.UpdateStatusAsync();
        }

        private ProjectManager ProjectMgr { get; set; } = new ProjectManager();
    }
}
