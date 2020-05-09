using ChatApp;
using ClientApp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ServerClient
{
    public class ChatClient
    {
        public delegate void MessageDelegate(ChatMessage chatMessage);
        public event MessageDelegate OnMessageReceived;

        public ConnectionInfo ConnectionInfo { get; private set; }
        public TcpClient Client { get; private set; }
        NetworkStream nwStream;

        public ChatClient()
        {
            Client = new TcpClient();   
        }

        public ChatClient(TcpClient tcpClient, NetworkStream nwStream)
        {
            Client = tcpClient;
            ConnectionInfo = new ConnectionInfo();
            this.nwStream = nwStream;
        }

        public async Task Connect(string SERVER_IP, int PORT_NO)
        {
            await Client.ConnectAsync(SERVER_IP, PORT_NO);
            nwStream = Client.GetStream();
            //ConnectionInfo = Serialiser.Serializer.Deserialize(nwStream); 
        }

        public async Task SendAsync(string msg)
        {
            //---create a TCPClient object at the IP and port no.---

            byte[] bytesToSend = ASCIIEncoding.ASCII.GetBytes(msg);

            //---send the text---
            Console.WriteLine("Sending : " + msg    );
            await nwStream.WriteAsync(bytesToSend, 0, bytesToSend.Length);
        }

        static int i= 0;
        public async Task ReceiveAsync()
        {
            while (true)
            {
                byte[] bytesToRead = new byte[Client.ReceiveBufferSize];
                int bytesRead = await nwStream.ReadAsync(bytesToRead, 0, Client.ReceiveBufferSize);
                string msg = Encoding.ASCII.GetString(bytesToRead, 0, bytesRead);
                Console.WriteLine("Received : " + msg);

                ChatMessage chatMessage = Serialiser.Serializer.DeserializeObject<ChatMessage>(msg);
                OnMessageReceived(chatMessage);
            }
        }

        public NetworkStream GetNetworkStream() => nwStream;
    }
}