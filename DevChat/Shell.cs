using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

namespace DevChat
{
    static class Shell
    {
        public static string WorkingDirectory { get; set; }

        public static Process Execute(string file, string argument, Action<string> outputHandler)
        {
            var info = new ProcessStartInfo(file)
            {
                WorkingDirectory = WorkingDirectory,
                Arguments = argument,
                UseShellExecute = false,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
            };

            var proc = new Process();
            proc.StartInfo = info;

            var handler = new DataReceivedEventHandler((sender, e) =>
            {
                if (e.Data == null)
                {
                    proc.Kill();
                }
                else
                {
                    Console.WriteLine(e.Data);

                    outputHandler(e.Data);
                }
            });
            proc.OutputDataReceived += handler;
            proc.ErrorDataReceived += handler;

            proc.Start();

            proc.BeginOutputReadLine();

            return proc;
        }

        public static string Execute(string command)
        {
            var output = new StringBuilder();

            var proc = Execute("cmd.exe", "/C " + command, data =>
            {
                output.AppendLine(data); 
            });

            while (proc.HasExited == false)
            {
                Task.Delay(10).Wait();
            }

            proc.Close();

            return output.ToString();
        }
    }
}
