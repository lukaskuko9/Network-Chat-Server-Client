using ServerClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ClientApp
{
    /// <summary>
    /// Interaction logic for ConnectionWindow.xaml
    /// </summary>
    public partial class ConnectionWindow : Window
    {
        ChatUser client;

        public ConnectionWindow()
        {
            InitializeComponent();
        }


        private async void btn_Connect(object sender, RoutedEventArgs e)
        {
            var c = await Connect();
            client = c;

            if(c != null)
            {
                MainWindow m = new MainWindow(c);
                m.Show();
                this.Close();
            }
        }


        public async Task<ChatUser> Connect(string username=null, string serverIp = null, int serverPort=0)
        {
            ChatUser user = null;
            ChatClient client = new ChatClient();

            try
            {
                if (username == null)
                    username = username_Textbox.Text;
                if (serverIp == null)
                    serverIp = IP_TextBox.Text;
                if (serverPort == 0)
                    serverPort = Convert.ToInt32(Port_TextBox.Text);

                if (username.Length > 10)
                {
                    MessageBox.Show("Max username length is limited to 10 characters!");
                    return null;
                }
                else if (username.Length == 0)
                {
                    MessageBox.Show("Username can't be empty!");
                    return null;
                }


                user = await client.Connect(IP_TextBox.Text, Convert.ToInt32(Port_TextBox.Text), username);
            }
            catch
            {
                MessageBox.Show($"Unable to connect at  { IP_TextBox.Text}:{Port_TextBox.Text}");
                client = null;
            }
            return user;
        }
    }
}
