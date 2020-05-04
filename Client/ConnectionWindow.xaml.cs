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
            var c = await connect();
            client = c;

            if(c != null)
            {
                MainWindow m = new MainWindow(c);
                m.Show();
                this.Close();
            }
        }

        async Task<ChatUser> connect()
        {
            ChatClient client = new ChatClient();
            ChatUser user = null;
            try
            { 
                await client.Connect(IP_TextBox.Text, Convert.ToInt32(Port_TextBox.Text));
                user = new ChatUser(client);
                user.Username = username_Textbox.Text;
                if(user.Username.Length > 20)
                {
                    MessageBox.Show("Max username length is limited to 20 characters!");
                    return null;
                }
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
