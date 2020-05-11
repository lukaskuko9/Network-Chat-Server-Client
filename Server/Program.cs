using ClientApp;
using System;
using System.Linq;

namespace Server
{

    class Program
    {
        static Server server;
        

        async static void AcceptClients()
        {
            while (true)
            {
                await server.AcceptTcpClient();
            }
        }

        static void Main(string[] args)
        {
            string serverip = "127.0.0.1";
            int port = 5000;
            if (args.Length >= 1)
            {
                serverip = args[0];
                port = Convert.ToInt32(args[1]);
            }
          
            server = new Server(serverip, port);
            Global.logger.WriteLine($"Starting server at {serverip}:{port}");
            server.StartListening();
            AcceptClients();

            ChatServerUser serverUser = new ChatServerUser();

            while(true)
            {
                // server.Listen(client);
                string text = Console.ReadLine();
                var msg = new ChatMessage(serverUser, text);
                server.SendMessageToAllClients(msg).Wait();
            }
        }
    }
}
