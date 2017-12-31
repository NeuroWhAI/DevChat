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
    public class Interactor : IPushMessage, IReceiveStreamWriter
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

        public void Start()
        {
            this.Online = true;

            m_sendTask = new Task(this.SendJob);
            m_sendTask.Start();

            m_recvTask = new Task(this.ReceiveJob);
            m_recvTask.Start();
        }

        public void Stop()
        {
            this.Online = false;
        }

        public void Wait()
        {
            m_sendTask.Wait();
            m_recvTask.Wait();

            m_sendTask = null;
            m_recvTask = null;


            var output = DequeueSendBuffer(2048);

            if (output.Length > 0)
            {
                m_ctx.RespondAsync($"```\n{output}\n```")
                    .ConfigureAwait(false).GetAwaiter().GetResult();
            }


            this.Online = false;
        }

        public void StopAndWait()
        {
            Stop();
            Wait();
        }

        public void PushMessage(string message)
        {
            if (this.Online)
            {
                m_sendBuffer.Enqueue(message);
            }
        }

        public void SetStreamWriter(StreamWriter sw)
        {
            this.InputStream = sw;
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

        private void SendJob()
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

                        Task.Delay(1000).Wait();
                    }
                }


                Task.Delay(this.SendPeriod).Wait();
            }
        }

        private void ReceiveJob()
        {
            MessageManager.Clear();

            
            while (this.Online)
            {
                if (this.InputStream != null)
                {
                    var msg = MessageManager.GetMessageMatch(arg =>
                    {
                        return (arg.Channel.Id == m_ctx.Channel.Id
                            && arg.Author.Id == m_ctx.User.Id);
                    });

                    if (msg != null)
                    {
                        if (msg.Message.Content == this.ExitKeyword)
                        {
                            break;
                        }

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

                Task.Delay(50).Wait();
            }


            this.Online = false;
        }

        private CommandContext m_ctx = null;
        private ConcurrentQueue<string> m_sendBuffer = new ConcurrentQueue<string>();
        private Task m_sendTask = null;
        private Task m_recvTask = null;
    }
}
