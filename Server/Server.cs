using ChatApp;
using ClientApp;
using ServerClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Remoting;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public static class IntExtensions
    {
        public static void GenerateNewConnectionInfo(this ChatClient client)
        {
            client.ConnectionInfo = new ConnectionInfo();
        }
    }

    class Server
    {
        TcpListener listener;
        List<ChatClient> clients = new List<ChatClient>();

        public delegate void ClientConnection(ChatClient client);
        public event ClientConnection OnClientConnected;
        public event ClientConnection OnClientDisconnected;

        public Server(string SERVER_IP, int PORT_NO)
        {
            IPAddress localAdd = IPAddress.Parse(SERVER_IP);
            listener = new TcpListener(localAdd, PORT_NO);

            OnClientConnected += async (ChatClient chatClient) =>
            {
                NetworkStream nwStream = chatClient.GetNetworkStream();

                Global.logger.WriteLine($"Client [ID:{chatClient.ConnectionInfo.ID}] connected...");
                clients.Add(chatClient);
                string data=Serialiser.Serializer.SerializeObject(chatClient.ConnectionInfo); // send user connection info
                await sendMessageToClient(chatClient, data);
                await Listen(chatClient); //listen to messages from this client
            };

            OnClientDisconnected += (ChatClient chatClient) =>
            {
                Global.logger.WriteLine($"Client [ID:{chatClient.ConnectionInfo.ID}] disconnected...");
                clients.Remove(chatClient);
                chatClient.Client.Dispose();
                chatClient = null;
            };
        }

        public async Task AcceptTcpClient()
        {
            TcpClient tcpClient = await listener.AcceptTcpClientAsync();
            NetworkStream nwStream = tcpClient.GetStream();
            ChatClient chatClient = new ChatClient(tcpClient, nwStream);
            chatClient.GenerateNewConnectionInfo();
            OnClientConnected(chatClient);
        }

        public void StartListening()
        {
            //---listen at the specified IP and port no.---
            Global.logger.WriteLine("Listening...");
            listener.Start();
        }

        private void sendMessageToAll(ChatMessage chatMessage)
        {
            //byte[] buffer = Encoding.ASCII.GetBytes(chatMessage.Content);

            foreach (var client in clients)
            {

                NetworkStream nwStream = client.GetNetworkStream();
               // string dataReceived = Encoding.ASCII.GetString(buffer, 0, buffer.Length);
                //Global.logger.WriteLine("Received : " + dataReceived);

                Serialiser.Serializer.Serialize(nwStream, chatMessage);
            }
        }

        private async Task sendMessageToClient(ChatClient client, string msg)
        {
            byte[] msgBytes  = Encoding.ASCII.GetBytes(msg);
            NetworkStream nwStream = client.GetNetworkStream();
            await nwStream.WriteAsync(msgBytes, 0, msgBytes.Length);
        }

        private async Task sendMessageToClient(ChatClient client, ChatMessage msg)
        {
            string data= Serialiser.Serializer.SerializeObject<ChatMessage>(msg);
            byte[] msgBytes = Encoding.ASCII.GetBytes(data);
            await sendMessageToClient(client, data);
        }


        private async Task sendMessageToClients(List<ChatClient> chatClients, ChatMessage msg)
        {
            int i = 0;
            foreach (var client in chatClients)
            {
                if(i % 2 == 0)
                    await sendMessageToClient(client, msg);

                i++;
            }
        }

        /*private async Task sendMessageToAll(byte[] buffer, int bytesRead)
        {
            foreach (var client in clients)
            {
                NetworkStream nwStream = client.GetNetworkStream();
                //byte[] buffer = new byte[client.SendBufferSize];

                //Console.WriteLine("Sending back : " + dataReceived);
                await nwStream.WriteAsync(buffer, 0, bytesRead);
            }
        }*/

        public async Task Listen(ChatClient client)
        {
            while (client != null && client.Client.Connected)
            {

                try
                {
                    //---get the incoming data through a network stream---
                    NetworkStream nwStream = client.GetNetworkStream();
                    byte[] buffer = new byte[client.Client.ReceiveBufferSize];

                    //---read incoming stream---
                    int bytesRead = await nwStream.ReadAsync(buffer, 0, client.Client.ReceiveBufferSize);

                    //---convert the data received into a string---
                    string dataReceived = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                    var m=Serialiser.Serializer.DeserializeObject<ChatMessage>(dataReceived);
                   // Global.logger.WriteLine("Received : " + dataReceived);

                    //---write back the text to the client---

                    if(m.GetType() == typeof(ChatMessage)) //is message
                    {
                        if(m.MessageType == ChatMessageType.Global)
                            await sendMessageToClients(clients,m);
                        //else if(m.MessageType == ChatMessageType.PM)

                    }
                    
                }
                catch(Exception e)
                {
                    if (!client.Client.Connected)
                        OnClientDisconnected(client);
                }


            }

        }
        public void Disconnect()
        {
            foreach (var client in clients)
            {
                Global.logger.WriteLine($"Disconnecting");
                client.Client.Close();
            }
            
            listener.Stop();
            Console.ReadLine();
        }

    }
}
