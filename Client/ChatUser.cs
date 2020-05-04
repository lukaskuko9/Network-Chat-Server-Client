using ServerClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientApp
{
    class ChatUser
    {
        MyClient client;
        public ChatUser(MyClient client)
        {
            this.client = client;
        }
    }
}
