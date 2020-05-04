namespace ClientApp
{
    interface IChatMessageFormat
    {
        ChatMessage GetMessageFromString(string textMessage);
        string GetStringFromMessage(ChatMessage chatMessage);
    }
}
