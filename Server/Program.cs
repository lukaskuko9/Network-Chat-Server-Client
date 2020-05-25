#define LOCALHOST 

using ChatApp.Chat;
using ChatApp.Logger;
using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using static ChatApp.Chat.ChatMessage;

namespace Server
{

    internal class Program
    {
        internal static Server server;
        

        async static void AcceptClients()
        {
            while (true)
            {
                try
                {
                    var task = server.AcceptTcpClient();
                    //await task;

                    int timeout = 5000;
                    if (await Task.WhenAny(task, Task.Delay(timeout)) == task)
                    {
                        //Console.WriteLine("Client accepted");
                    }
                }
                catch(Exception e)
                {
                    Logger.Instance.WriteLine(e.Message);
                }

            }
        }


        static void Main(string[] args)
        {
#if LOCALHOST
            string serverip = "127.0.0.1";      
#else
            string serverip = "25.148.88.117";
#endif

            int port = 5000;
            if (args.Length >= 1)
            {
                serverip = args[0];
                port = Convert.ToInt32(args[1]);
            }
          
            server = new Server(serverip, port);
            server.OnClientConnected += async (ChatUser user) =>
            {
                NetworkStream nwStream = user.Client.GetNetworkStream();
                user.Client.ConnectionInfo.Status = ConnectionInfo.ConnectionStatus.Connected;

                Logger.Instance.WriteLine($"Client [ID: {user.Client.ConnectionInfo.ID} Name: {user.Username}] connected...");
                server.Clients.Add(user);

                string msg = $"User {user.Username} joined the server.";
                ChatMessage chatMessage = new ChatMessage(server.ServerUser, msg)
                {
                    MessageType = ChatMessageType.Connection
                };

                await server.SendMessageToAllClients(chatMessage);
                await server.ListenTo(user); //listen to messages from this client
            };

            server.OnClientDisconnected += async (ChatUser user) =>
            {
                string msg = null;
                if(user.Client.ConnectionInfo.Status == ConnectionInfo.ConnectionStatus.Kicked)
                    msg = $"User {user.Username} was kicked by the server.";
                else
                    msg = $"User {user.Username} left the server.";

                ChatMessage chatMessage = new ChatMessage(server.ServerUser, msg)
                {
                    MessageType = ChatMessageType.Connection
                };

                Logger.Instance.WriteLine($"Client [ID: {user.Client.ConnectionInfo.ID} Name: {user.Username}] disconnected...");
                server.Clients.Remove(user);
                user.Client.TcpClient.Dispose();
                user = null;

                try
                {
                    await server.SendMessageToAllClients(chatMessage);
                }
                catch (Exception e)
                {
                    Logger.Instance.WriteLine(e.Message);
                }
            };
            Logger.Instance.WriteLine($"Starting server at {serverip}:{port}");
            server.StartListening();
            AcceptClients();
            while (true)
            {
                string text = Console.ReadLine();
                ServerCommands.ExecuteCommand(text);
            }
        }
    }
}
