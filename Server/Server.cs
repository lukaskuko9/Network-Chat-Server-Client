using ChatApp;
using ChatApp.Chat;
using ChatApp.Logger;
using ChatApp.Serialization;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static ChatApp.Chat.ChatMessage;

namespace Server
{
    public static class Extension
    {
        public static void GenerateNewConnectionInfo(this ChatClient client)
        {
            client.ConnectionInfo = new ConnectionInfo();
        }
    }

    internal class Server
    {
        public ChatUser ServerUser { get; private set; }

        TcpListener listener;
        public List<ChatUser> Clients { get; private set; } = new List<ChatUser>();

        public delegate void ClientConnection(ChatUser client);
        public event ClientConnection OnClientConnected;
        public event ClientConnection OnClientDisconnected;

        public Server(string SERVER_IP, int PORT_NO)
        {
            ServerUser = new ChatUser("server"); //"server" is the name of server "user"
            IPAddress localAdd = IPAddress.Parse(SERVER_IP);
            listener = new TcpListener(localAdd, PORT_NO);
        }

        private async Task wait(ChatClient chatClient)
        {
            dynamic o = null;
            while (o == null)
            {
                NetworkStream nwStream = chatClient.GetNetworkStream();
                byte[] buffer = new byte[chatClient.TcpClient.ReceiveBufferSize];
                int bytesRead = await nwStream.ReadAsync(buffer, 0, chatClient.TcpClient.ReceiveBufferSize);

                string dataReceived = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                o = Serializer.DeserializeObject(dataReceived);
                if (o.GetType() != typeof(ChatUser))
                    o = null;
            }
        }

        public async Task AcceptTcpClient()
        {
            TcpClient tcpClient = await listener.AcceptTcpClientAsync();
            ChatClient chatClient = new ChatClient(tcpClient);
            ChatUser chatUser = new ChatUser(chatClient);

            chatClient.GenerateNewConnectionInfo();

            string data = Serializer.SerializeObject(chatUser.Client.ConnectionInfo);
            await SendMessageToClient(chatUser, data); // send user connection info
            
            dynamic o = null;
            while (o == null)
            {
                NetworkStream nwStream = chatClient.GetNetworkStream();
                byte[] buffer = new byte[chatClient.TcpClient.ReceiveBufferSize];
                int bytesRead = await nwStream.ReadAsync(buffer, 0, chatClient.TcpClient.ReceiveBufferSize);

                string dataReceived = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                o = Serializer.DeserializeObject(dataReceived);
                if (o.GetType() != typeof(ChatUser))
                    o = null;
            }
            chatUser.Username = o.Username;
            OnClientConnected(chatUser);
        }

        public void StartListening()
        {
            //---listen at the specified IP and port no.---
            Logger.Instance.WriteLine("Listening...");
            listener.Start();
        }

        public async Task<bool> SendMessageToClient(ChatUser client, string msg)
        {
            try
            {
                byte[] msgBytes = Encoding.ASCII.GetBytes(msg);
                NetworkStream nwStream = client.Client.GetNetworkStream();
                await nwStream.WriteAsync(msgBytes, 0, msgBytes.Length);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> SendMessageToClient(ChatUser client, ChatMessage msg)
        {
            string data = Serializer.SerializeObject<ChatMessage>(msg);
            byte[] msgBytes = Encoding.ASCII.GetBytes(data);
            bool success = await SendMessageToClient(client, data);
            return success;
        }

        public async Task SendMessageToAllClients(ChatMessage msg)
        {
            foreach (var client in Clients)
                await SendMessageToClient(client, msg);
        }

        public async Task SendMessageToClients(ICollection<ChatUser> chatClients, ChatMessage msg)
        {
            foreach (var client in chatClients)
                    await SendMessageToClient(client, msg);
        }

        async Task<dynamic> handleDeserializedObject(dynamic deserializedObject)
        {
            dynamic o = null;

            if (deserializedObject.GetType() == typeof(ChatMessage)) //is message
            {
                ChatMessage m = deserializedObject;
                Logger.Instance.WriteLine(m.ToString());
                if (m.MessageTargets == ChatMessageTargets.Global)
                    await SendMessageToAllClients(m);
                else
                    await SendMessageToClients(m.Receivers, m);
            }
            else
                throw new Exception("error: couldnt handle deserialized object");

            return o;
        }

        private async Task<dynamic> ListenOnce(ChatUser client)
        {
            try
            {
                NetworkStream nwStream = client.Client.GetNetworkStream();
                byte[] buffer = new byte[client.Client.TcpClient.ReceiveBufferSize];
                int bytesRead = await nwStream.ReadAsync(buffer, 0, client.Client.TcpClient.ReceiveBufferSize);

                string dataReceived = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                dynamic deserializedObject = Serializer.DeserializeObject(dataReceived);
                await handleDeserializedObject(deserializedObject);
                return deserializedObject;
            }
            catch (Exception e)
            {
                if (!client.Client.TcpClient.Connected)
                    OnClientDisconnected(client);

                return e;
            }
        }

        public async Task ListenTo(ChatUser client)
        {
            while (client != null && client.Client.TcpClient.Connected)
            {
                dynamic o = await ListenOnce(client);
                try
                {
                    Exception e = o;
                    //Message = "An existing connection was forcibly closed by the remote host"
                    if (e.InnerException.Message != "An existing connection was forcibly closed by the remote host")
                        Logger.Instance.WriteLine(o.ToString());
                }
                catch { }
            }                
        }

        public void DisconnectUser(ChatUser client)
        {
            client.Client.TcpClient.Close();
        }

        public void Stop()
        {
            foreach (var client in Clients)
            {
                Logger.Instance.WriteLine($"Disconnecting {client.Client.ConnectionInfo.ID}");
                client.Client.TcpClient.Close();
            }
            
            listener.Stop();
            Console.ReadLine();
        }

    }
}
