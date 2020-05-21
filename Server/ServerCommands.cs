using ClientApp;
using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;

namespace Server
{
    public class ServerCommands
    {
        public delegate void Command(string arg);
        static char commandChar = '-';
        
        private static IList<string> getCmdParameters(string cmd)
        {
            var list = new List<string>();
            string[] parameter = cmd.Split(' ');

            foreach (var par in parameter)
            {
                if (par.StartsWith(commandChar.ToString()))
                    list.Add(par);
            }

            return list;
        }

        static Server server => Program.server;

        public static void NoCommand(string arg)
        {
            Global.logger.WriteLine("Command not recognized");
        }

        public static void ExecuteCommand(string cmd)
        {
            object method = null;

            var type = typeof(ServerCommands);

            MethodInfo[] methodInfos = type.GetMethods(BindingFlags.Public | BindingFlags.Static);
            foreach (var mInfo in methodInfos)
            {
                var att = mInfo.GetCustomAttribute<CommandAttribute>();
                if (att != null && 
                    (cmd.StartsWith(att.CommandTrigger) || (cmd + " ").StartsWith(att.CommandTrigger))
                    ) // if method has CommandAttribute
                {

                    // fixes situation - would try to access negative index of a string
                    if (att.CommandTrigger.Length > cmd.Length - att.CommandTrigger.Length)
                        cmd += " ";

                    string arg = cmd.Substring(att.CommandTrigger.Length, cmd.Length - att.CommandTrigger.Length);
                    method = mInfo;
                    mInfo.Invoke(null, new object[] { arg });
                    break;
                }
            }

            if (method == null)
                NoCommand("");
        }

        static IList<string> getCommandParameters(string cmd)
        {
            var list = new List<string>();
            string[] parameter = cmd.Split(commandChar);

            foreach (var par in parameter)
            {
                if(par != string.Empty)
                    list.Add(commandChar + par);
            }
            return list;
        }


        [Command("/list")]
        public static void ListClients(string arg)
        {
            if(server.Clients.Count == 0)
            {
                Console.WriteLine("There are no users connected to server");
                return;
            }

            int count = 100;
            if(Int32.TryParse(arg, out int res))
            {
                count = res;
            }
            for (int i = 0; i < count && i < server.Clients.Count; i++)
            {
                ChatUser chatUser = server.Clients[i];
                string clientIP = ((IPEndPoint)chatUser.Client.TcpClient.Client.RemoteEndPoint).Address.ToString();
                Console.WriteLine($"[ID: {chatUser.Client.ConnectionInfo.ID}][IP: {clientIP}] [Username: {chatUser.Username}]");
            }
        }

        [Command("/kick")]
        public static void Kick(string arg)
        {
            try
            {
                int ID = Convert.ToInt32(arg);
                ChatUser user = server.Clients.Find((u) => u.Client.ConnectionInfo.ID == ID);
                user.Client.ConnectionInfo.Status = ChatApp.ConnectionInfo.ConnectionStatus.Kicked;
                server.Disconnect(user);
            }
            catch (Exception e){
                Console.WriteLine(e.Message);
            }
        }

        [Command("/say")]
        public async static void SayCmd(string arg)
        {
            ChatMessage msg;
            var parameters = getCommandParameters(arg);
            var content = arg;
            if (parameters.Count != 0)
            {
                msg = new ChatMessage(server.ServerUser, arg);
                int ignoreCount = 0;
                foreach (var param in parameters)
                {
                    ignoreCount += param.Length;

                    if (param.StartsWith("-s"))
                        msg.MessageType = ChatMessage.ChatMessageType.Server;
                    else if (param.StartsWith("-c"))
                        msg.MessageType = ChatMessage.ChatMessageType.Connection;
                    else if (param.Contains("-m"))
                        msg.MessageType = ChatMessage.ChatMessageType.Message;
                    else
                    {
                        Console.WriteLine($"Unknown parameter {param}");
                        return;
                    }
                }
                var att = MethodInfo.GetCurrentMethod().GetCustomAttributes();
             //   content = arg.Substring(0, ignoreCount + att.CommandTrigger.Length);
            }
            else
            {
                msg = new ChatMessage(server.ServerUser, arg);
            }
            


            Console.WriteLine(msg);
            await server.SendMessageToAllClients(msg);
        }

        
    }
}
