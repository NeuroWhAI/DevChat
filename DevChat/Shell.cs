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

            proc.OutputDataReceived += new DataReceivedEventHandler((sender, e) =>
            {
                if (e.Data != null)
                {
                    Console.WriteLine(e.Data);

                    outputHandler(e.Data);
                }
            });
            proc.ErrorDataReceived += new DataReceivedEventHandler((sender, e) =>
            {
                if (e.Data != null)
                {
                    Console.WriteLine(e.Data);

                    outputHandler(e.Data);
                }
            });

            proc.Start();

            proc.BeginOutputReadLine();

            return proc;
        }

        public static string Execute(string file, string argument)
        {
            var info = new ProcessStartInfo(file)
            {
                WorkingDirectory = WorkingDirectory,
                Arguments = argument,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
            };

            var proc = new Process();
            proc.StartInfo = info;

            proc.Start();

            string output = proc.StandardOutput.ReadToEnd() + "\n"
                + proc.StandardError.ReadToEnd();

            proc.WaitForExit();

            return output;
        }
    }
}
