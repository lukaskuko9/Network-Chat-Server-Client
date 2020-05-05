using System;

namespace ClientApp
{
    class ChatMessageFormatClassic : IChatMessageFormat
    {
        public int MyProperty { get; set; }
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
