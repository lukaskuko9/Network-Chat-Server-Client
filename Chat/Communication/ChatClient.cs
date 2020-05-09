using ClientApp;
using System;
using System.Collections.Generic;
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

        public TcpClient TcpClient { get; set; }
        NetworkStream nwStream;
        public ChatClient()
        {
            TcpClient = new TcpClient();   
        }

        public ChatClient(TcpClient tcpClient)
        {
            this.TcpClient = tcpClient;
        }

        public async Task Connect(string SERVER_IP, int PORT_NO)
        {
            await TcpClient.ConnectAsync(SERVER_IP, PORT_NO);
            nwStream = TcpClient.GetStream();
        }

        public async Task SendAsync(string msg)
        {
            //---create a TCPClient object at the IP and port no.---

            byte[] bytesToSend = ASCIIEncoding.ASCII.GetBytes(msg);

            //---send the text---
           // Console.WriteLine("Sending : " + msg    );
            await nwStream.WriteAsync(bytesToSend, 0, bytesToSend.Length);
        }

        public async Task ReceiveAsync()
        {
            while (true)
            {
                byte[] bytesToRead = new byte[TcpClient.ReceiveBufferSize];
                int bytesRead =  await nwStream.ReadAsync(bytesToRead, 0, TcpClient.ReceiveBufferSize);
                string msg = Encoding.ASCII.GetString(bytesToRead, 0, bytesRead);
                Console.WriteLine("Received : " + msg);

                ChatMessage chatMessage = ChatMessage.GetMessageFromString(msg);
                OnMessageReceived(chatMessage);
            }
        }
    }
}