using ServerClient;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ClientApp
{
    public class ChatUser
    {
        public static uint LAST_ID { get; private set; } = 0;

        public ChatClient ChatClient { get; set; }
        public string Username { get; set; }
        public uint ID { get; set; }

        public ChatUser(ChatClient client)
        {
            this.ChatClient = client;
            this.ID = ++LAST_ID;
        }

        public ChatUser(TcpClient tcpClient)
        {
            this.ChatClient = new ChatClient(tcpClient);
        }

        internal ChatUser(string username)
        {
            this.Username = username;
        }

        public override string ToString()
        {
            return Username;
        }

        public async void SendMessageToAll(string msg)
        {
            if (msg == string.Empty)
                return;

            ChatMessage chatMessage = new ChatMessage(this, msg);

            if (this.ChatClient != null)
            {
                var raw = ChatMessage.GetStringFromMessage(chatMessage);
                await this.ChatClient.SendAsync(raw);
            }
        }
    }
}
