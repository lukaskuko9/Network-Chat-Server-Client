using ChatApp;
using ServerClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ClientApp
{
    [Serializable()]
    public class ChatUser : ISerializable
    {
        public ChatClient Client { get; set; }
        public string Username { get; set; }
        
        public ChatUser(ChatClient client)
        {
            this.Client = client;
        }

        internal ChatUser(string username)
        {
            this.Username = username;
        }

        public override string ToString()
        {
            return Username;
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(Username), this.Username);
            info.AddValue(nameof(ChatClient), this.Client);
        }

        public ChatUser(SerializationInfo info, StreamingContext context)
        {
            // Reset the property value using the GetValue method.
            Username = info.GetString(nameof(Username));
            Client = (ChatClient)info.GetValue(nameof(ChatClient),typeof(ChatClient));
        }
    }
}
