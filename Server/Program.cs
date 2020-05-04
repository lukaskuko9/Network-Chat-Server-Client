using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class Server
    {
       // const int PORT_NO = 5000;
       // const string SERVER_IP = "127.0.0.1";
        
        TcpListener listener;
        List<TcpClient> clients = new List<TcpClient>();

        public delegate void ClientConnection(TcpClient client);
        public event ClientConnection OnClientConnected;

        public Server(string SERVER_IP, int PORT_NO)
        {
            IPAddress localAdd = IPAddress.Parse(SERVER_IP);
            listener = new TcpListener(localAdd, PORT_NO);

            OnClientConnected += (TcpClient q) =>
            {

                clients.Add(q);
            };
        }

        public TcpClient AcceptTcpClient()
        {
            TcpClient client = listener.AcceptTcpClient();
            clients.Add(client);
            return client;
        }


        public async Task AcceptTcpClients()
        {
            TcpClient client = await listener.AcceptTcpClientAsync();
            OnClientConnected(client);
        }

        public void StartListening()
        {
            //---listen at the specified IP and port no.---
            Console.WriteLine("Listening...");
            listener.Start();
        }

        public void Listen(TcpClient client)
        {            
            //---get the incoming data through a network stream---
            NetworkStream nwStream = client.GetStream();
            byte[] buffer = new byte[client.ReceiveBufferSize];

            //---read incoming stream---
            int bytesRead = nwStream.Read(buffer, 0, client.ReceiveBufferSize);

            //---convert the data received into a string---
            string dataReceived = Encoding.ASCII.GetString(buffer, 0, bytesRead);
            Console.WriteLine("Received : " + dataReceived);

            //---write back the text to the client---
            Console.WriteLine("Sending back : " + dataReceived);
            nwStream.Write(buffer, 0, bytesRead);
        }

        public void Disconnect()
        {
            foreach (var client in clients)
            {
                Console.WriteLine($"Disconnecting");
                client.Close();
            }
            
            listener.Stop();
            Console.ReadLine();
        }
    }


    class Program
    {


        static void Main(string[] args)
        {
            string serverip = args[0];
            int port = Convert.ToInt32(args[1]);
            Server server = new Server(serverip, port);
            Console.WriteLine($"Starting server at {serverip}:{port}");
            server.StartListening();
            var client = server.AcceptTcpClient();

            Console.WriteLine("Client accepted");
            while(true)
            {
                server.Listen(client);

            }
        }
    }
}
