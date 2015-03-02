using System;
using System.Threading;

namespace ClientSimulateur
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine(">> Trying to connect");
                Simulator simulator = new Simulator(666);
                Console.WriteLine(">> Sending Packets");

                ConsoleKeyInfo keyinfo;
                do
                {
                    while (!Console.KeyAvailable)
                    {
                        simulator.SendRandomRequest();
                        Thread.Sleep(100);
                    }

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
