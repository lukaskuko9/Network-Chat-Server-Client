using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Logger
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
