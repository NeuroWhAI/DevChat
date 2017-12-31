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
                WorkingDirectory = this.WorkingDirectory,
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
                if (!string.IsNullOrEmpty(e.Data))
                {
                    outputHandler(e.Data);
                }
            });

            proc.BeginOutputReadLine();
            proc.Start();

            return proc;
        }

        public static string Execute(string command)
        {
            var output = new StringBuilder();

            var proc = Execute("cmd.exe", command, data =>
            {
               output.Append(data); 
            });

            proc.WaitForExit();
            proc.Close();

            return output.ToString();
        }
    }
}
