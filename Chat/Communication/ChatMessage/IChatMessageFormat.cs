namespace ClientApp
{
    interface IChatMessageFormat
    {
        ChatMessage ParseStringToMessage(string textMessage);
        string ParseMessageToString(ChatMessage chatMessage);
    }
}
