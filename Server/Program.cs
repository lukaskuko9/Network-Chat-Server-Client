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
            string serverip = args[0];
            int port = Convert.ToInt32(args[1]);

            serverip = "25.148.88.117";

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
