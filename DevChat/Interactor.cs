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
        public TimeSpan SendPeriod { get; set; } = TimeSpan.FromSeconds(2.0);
        public StreamWriter InputStream { get; set; } = null;
        public string ExitKeyword { get; set; } = "!q";
        public bool Online { get; set; } = false;

        public async Task Start()
        {
            this.Online = true;


            var interactivity = m_ctx.Client.GetInteractivityModule();


            // Send task
            var sendTask = new Task(() =>
            {
                while (this.Online)
                {
                    var output = DequeueSendBuffer(2048);

                    while (this.Online && output.Length > 0)
                    {
                        try
                        {
                            m_ctx.RespondAsync($"```\n{output}\n```")
                                .ConfigureAwait(false).GetAwaiter().GetResult();

                            break;
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                            Console.WriteLine(e.StackTrace);
                        }
                    }


                    Task.Delay(this.SendPeriod).Wait();
                }
            });
            sendTask.Start();


            // Receive task
            while (this.Online)
            {
                var msg = await interactivity.WaitForMessageAsync(xm => xm.Author.Id == m_ctx.User.Id,
                    this.TimeOut);

                if (msg == null || msg.Message.Content == this.ExitKeyword)
                {
                    break;
                }

                if (this.InputStream != null)
                {
                    try
                    {
                        this.InputStream.WriteLine(msg.Message.Content);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                        Console.WriteLine(e.StackTrace);

                        this.InputStream = null;
                        break;
                    }
                }
            }


            this.Online = false;


            sendTask.Wait();
        }

        public async Task Finish()
        {
            var output = DequeueSendBuffer(2048);

            if (output.Length > 0)
            {
                await m_ctx.RespondAsync($"```\n{output}\n```");
            }


            this.Online = false;
        }

        public void PushMessage(string message)
        {
            m_sendBuffer.Enqueue(message);
        }

        private string DequeueSendBuffer(int maxLength)
        {
            var buffer = new StringBuilder();

            while (m_sendBuffer.IsEmpty == false)
            {
                string msg;
                while (!m_sendBuffer.TryPeek(out msg))
                {
                    Task.Delay(10).Wait();
                }

                if (buffer.Length + msg.Length > maxLength)
                {
                    break;
                }
                
                while (!m_sendBuffer.TryDequeue(out msg))
                {
                    Task.Delay(10).Wait();
                }

                buffer.AppendLine(msg);
            }

            return buffer.ToString();
        }

        private CommandContext m_ctx = null;
        private ConcurrentQueue<string> m_sendBuffer = new ConcurrentQueue<string>();
    }
}
