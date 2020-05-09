using Chat.Communication.Commands;
using System;

namespace ClientApp
{
    abstract class ChatMessageBase : IChatMessageFormat
    {
        public static MessageCommand CmdMessage { get; set; } = new MessageCommand();

        public ChatMessage ParseFroMString(string textMessage)
        {
            var a =CmdMessage.CommandParse(textMessage);
            return a.Invoke(textMessage).Result as ChatMessage;
        }

        public ChatMessage ParseStringToMessage(string textMessage)
        {
            string[] split = textMessage.Split(';');

            if (split.Length < 2)
                return null;
            DateTime time = new DateTime();
            try
            {
                time = DateTime.Parse(split[0]);
            }
            catch
            {
                Console.WriteLine("Couldnt parse dateTime from string");
            }


            ChatUser user = new ChatUser(split[1]);
            string content = split[2];
            ChatMessage chatMessage = new ChatMessage(user, content);
            chatMessage.Time = time;
            return chatMessage;
        }

        public string ParseMessageToString(ChatMessage chatMessage)
        {
            var a =CmdMessage.CommandParse(chatMessage.Content).Invoke(chatMessage.Content);
            return a.Message;
        }
    }

    class ChatMessageFormatClassic : ChatMessageBase
    {
        public ChatMessage GetMessageFromString(string textMessage)
        {
            string[] split = textMessage.Split(';');

            if (split.Length < 2)
                return null;
            DateTime time = new DateTime();
            try
            {
                time = DateTime.Parse(split[0]);
            }
            catch
            {
                Console.WriteLine("Couldnt parse dateTime from string");
            }


            ChatUser user = new ChatUser(split[1]);
            string content = split[2];
            ChatMessage chatMessage = new ChatMessage(user, content);
            chatMessage.Time = time;
            return chatMessage;
        }

        public string GetStringFromMessage(ChatMessage chatMessage)
        {
            return $"{chatMessage.Time.ToLongTimeString()};{chatMessage.Author};{chatMessage.Content}";
        }
    }
}
