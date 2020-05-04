using System;
using System.Collections.Generic;
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
        public event ClientConnection OnClientDisconnected;

        public Server(string SERVER_IP, int PORT_NO)
        {
            IPAddress localAdd = IPAddress.Parse(SERVER_IP);
            listener = new TcpListener(localAdd, PORT_NO);

            OnClientConnected += async (TcpClient client) =>
            {
                Global.logger.WriteLine("Client connected...");
                clients.Add(client);
                await Listen(client);
            };

            OnClientDisconnected += (TcpClient client) =>
            {
                Global.logger.WriteLine("Client disconnected...");
                clients.Remove(client);
                client.Dispose();
                client = null;
            };
        }

        public async Task AcceptTcpClient()
        {
            TcpClient client = await listener.AcceptTcpClientAsync();    
            OnClientConnected(client);
        }

        public void StartListening()
        {
            //---listen at the specified IP and port no.---
            Global.logger.WriteLine("Listening...");
            listener.Start();
        }

        private async Task sendMessageToAll(byte[] buffer, int bytesRead)
        {
            foreach (var client in clients)
            {
                NetworkStream nwStream = client.GetStream();
                //byte[] buffer = new byte[client.SendBufferSize];

                //Console.WriteLine("Sending back : " + dataReceived);
                await nwStream.WriteAsync(buffer, 0, bytesRead);
            }
        }
        public async Task Listen(TcpClient client)
        {
            while (client != null && client.Connected)
            {

                try
                {
                    //---get the incoming data through a network stream---
                    NetworkStream nwStream = client.GetStream();
                    byte[] buffer = new byte[client.ReceiveBufferSize];

                    //---read incoming stream---
                    int bytesRead = await nwStream.ReadAsync(buffer, 0, client.ReceiveBufferSize);

                    //---convert the data received into a string---
                    string dataReceived = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                    Global.logger.WriteLine("Received : " + dataReceived);

                    //---write back the text to the client---
                    await sendMessageToAll(buffer, bytesRead);
                }
                catch
                {
                    if (!client.Connected)
                        OnClientDisconnected(client);
                }


            }

        }

        public void Disconnect()
        {
            foreach (var client in clients)
            {
                Global.logger.WriteLine($"Disconnecting");
                client.Close();
            }
            
            listener.Stop();
            Console.ReadLine();
        }
    }
}
