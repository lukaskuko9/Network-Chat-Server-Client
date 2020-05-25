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
    [AutoSerializable(typeof(ChatGroup))]
    public class ChatGroup : AutoSerializable
    {
        public HashSet<ChatUser> Users { get; private set; }

        public ChatGroup(HashSet<ChatUser> Users)
        {
            this.Users = Users;
        }

        public ChatGroup(SerializationInfo info, StreamingContext context) : base(info,context)
        {
        }
    }
}
