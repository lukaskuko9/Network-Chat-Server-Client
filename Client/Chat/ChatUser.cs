using ServerClient;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientApp
{

    public class ChatUser
    {
        public ChatClient Client { get; set; }
        public string Username { get; set; }
        
        public ChatUser(ChatClient client)
        {
            this.Client = client;
        }

        internal ChatUser(string username)
        {
            this.Username = username;
        }

        public override string ToString()
        {
            return Username;
        }
    }
}
