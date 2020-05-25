using System;

namespace ChatApp.Logger
{
    public class ConsoleLogger : ILogger
    {
        public void Write(string messsage)
        {
            Console.Write(messsage);
        }

        public void WriteLine(string message)
        {
            Console.WriteLine(message);
        }
    }
}
