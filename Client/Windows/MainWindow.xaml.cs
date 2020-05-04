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
using System.Windows.Navigation;
using System.Windows.Shapes;
using ServerClient;

namespace ClientApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ChatUser user = null;

        public MainWindow(ChatUser client)
        { 
            InitializeComponent();
            user = client;
            receive();
        }

        private async void receive()
        {
            this.user.Client.OnMessageReceived += Client_OnMessageReceived;
            await user.Client.ReceiveAsync();
        }

        private void Client_OnMessageReceived(ChatMessage chatMessage)
        {
            Chat.Text += chatMessage + Environment.NewLine;
        }

        private async void btn_Send(object sender, RoutedEventArgs e)
        {
            string msg = chatMessage_TextBox.Text;
            if (msg == string.Empty)
                return;

            ChatMessage chatMessage = new ChatMessage(user, msg);

            if (user.Client != null)
                await user.Client.SendAsync(ChatMessage.GetStringFromMessage(chatMessage));
        }
    }
}
