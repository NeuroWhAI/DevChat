using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Interactivity;
using DSharpPlus.CommandsNext.Exceptions;
using DSharpPlus.Entities;

namespace DevChat
{
    public class DiscordBot
    {
        public bool Online { get; set; } = false;

        public DiscordBot(string token)
        {
            m_discord = new DiscordClient(new DiscordConfiguration
            {
                Token = token,
                TokenType = TokenType.Bot,
                AutoReconnect = true,
#if DEBUG
                LogLevel = LogLevel.Debug,
                UseInternalLogHandler = true,
#endif
            });


            Init();
        }

        public void RegisterCommand<T>() where T : class
        {
            m_commands.RegisterCommands<T>();
        }

        public void Start()
        {
            m_discord.ConnectAsync().Wait();

            this.Online = true;

            while (this.Online)
            {
                Task.Delay(500).Wait();
            }
        }

        private void Init()
        {
            m_discord.Ready += async (arg) =>
            {
                Console.WriteLine("Bot ready!");

                await Task.CompletedTask;
            };

            m_discord.MessageCreated += Discord_MessageCreated;


            m_commands = m_discord.UseCommandsNext(new CommandsNextConfiguration
            {
                StringPrefix = ">",
                CaseSensitive = false,
            });

            m_commands.CommandExecuted += Commands_CommandExecuted;
            m_commands.CommandErrored += Commands_CommandErrored;


            m_interactivity = m_discord.UseInteractivity(new InteractivityConfiguration()
            {
                PaginationBehaviour = TimeoutBehaviour.Ignore,
                Timeout = TimeSpan.FromHours(2.0),
                PaginationTimeout = TimeSpan.FromHours(2.0),
            });
        }

        private static async Task Discord_MessageCreated(DSharpPlus.EventArgs.MessageCreateEventArgs e)
        {
            Console.WriteLine("({0}): [{1}] - {2}#{3}", DateTime.UtcNow.ToShortTimeString(),
                e.Channel.Name, e.Author.Username, e.Author.Id);

            await Task.CompletedTask;
        }

        private async Task Commands_CommandErrored(CommandErrorEventArgs e)
        {
            if (e.Exception is ChecksFailedException ex)
            {
                var emoji = DiscordEmoji.FromName(e.Context.Client, ":no_entry:");
                await e.Context.RespondAsync($"{emoji} Access denied!");
            }
            else
            {
                Console.WriteLine(e.Exception.Message);
                Console.WriteLine(e.Exception.StackTrace);

                await e.Context.RespondAsync(e.Exception.Message);
            }
        }

        private async Task Commands_CommandExecuted(CommandExecutionEventArgs e)
        {
            Console.WriteLine("{0}: {1}", e.Context.User.Username, e.Context.Message.Content);
            await Task.CompletedTask;
        }

        private DiscordClient m_discord = null;
        private CommandsNextModule m_commands = null;
        private InteractivityModule m_interactivity = null;
    }
}
