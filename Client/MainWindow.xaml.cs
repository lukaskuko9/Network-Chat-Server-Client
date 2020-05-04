using System;
using System.Windows;
using System.Windows.Input;


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
            chatMessage_TextBox.KeyDown += ChatMessage_TextBox_KeyDown;
            user = client;
            receive();
        }

        private void ChatMessage_TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key==Key.Enter)
            {
                string msg = chatMessage_TextBox.Text;
                SendMessage(msg);
            }
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

        async void SendMessage(string msg)
        {  
            if (msg == string.Empty)
                return;

            ChatMessage chatMessage = new ChatMessage(user, msg);

            if (user.Client != null)
                await user.Client.SendAsync(ChatMessage.GetStringFromMessage(chatMessage));

            chatMessage_TextBox.Text = string.Empty;
        }

        private void btnClick_Send(object sender, RoutedEventArgs e)
        {
            string msg = chatMessage_TextBox.Text;
            SendMessage(msg);
        }
    }
}