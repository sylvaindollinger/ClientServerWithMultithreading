using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServeurMultithread
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {

                Scheduler scheduler = new Scheduler();
                Listener listener = new Listener(scheduler);

                ConsoleKeyInfo keyinfo;
                do
                {
                    while (!Console.KeyAvailable) { scheduler.TryStartNext(); }
                    keyinfo = Console.ReadKey();
                }
                while (keyinfo.Key != ConsoleKey.Escape);


            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                Console.WriteLine(">> PRESS A KEY TO EXIT");
                Console.ReadKey();
            }

        }
    }
}
