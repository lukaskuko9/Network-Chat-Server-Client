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
    

            while(true)
            {
               // server.Listen(client);

            }
        }
    }
}
