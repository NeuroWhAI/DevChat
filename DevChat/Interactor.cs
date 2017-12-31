using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Collections.Concurrent;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Interactivity;
using DSharpPlus.Entities;

namespace DevChat
{
    public class Interactor
    {
        public Interactor(CommandContext ctx)
        {
            m_ctx = ctx;
        }

        public TimeSpan TimeOut { get; set; } = TimeSpan.FromHours(2.0);
        public StreamWriter InputStream { get; set; } = null;
        public string ExitKeyword { get; set; } = "";

        public void Start()
        {
            var interactivity = m_ctx.Client.GetInteractivityModule();


            // TODO: Task 만들어서 Send하기


            while (true)
            {
                var msg = interactivity.WaitForMessageAsync(xm => xm.Author.Id == m_ctx.User.Id,
                    this.TimeOut).ConfigureAwait(false).GetAwaiter().GetResult();

                if (msg == null || msg.Message.Content == this.ExitKeyword)
                {
                    break;
                }

                if (this.InputStream != null)
                {
                    this.InputStream.WriteLine(msg.Message.Content);
                }
            }
        }

        public void Finish()
        {
            var buffer = new StringBuilder();

            while (m_sendBuffer.IsEmpty == false)
            {
                string msg;
                while (!m_sendBuffer.TryDequeue(out msg))
                {
                    Task.Delay(10).Wait();
                }

                buffer.AppendLine(msg);

                if (buffer.Length > 2048)
                {
                    break;
                }
            }

            m_ctx.RespondAsync("```\n" + buffer.ToString() + "\n```")
                .ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public void PushMessage(string message)
        {
            m_sendBuffer.Enqueue(message);
        }

        private CommandContext m_ctx = null;
        private ConcurrentQueue<string> m_sendBuffer = new ConcurrentQueue<string>();
        private DateTime m_latestSendTime = DateTime.MinValue;
    }
}
