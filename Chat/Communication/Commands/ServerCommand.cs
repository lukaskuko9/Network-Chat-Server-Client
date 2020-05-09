using ClientApp;
using ServerClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static Chat.Communication.Commands.CommunicationCommandBase;

namespace Chat.Communication.Commands
{
    public interface ICommunicationCommand
    {
        Task SendRaw(ChatClient client, string RawMessage);
        CommandData RunCommand(string Command);
        CommandDelegate CommandParse(string cmd);
    }

    public struct CommandData
    {
        public string Message { get; set; }
        public bool Success { get; set; }
        public object Result { get; set; }

        public CommandData(string Message, bool Success, object Result=null)
        {
            this.Message = Message;
            this.Success = Success;
            this.Result = Result;
        }
    }

    public abstract class CommunicationCommandBase : ICommunicationCommand
    {
        public static List<CommunicationCommandBase> CommandBaseTypes { get; set; } = new List<CommunicationCommandBase>();
        public const int PREFIX_LENGTH = 3;

        public delegate CommandData CommandDelegate(string cmd);
        public string Prefix { get; private set; }

        public abstract CommandDelegate CommandParse(string cmd);

        public CommandData RunCommand(string CmdStr)
        {
            string RawCmdStr = CmdStr.Skip(Prefix.Length).ToString(); //get command without prefix
            CommandDelegate command = CommandParse(RawCmdStr);
            return command.Invoke(RawCmdStr);
        }


        public CommunicationCommandBase(string prefix)
        {
            this.Prefix = prefix;
            CommandBaseTypes.Add(this);
        }

        protected CommandData NoCommand(string cmd)
        {
            return new CommandData("No command found", false, null); 
        }

        public static CommunicationCommandBase GetWhichCommand(string CommandStr)
        {
            string prefix = CommandStr.Substring(PREFIX_LENGTH).ToString();
            foreach (var commType in CommandBaseTypes)
            {
                if(commType.Prefix.StartsWith(prefix))
                {
                    return commType;
                }
            }
            return null;
        }

        public async Task SendRaw(ChatClient client, string CommandStr)
        {
            string rawmessage = Prefix + CommandStr;
            await client.SendAsync(rawmessage);
        }
    }

    public class ServerCommand : CommunicationCommandBase
    {
        const string PREFIX = "s//";
        public ServerCommand() : base(PREFIX)
        {

        }

        private CommandData testMessage(string cmd)
        {
            return new CommandData("Message", true);
        }


        public override CommandDelegate CommandParse(string RawCmdStr)
        {
            CommandDelegate command = NoCommand;
            if (Regex.IsMatch(RawCmdStr, "msg"))
            {
                command = testMessage;
            }
            return command;
        }
    }
}
