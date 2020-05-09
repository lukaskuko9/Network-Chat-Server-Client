using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace ClientApp
{
    [Serializable()]
    public class ChatMessage : ISerializable
    {
        static private IChatMessageFormat _chatMessageFormat = new ChatMessageFormatClassic();

        public DateTime Time { get; set; }

        public ChatUser Author { get; set; }
        public string Content { get; set; }
        public ChatMessage(ChatUser author, string content)
        {
            this.Author = author;
            this.Content = content;
            this.Time = DateTime.Now;
        }

        public static ChatMessage GetMessageFromString(string textMessage)
        {
            return _chatMessageFormat.GetMessageFromString(textMessage);
        }

        public static string GetStringFromMessage(ChatMessage chatMessage)
        {
            return _chatMessageFormat.GetStringFromMessage(chatMessage);
        }

        public override string ToString()
        {
            return $"{Time.ToShortTimeString()} {Author}: {Content}";
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(Time), this.Time);
            info.AddValue(nameof(Content), this.Content);
            info.AddValue(nameof(Author), this.Author);
        }

        public ChatMessage(SerializationInfo info, StreamingContext context)
        {
            Time = (DateTime)info.GetValue(nameof(Time), typeof(DateTime));
            Content = (string)info.GetString(nameof(Content));
            Author = (ChatUser)info.GetValue(nameof(Author), typeof(ChatUser));
        }
    }
}
