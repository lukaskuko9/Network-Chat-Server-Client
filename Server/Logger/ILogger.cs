using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Logger
{
    public interface ILogger
    {
        void Write(string messsage);
        void WriteLine(string message);
    }
}
