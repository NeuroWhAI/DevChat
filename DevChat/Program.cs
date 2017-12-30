using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace DevChat
{
    class Program
    {
        static void Main(string[] args)
        {
#if DEBUG
            using (var sr = new StreamReader("data.txt"))
            {
                string token = sr.ReadLine();
                args = new string[]
                {
                    token
                };
            }
#endif

            if (args.Length < 1)
            {
                Console.WriteLine("program {token}");
                return;
            }


            var bot = new DiscordBot(args[0]);
            //bot.RegisterCommand<>();

            bot.Start();
        }
    }
}
