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
        public virtual bool IsServer { get; protected set; } = false;

        public ChatUser(ChatClient client)
        {
            this.Client = client;
        }

        public ChatUser(string username)
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
            info.AddValue(nameof(IsServer), this.IsServer);
        }

        public ChatUser(SerializationInfo info, StreamingContext context)
        {
            Username = info.GetString(nameof(Username));
            Client = (ChatClient)info.GetValue(nameof(ChatClient),typeof(ChatClient));
            IsServer = info.GetBoolean(nameof(IsServer));
        }
    }

    [Serializable()]
    public class ChatServerUser : ChatUser, ISerializable
    {
        public override bool IsServer => true;

        public ChatServerUser() : base("server")
        {

        }

        public ChatServerUser(SerializationInfo info, StreamingContext context) : base(info,context)
        {
        }
    }
}
