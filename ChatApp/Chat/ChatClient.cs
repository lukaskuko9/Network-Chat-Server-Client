using ChatApp;
using ClientApp;
using Server;
using System;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

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

        public string ServerIP { get; private set; }
        public int ServerPort { get; private set; }

        public ChatClient()
        {
            TcpClient = new TcpClient();   
        }

        public ChatClient(TcpClient tcpClient)
        {
            TcpClient = tcpClient;
            this.nwStream = tcpClient.GetStream();
        }

        public async Task<ChatUser> Connect(string SERVER_IP, int PORT_NO, string username)
        {
            if (username == null || username == string.Empty)
                throw new Exception("Username cant be null or empty!");

            try
            {
                TcpClient = new TcpClient();
                await TcpClient.ConnectAsync(SERVER_IP, PORT_NO);

                nwStream = TcpClient.GetStream();

                ServerIP = SERVER_IP;
                ServerPort = PORT_NO;

                ChatUser user = new ChatUser(this);
                user.Client.TcpClient = TcpClient;
                user.Username = username;

                string userStr = Serializer.Serializer.SerializeObject(user);
                await user.Client.SendAsync(userStr);
                return user;
            }
            catch
            {
                return null;
            }
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
            while (TcpClient.Connected)
            {
                try
                {
                    byte[] bytesToRead = new byte[TcpClient.ReceiveBufferSize];
                    int bytesRead = await nwStream.ReadAsync(bytesToRead, 0, TcpClient.ReceiveBufferSize);
                    string msg = Encoding.ASCII.GetString(bytesToRead, 0, bytesRead);

                    dynamic deserializedObject = Serializer.Serializer.DeserializeObject(msg);
                    handleDeserializedObject(deserializedObject);
                }
                catch(Exception e)
                {
                    if (!TcpClient.Connected)
                        TcpClient.Close();
                    else
                    {
                        try 
                        {
                            //send empty message to check if client still connected
                            await this.SendAsync("\0");
                        }
                        catch
                        {
                            //message couldn't be sent, disconnecting client
                            TcpClient.Close();
                            Global.logger.WriteLine(e.Message);
                            break;
                        }
                    }
                }
            }
            if (MessageBox.Show("Disconnected from server. Reconnect?", "Connection loss", 
                MessageBoxButton.YesNo, 
                MessageBoxImage.Error,
                MessageBoxResult.None,
                MessageBoxOptions.DefaultDesktopOnly) == MessageBoxResult.Yes)
            {
                return;
            }
            else
                Application.Current.Shutdown();
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