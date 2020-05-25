using ChatApp;
using ChatApp.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Chat
{
    [Serializable()]
    [AutoSerializableAttribute(typeof(ChatUser))]
    public class ChatUser : AutoSerializable
    {
        public ChatClient Client { get; set; }
        public string Username { get; set; }
        public  uint? ID {
            get
            {
                try {
                    return Client.ConnectionInfo.ID;
                }
                catch
                {
                    return null;
                }
            }
        }

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

        public ChatUser(SerializationInfo info, StreamingContext context) : base(info,context)
        {
        }
    }
}
