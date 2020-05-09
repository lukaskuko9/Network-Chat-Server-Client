﻿using System;
using System.ComponentModel;

namespace ClientApp
{
    public class ChatMessage
    {
        public DateTime Time { get; set; }
        public ChatUser Author { get; set; }
        public string Content { get; set; }

        static private IChatMessageFormat _chatMessageFormat = new ChatMessageFormatClassic();
        public ChatMessage(ChatUser author, string content)
        {
            this.Author = author;
            this.Content = content;
            this.Time = DateTime.Now;
        }

        public static ChatMessage GetMessageFromString(string textMessage)
        {
            return _chatMessageFormat.ParseStringToMessage(textMessage);
        }

        public static string GetStringFromMessage(ChatMessage chatMessage)
        {
            return _chatMessageFormat.ParseMessageToString(chatMessage);
        }

        public override string ToString()
        {
            return $"{Time.ToShortTimeString()} {Author}: {Content}";
        }
    }
}