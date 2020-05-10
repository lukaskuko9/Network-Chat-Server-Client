using ChatApp;
using ClientApp;
using System;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ServerClient
{
    [Serializable()]
    public class ChatClient : ISerializable
    {
        public delegate void MessageDelegate(ChatMessage chatMessage);
        public event MessageDelegate OnMessageReceived;

        public ConnectionInfo ConnectionInfo { get; set; }
        public TcpClient Client { get; private set; }
        NetworkStream nwStream;

        public ChatClient()
        {
            Client = new TcpClient();   
        }

        public ChatClient(TcpClient tcpClient, NetworkStream nwStream)
        {
            Client = tcpClient;
            this.nwStream = nwStream;
        }

        public async Task Connect(string SERVER_IP, int PORT_NO)
        {
            await Client.ConnectAsync(SERVER_IP, PORT_NO);
            nwStream = Client.GetStream();
        }

        public async Task SendAsync(string msg)
        {
            //---create a TCPClient object at the IP and port no.---

            byte[] bytesToSend = ASCIIEncoding.ASCII.GetBytes(msg);

            //---send the text---
            Console.WriteLine("Sending : " + msg    );
            await nwStream.WriteAsync(bytesToSend, 0, bytesToSend.Length);
        }

        void handleDeserializedObject(dynamic deserializedObject)
        {
            if (deserializedObject.GetType() == typeof(ChatMessage))
            {
                OnMessageReceived(deserializedObject);
            }
            else if (deserializedObject.GetType() == typeof(ConnectionInfo))
            {
                this.ConnectionInfo = deserializedObject;
               
            }
            else
                throw new Exception("error: couldnt handle deserialized object");
        }

        public async Task ReceiveAsync()
        {
            
            while (true)
            {
                byte[] bytesToRead = new byte[Client.ReceiveBufferSize];
                int bytesRead = await nwStream.ReadAsync(bytesToRead, 0, Client.ReceiveBufferSize);
                string msg = Encoding.ASCII.GetString(bytesToRead, 0, bytesRead);
                Console.WriteLine("Received : " + msg);

                dynamic deserializedObject = Serialiser.Serializer.DeserializeObject(msg);
                handleDeserializedObject(deserializedObject);
            }
        }

        public NetworkStream GetNetworkStream() => nwStream;

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(ConnectionInfo), this.ConnectionInfo);
        }

        public ChatClient(SerializationInfo info, StreamingContext context)
        {
            // Reset the property value using the GetValue method.
            ConnectionInfo = (ConnectionInfo)info.GetValue(nameof(ConnectionInfo), typeof(ConnectionInfo));
        }
    }
}