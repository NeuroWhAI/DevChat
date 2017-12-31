using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using System.Diagnostics;

namespace DevChat
{
    static class Shell
    {
        public static string WorkingDirectory { get; set; }

        public static void WaitForExit(Process proc)
        {
            if (ResetEventMap.Contains(proc))
            {
                var resetEvent = ResetEventMap[proc];

                resetEvent.WaitOne();

                resetEvent.Dispose();
                ResetEventMap.Remove(proc);
            }
            else
            {
                proc.WaitForExit();
            }
        }

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

            ResetEventMap.Add(proc, new AutoResetEvent(false));

            var handler = new DataReceivedEventHandler((sender, e) =>
            {
                if (e.Data == null)
                {
                    ResetEventMap[proc].Set();
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

            WaitForExit(proc);
            proc.Close();

            return output.ToString();
        }

        private static Dictionary<Process, AutoResetEvent> ResetEventMap
        { get; set; } = new Dictionary<Process, AutoResetEvent>();
    }
}
