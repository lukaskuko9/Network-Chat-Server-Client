using ChatApp;
using ChatApp.Chat;
using ChatApp.Serialization;
using System;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ChatApp.Chat
{
    [Serializable()]
    [AutoSerializableAttribute(typeof(ChatClient))]
    public class ChatClient : AutoSerializable
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

        internal ChatClient(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public ChatClient(TcpClient tcpClient) : base()
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

                string userStr = Serializer.SerializeObject(user);
                await user.Client.SendAsync(userStr);
                return user;
            }
            catch(Exception e)
            {
                Logger.Logger.Instance.WriteLine(e.Message);
                return null;
            }
        }

        public async Task SendObjectAsync<T>(T obj) where T: ISerializable
        {
            string msg = Serializer.SerializeObject(obj);
            await SendAsync(msg);
        }

        public async Task SendAsync(string msg)
        {
            byte[] bytesToSend = ASCIIEncoding.ASCII.GetBytes(msg);
            Console.WriteLine("Sending : " + msg);
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

                    dynamic deserializedObject = Serializer.DeserializeObject(msg);
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
                            Logger.Logger.Instance.WriteLine(e.Message);
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
    }
}