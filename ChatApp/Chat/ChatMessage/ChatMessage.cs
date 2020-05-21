using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace ClientApp
{
    [Serializable()]
    public class ChatMessage : ISerializable
    {
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

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(Time), this.Time);
            info.AddValue(nameof(Content), this.Content);
            info.AddValue(nameof(Author), this.Author);
            info.AddValue(nameof(MessageTargets), this.MessageTargets);
            info.AddValue(nameof(MessageType), this.MessageType);
            info.AddValue(nameof(Receivers), this.Receivers);
        }

        public ChatMessage(SerializationInfo info, StreamingContext context)
        {
            Time = (DateTime)info.GetValue(nameof(Time), typeof(DateTime));
            Content = (string)info.GetString(nameof(Content));
            Author = (ChatUser)info.GetValue(nameof(Author), typeof(ChatUser));
            Receivers = (HashSet<ChatUser>)info.GetValue(nameof(Receivers), typeof(HashSet<ChatUser>));
            MessageType = (ChatMessageType)info.GetValue(nameof(MessageType), typeof(ChatMessageType));
        }

        public override string ToString()
        {
            return $"[{ChatTime}] {Author}: {Content}";
        }
    }
}
