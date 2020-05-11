using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace ClientApp
{
    [Serializable()]
    public class ChatMessage : ISerializable
    {
        public enum ChatMessageType
        {
            Global, PM, Group
        }
        public DateTime Time { get; set; }
        public ChatUser Author { get; set; }

        public string ChatTime => Time.ToShortTimeString();
        public bool ServerMessage { get; set; }

        public string Content { get; set; }
        public ChatMessageType MessageType {
            get
            {
                ChatMessageType msgType = ChatMessageType.Global;
                if (Receivers.Count == 1)
                    msgType = ChatMessageType.PM;
                else if (Receivers.Count > 1)
                    msgType = ChatMessageType.Group;

                return msgType;
            }
        }
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
            info.AddValue(nameof(MessageType), this.MessageType);
            info.AddValue(nameof(Receivers), this.Receivers);
        }

        public ChatMessage(SerializationInfo info, StreamingContext context)
        {
            Time = (DateTime)info.GetValue(nameof(Time), typeof(DateTime));
            Content = (string)info.GetString(nameof(Content));
            Author = (ChatUser)info.GetValue(nameof(Author), typeof(ChatUser));
            Receivers = (HashSet<ChatUser>)info.GetValue(nameof(Receivers), typeof(HashSet<ChatUser>));
        }

        public override string ToString()
        {
            return $"[{ChatTime}] {Author}: {Content}";
        }
    }
}
