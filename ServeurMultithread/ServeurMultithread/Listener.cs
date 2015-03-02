using CommonLibrary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ServeurMultithread
{
    class Listener
    {
        static byte[] address = { 127, 0, 0, 1 };
        static int port = 8888;

        private byte[] buffer = new byte[Job.bufferSize];

        Scheduler ordonnanceur = null;

        public Listener(Scheduler ordonnanceur)
        {
            try
            {
                this.ordonnanceur = ordonnanceur;
                Socket activeSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPAddress ip = new IPAddress(address);
                IPEndPoint iep = new IPEndPoint(ip, port);
                activeSocket.Bind(iep);
                activeSocket.Listen(1);

                BeginAccept(activeSocket);
                Console.WriteLine(" >> SERVER STARTED");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                Console.WriteLine(">> PRESS A KEY TO EXIT");
                Console.ReadKey();
            }
        }

      

        //*************************************************************************************************
        // CALLBACKS
        //*************************************************************************************************
       
        private void OnConnectRequest(IAsyncResult ar)
        {
            Socket activeSocket = (Socket)ar.AsyncState;
            activeSocket = activeSocket.EndAccept(ar);
            Console.WriteLine("CLIENT {0} CONNECTED", activeSocket.RemoteEndPoint);
            BeginReceive(activeSocket);
           // BeginAccept(activeSocket);
        }


        private void OnReceivedData(IAsyncResult ar)
        {
            // Socket was the passed in object
            Socket activeSocket = (Socket)ar.AsyncState;

            // Check if we got any data
            try
            {
                int nBytesRec = activeSocket.EndReceive(ar);
                if (nBytesRec > 0)
                {
                    ordonnanceur.Enqueue(buffer);
                    // If the connection is still usable restablish the callback
                    BeginReceive(activeSocket);
                }
                else
                {
                    // If no data was recieved then the connection is probably dead
                    Console.WriteLine("Client {0}, disconnected",
                                       activeSocket.RemoteEndPoint);

                    //activeSocket.Shutdown(SocketShutdown.Both);
                    //activeSocket.Disconnect(true);
                    //activeSocket.Close();
                    //activeSocket.Dispose(); 


                    //activeSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    //IPAddress ip = new IPAddress(address);
                    //IPEndPoint iep = new IPEndPoint(ip, port);

                    //activeSocket.Bind(iep);
                   
                    activeSocket.Listen(1);

                    BeginAccept(activeSocket);

                }

            }
            catch (SocketException e)
            {
                activeSocket.Shutdown(SocketShutdown.Both);
                activeSocket.Disconnect(true);
                activeSocket.Close();
                activeSocket.Dispose();
                

                activeSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPAddress ip = new IPAddress(address);
                IPEndPoint iep = new IPEndPoint(ip, port);
                activeSocket.Bind(iep);
                activeSocket.Listen(1);

                BeginAccept(activeSocket);

            }
            
            

        }

        //*************************************************************************************************
        // ACCEPT
        //*************************************************************************************************

        private void BeginAccept(Socket socket)
        {
            AsyncCallback ConnectRequested = new AsyncCallback(OnConnectRequest);
            socket.BeginAccept(ConnectRequested, socket);
        }

        private void EndAccept(IAsyncResult ar)
        {
            Socket activeSocket = (Socket)ar.AsyncState;
            activeSocket = activeSocket.EndAccept(ar);
        }


        //*************************************************************************************************
        // RECEIVE
        //*************************************************************************************************


        private void BeginReceive(Socket socket)
        {
            AsyncCallback receiveData = new AsyncCallback(OnReceivedData);
            socket.BeginReceive(buffer, 0, buffer.GetLength(0),
                               SocketFlags.None, receiveData, socket);
            
        }

      

    }
}
