using ChatApp;
using ClientApp;
using ServerClient;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using static ClientApp.ChatMessage;

namespace Server
{
    public static class Extension
    {
        public static void GenerateNewConnectionInfo(this ChatClient client)
        {
            client.ConnectionInfo = new ConnectionInfo();
        }
    }

    class Server
    {
        TcpListener listener;
        List<ChatUser> clients = new List<ChatUser>();

        public delegate void ClientConnection(ChatUser client);
        public event ClientConnection OnClientConnected;
        public event ClientConnection OnClientDisconnected;

        public Server(string SERVER_IP, int PORT_NO)
        {
            IPAddress localAdd = IPAddress.Parse(SERVER_IP);
            listener = new TcpListener(localAdd, PORT_NO);

            OnClientConnected += async (ChatUser user) =>
            {
                NetworkStream nwStream = user.Client.GetNetworkStream();

                Global.logger.WriteLine($"Client [ID: {user.Client.ConnectionInfo.ID} Name: {user.Username}] connected...");
                clients.Add(user);

                string data=Serializer.Serializer.SerializeObject(user.Client.ConnectionInfo); 
                await SendMessageToClient(user, data);// send user connection info

                await Listen(user); //listen to messages from this client
            };

            OnClientDisconnected += (ChatUser user) =>
            {
                Global.logger.WriteLine($"Client [ID: {user.Client.ConnectionInfo.ID} Name: {user.Username}] disconnected...");
                clients.Remove(user);
                user.Client.TcpClient.Dispose();
                user = null;
            };
        }

        public async Task AcceptTcpClient()
        {
            TcpClient tcpClient = await listener.AcceptTcpClientAsync();
            ChatClient chatClient = new ChatClient(tcpClient);
            ChatUser chatUser = new ChatUser(chatClient);

            chatClient.GenerateNewConnectionInfo();
            OnClientConnected(chatUser);
        }

        public void StartListening()
        {
            //---listen at the specified IP and port no.---
            Global.logger.WriteLine("Listening...");
            listener.Start();
        }

        public async Task SendMessageToClient(ChatUser client, string msg)
        {
            byte[] msgBytes  = Encoding.ASCII.GetBytes(msg);
            NetworkStream nwStream = client.Client.GetNetworkStream();
            await nwStream.WriteAsync(msgBytes, 0, msgBytes.Length);
        }

        public async Task SendMessageToClient(ChatUser client, ChatMessage msg)
        {
            string data= Serializer.Serializer.SerializeObject<ChatMessage>(msg);
            byte[] msgBytes = Encoding.ASCII.GetBytes(data);
            await SendMessageToClient(client, data);
        }
        public async Task SendMessageToAllClients(ChatMessage msg)
        {
            foreach (var client in clients)
                await SendMessageToClient(client, msg);
        }

        public async Task SendMessageToClients(ICollection<ChatUser> chatClients, ChatMessage msg)
        {
            foreach (var client in chatClients)
                    await SendMessageToClient(client, msg);
        }

        async Task handleDeserializedObject(dynamic deserializedObject)
        {
            if (deserializedObject.GetType() == typeof(ChatMessage)) //is message
            {
                ChatMessage m = deserializedObject;
                Global.logger.WriteLine(m.ToString());
                if (m.MessageType == ChatMessageType.Global)
                    await SendMessageToAllClients(m);
                else
                    await SendMessageToClients(m.Receivers, m);
            }
            else
                throw new Exception("error: couldnt handle deserialized object");
        }

        public async Task Listen(ChatUser client)
        {
            while (client != null && client.Client.TcpClient.Connected)
            {
                try
                {
                    NetworkStream nwStream = client.Client.GetNetworkStream();
                    byte[] buffer = new byte[client.Client.TcpClient.ReceiveBufferSize];
                    int bytesRead = await nwStream.ReadAsync(buffer, 0, client.Client.TcpClient.ReceiveBufferSize);

                    string dataReceived = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                    dynamic deserializedObject = Serializer.Serializer.DeserializeObject(dataReceived);
                    await handleDeserializedObject(deserializedObject);
                }
                catch(Exception e)
                {
                    if (!client.Client.TcpClient.Connected)
                        OnClientDisconnected(client);
                    else
                        Global.logger.WriteLine(e.Message);
                }
            }
        }
        public void Stop()
        {
            foreach (var client in clients)
            {
                Global.logger.WriteLine($"Disconnecting {client.Client.ConnectionInfo.ID}");
                client.Client.TcpClient.Close();
            }
            
            listener.Stop();
            Console.ReadLine();
        }

    }
}
