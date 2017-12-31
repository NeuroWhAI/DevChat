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
            var outputEvent = OutputEventMap[proc];
            var errorEvent = ErrorEventMap[proc];

            outputEvent.WaitOne();
            errorEvent.WaitOne();

            outputEvent.Dispose();
            OutputEventMap.Remove(proc);

            errorEvent.Dispose();
            ErrorEventMap.Remove(proc);
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

            OutputEventMap.Add(proc, new AutoResetEvent(false));
            ErrorEventMap.Add(proc, new AutoResetEvent(false));

            proc.OutputDataReceived += new DataReceivedEventHandler((sender, e) =>
            {
                if (e.Data == null)
                {
                    OutputEventMap[proc].Set();
                }
                else
                {
                    Console.WriteLine(e.Data);

                    outputHandler(e.Data);
                }
            });
            proc.ErrorDataReceived += new DataReceivedEventHandler((sender, e) =>
            {
                if (e.Data == null)
                {
                    ErrorEventMap[proc].Set();
                }
                else
                {
                    Console.WriteLine(e.Data);

                    outputHandler(e.Data);
                }
            });

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

        private static Dictionary<Process, AutoResetEvent> OutputEventMap
        { get; set; } = new Dictionary<Process, AutoResetEvent>();
        private static Dictionary<Process, AutoResetEvent> ErrorEventMap
        { get; set; } = new Dictionary<Process, AutoResetEvent>();
    }
}
