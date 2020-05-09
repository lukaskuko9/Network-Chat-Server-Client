namespace Chat.Communication.Commands
{
    public class MessageCommand : CommunicationCommandBase
    {
        const string PREFIX = "msg";
        public MessageCommand() : base(PREFIX)
        {

        }

        public override CommandDelegate CommandParse(string cmd)
        {
            CommandDelegate c = SendMsgToAll;
            
            return c;
        }

        private CommandData SendMsgToAll(string msg)
        {
            string msgFormat = $"{chatMessage.Time.ToLongTimeString()};{chatMessage.Author};{chatMessage.Content}";
            return new CommandData(msg,true);
        }
    }
}
