using ChatApp;
using ChatApp.Chat;
using ChatApp.Serialization;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;

namespace ChatApp.Chat
{
    [Serializable()]
    [AutoSerializableAttribute(typeof(ChatMessage))]
    public class ChatMessage : AutoSerializable
    {
        internal ChatMessage(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public enum ChatMessageTargets
        {
            Global, PM, Group
        }
        public enum ChatMessageType
        {
            Message,Server,Connection
        }

        public DateTime Time { get; set; }
        public ChatUser Author { get; set; }

        public string ChatTime => Time.ToShortTimeString();

        public string Content { get; set; }
        public ChatMessageTargets MessageTargets {
            get
            {
                ChatMessageTargets msgType = ChatMessageTargets.Global;
                if (Receivers.Count == 1)
                    msgType = ChatMessageTargets.PM;
                else if (Receivers.Count > 1)
                    msgType = ChatMessageTargets.Group;

                return msgType;
            }
        }
        public ChatMessageType MessageType { get; set; } = ChatMessageType.Message;

        public HashSet<ChatUser> Receivers { get; private set; }

        public ChatMessage(ChatUser author, string content)
        {
            this.Author = author;
            this.Content = content;
            this.Time = DateTime.Now;
            this.Receivers = new HashSet<ChatUser>();
        }

        public override string ToString()
        {
            return $"[{ChatTime}] {Author}: {Content}";
        }
    }
}
