using ChatApp;
using ClientApp;
using System;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace ServerClient
{
    [Serializable()]
    public class ChatClient : ISerializable
    {
        public delegate void MessageDelegate(ChatMessage chatMessage);
        public event MessageDelegate OnMessageReceived;

        public ConnectionInfo ConnectionInfo { get; set; }
        public TcpClient TcpClient { get; private set; }
        NetworkStream nwStream;

        public ChatClient()
        {
            TcpClient = new TcpClient();   
        }

        public ChatClient(TcpClient tcpClient)
        {
            TcpClient = tcpClient;
            this.nwStream = tcpClient.GetStream();
        }

        public async Task Connect(string SERVER_IP, int PORT_NO)
        {
            await TcpClient.ConnectAsync(SERVER_IP, PORT_NO);
            nwStream = TcpClient.GetStream();
        }

        public async Task SendAsync(string msg)
        {
            byte[] bytesToSend = ASCIIEncoding.ASCII.GetBytes(msg);
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
                byte[] bytesToRead = new byte[TcpClient.ReceiveBufferSize];
                int bytesRead = await nwStream.ReadAsync(bytesToRead, 0, TcpClient.ReceiveBufferSize);
                string msg = Encoding.ASCII.GetString(bytesToRead, 0, bytesRead);

                dynamic deserializedObject = Serializer.Serializer.DeserializeObject(msg);
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
            ConnectionInfo = (ConnectionInfo)info.GetValue(nameof(ConnectionInfo), typeof(ConnectionInfo));
        }
    }
}