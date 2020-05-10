using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace ClientApp
{
    public enum ChatMessageType
    {
        Global, PM
    }

    [Serializable()]
    public class ChatMessage : ISerializable
    {
        static private IChatMessageFormat _chatMessageFormat = new ChatMessageFormatClassic();

        public DateTime Time { get; set; }
        public ChatUser Author { get; set; }

        public string Content { get; set; }
        public ChatMessageType MessageType { get; private set; }
        public List<ChatUser> Receivers { get; set; }

        public ChatMessage(ChatUser author, string content)
        {
            this.Author = author;
            this.Content = content;
            this.Time = DateTime.Now;
            this.MessageType = ChatMessageType.Global;
        }

        public ChatMessage()
        {

        }

        public static ChatMessage GetMessageFromString(string textMessage)
        {
            return _chatMessageFormat.GetMessageFromString(textMessage);
        }

        public static string GetStringFromMessage(ChatMessage chatMessage)
        {
            return _chatMessageFormat.GetStringFromMessage(chatMessage);
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(Time), this.Time);
            info.AddValue(nameof(Content), this.Content);
            info.AddValue(nameof(Author), this.Author);
            info.AddValue(nameof(MessageType), this.MessageType);
        }

        public ChatMessage(SerializationInfo info, StreamingContext context)
        {
            Time = (DateTime)info.GetValue(nameof(Time), typeof(DateTime));
            Content = (string)info.GetString(nameof(Content));
            Author = (ChatUser)info.GetValue(nameof(Author), typeof(ChatUser));
            MessageType = (ChatMessageType)info.GetValue(nameof(MessageType), typeof(ChatMessageType));
        }
    }
}
