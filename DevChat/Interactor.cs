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

        public async Task Start()
        {
            var interactivity = m_ctx.Client.GetInteractivityModule();


            // Send task
            var sendTask = new Task(() =>
            {
                while (interactivity != null)
                {
                    
                }
            });
            sendTask.Start();


            // Receive task
            while (interactivity != null)
            {
                var msg = await interactivity.WaitForMessageAsync(xm => xm.Author.Id == m_ctx.User.Id,
                    this.TimeOut);

                if (msg == null || msg.Message.Content == this.ExitKeyword)
                {
                    break;
                }

                if (this.InputStream != null)
                {
                    this.InputStream.WriteLine(msg.Message.Content);
                }
            }


            interactivity = null;


            sendTask.Wait();
        }

        public async Task Finish()
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

            await m_ctx.RespondAsync("```\n" + buffer.ToString() + "\n```");
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
