using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ServerClient
{
    public class MyClient
    {

        public delegate void MessageDelegate(string str);
        public event MessageDelegate OnMessageReceived;

        TcpClient client;
        NetworkStream nwStream;
        public MyClient()
        {
            client = new TcpClient();   
        }

        public async Task Connect(string SERVER_IP, int PORT_NO)
        {
            await client.ConnectAsync(SERVER_IP, PORT_NO);
            nwStream = client.GetStream();
        }

        public async Task Send(string msg)
        {
            //---create a TCPClient object at the IP and port no.---

            byte[] bytesToSend = ASCIIEncoding.ASCII.GetBytes(msg);

            //---send the text---
            Console.WriteLine("Sending : " + msg    );
            await nwStream.WriteAsync(bytesToSend, 0, bytesToSend.Length);
        }

        public async Task Receive()
        {
            while (true)
            {
                byte[] bytesToRead = new byte[client.ReceiveBufferSize];
                int bytesRead =  await nwStream.ReadAsync(bytesToRead, 0, client.ReceiveBufferSize);
                string msg = Encoding.ASCII.GetString(bytesToRead, 0, bytesRead);
                Console.WriteLine("Received : " + msg);
                OnMessageReceived(msg);
            }
        }
    }
}