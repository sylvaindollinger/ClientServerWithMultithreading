using CommonLibrary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ClientSimulateur
{


    class Simulator
    {
        private byte[] address = { 127, 0, 0, 1 };
        private Int32 port = 8888;
        private byte[] buffer = new byte[Job.bufferSize];
        private Socket sock = default(Socket);
        private IPEndPoint iep;

        private Random randomizer;
        UInt64 idTraitement;


        public Simulator(int Seed)
        {
            randomizer = new Random(Seed);
            idTraitement = 0;
            sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPAddress ip = new IPAddress(address);
            iep = new IPEndPoint(ip, port);
            sock.Connect(iep);
            Console.WriteLine("Connection made with {0}", sock.RemoteEndPoint);
        }

        ~Simulator()
        {
            sock.Disconnect(false);
            sock.Close();
          }

        //---------------------------------------------------------------------------
        //
        // SIMULATEUR
        //
        //---------------------------------------------------------------------------

        public void SendRandomRequest()
        {

            idTraitement += 1;
            Job traitement = new Job(
                idTraitement, 
                (byte) randomizer.Next(3), 
                (byte) (randomizer.Next(4) + 1),
                (UInt32) (randomizer.Next(5000))+5000);
            
            SendRequest(traitement);
        }

        private void SendRequest(Job traitement)
        {
            byte[] buffer = traitement.ToByte();
            Console.WriteLine(" >> sending [{0}]", traitement.ToString());
            sock.Send(buffer, 0, buffer.Length, SocketFlags.None);
        }

    }
}
